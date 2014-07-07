using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using _51API.FullSearch.MODEL;
using _51API.FullSearch.HELP;
using Lucene.Net.Store;
using Lucene.Net.Index;
using System.IO;
using Lucene.Net.Search;
using Lucene.Net.Documents;
using _51API.FullSearch.Cache;

namespace _51API.FullSearch.BLL
{
    public class ProListBLL
    {
        _51API.FullSearch.Cache.ICacheManager Icache = new _51API.FullSearch.Cache.MemoryCacheManager();
        static string indexPath = _51API.FullSearch.HELP.StaticPath.IndexManagerPath;

        #region 返回查询结果列表
        /// <summary>
        /// 返回查询结果列表
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public MODEL.ProListResult getList(MODEL.SearchConditionModel model)
        {
            string key = model.keyWord + model.Category + model.ShopId + model.ResultMsg + model.PageIndex + model.PageSize;
            var re = Icache.Get(key, () => {
                return GetListFromCache(model);
            });
            return re;
            //return GetListFromCache(model);
        }

        static ProListResult GetListFromCache(MODEL.SearchConditionModel model)
        {
            model.keyWord = string.IsNullOrEmpty(model.keyWord) ? "" : model.keyWord;
            FSDirectory directory = FSDirectory.Open(new DirectoryInfo(indexPath), new NoLockFactory());
            IndexReader reader = IndexReader.Open(directory, true);
            IndexSearcher searcher = new IndexSearcher(reader);
            //搜索条件
            // PhraseQuery query =null;
            TermQuery query = null;
            BooleanQuery queryOr = new BooleanQuery();


            #region 拼音的模糊查询
            TermQuery query1 = new TermQuery(new Term("PinYin", model.keyWord));
            queryOr.Add(query1, BooleanClause.Occur.SHOULD);
            #endregion

            if (!string.IsNullOrEmpty(model.Category))
            {
                //如果类别不为空的话，那么类别在指定类别下查询
                TermQuery query2 = new TermQuery(new Term("Category", model.Category));
                queryOr.Add(query2, BooleanClause.Occur.MUST);
            }
            if (model.ShopId != null)
            {
                //如果店铺Id不为空的话，那么在指定的店铺下查询
                TermQuery query2 = new TermQuery(new Term("ShopId", model.ShopId.ToString()));
                queryOr.Add(query2, BooleanClause.Occur.MUST);
            }

            //把用户输入的关键字进行分词
            foreach (string word in HELP.CommonHelp.SplitWords(model.keyWord))
            {
                query = new TermQuery(new Term("Content", word));
                queryOr.Add(query, BooleanClause.Occur.SHOULD);//这里设置 条件为Or关系
                query = new TermQuery(new Term("Title", word));
                queryOr.Add(query, BooleanClause.Occur.SHOULD);

            }
            //query.Add(new Term("content", "C#"));//多个查询条件时 为且的关系
            //queryOr.SetSlop(100); //指定关键词相隔最大距离

            //TopScoreDocCollector盛放查询结果的容器
            TopScoreDocCollector collector = TopScoreDocCollector.create(1000, true);
            searcher.Search(queryOr, null, collector);//根据query查询条件进行查询，查询结果放入collector容器
            //TopDocs 指定0到GetTotalHits() 即所有查询结果中的文档 如果TopDocs(20,10)则意味着获取第20-30之间文档内容 达到分页的效果
            //ScoreDoc[] docs = collector.TopDocs(model.PageIndex-1,model.PageSize).scoreDocs;collector.GetTotalHits()
            ScoreDoc[] docs = collector.TopDocs(model.PageIndex - 1, model.PageSize).scoreDocs;
            //展示数据实体对象集合
            List<MODEL.Pro> ResultList = new List<MODEL.Pro>();
            List<string> Ids = new List<string>();
            for (int i = 0; i < docs.Length; i++)
            {
                int docId = docs[i].doc;//得到查询结果文档的id（Lucene内部分配的id）
                Document doc = searcher.Doc(docId);//根据文档id来获得文档对象Document 
                string Id = doc.Get("Id");
                Ids.Add(Id);

                if (model.ResultMsg)
                {
                    MODEL.Pro pro = new MODEL.Pro();
                    pro.Id = Id;
                    pro.Title = doc.Get("Title");
                    //pro.Content = HELP.CommonHelp.HightLight(model.keyWord, doc.Get("Content"));
                    pro.Content = doc.Get("Content");
                    pro.Category = doc.Get("Category");
                    ResultList.Add(pro);
                }

            }
            return new ProListResult() { ContentList = ResultList, IdList = Ids, Total = collector.GetTotalHits() };
        } 
        #endregion

        #region 新增索引
        /// <summary>
        /// 新增索引
        /// </summary>
        /// <param name="pro"></param>
        /// <returns></returns>
        public MODEL.AjaxResult addPro(MODEL.Pro pro)
        {
            AjaxResult ajax = new AjaxResult();
            if (string.IsNullOrEmpty(pro.Id)||string.IsNullOrEmpty(pro.Title))
            {
                ajax.Result = false;
                ajax.Msg = "新增索引时候Id和标题不允许为空！";
            }
            else
            {
                try
                {
                    IndexManager.Instance.Add(pro);
                    ajax.Msg = "添加成功";
                    ajax.Result = true;
                }
                catch (Exception)
                {
                    ajax.Msg = "添加失败";
                    ajax.Result = false;
                }
            }
            return ajax;
        } 
        #endregion

        #region 修改
        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="pro"></param>
        /// <returns></returns>
        public MODEL.AjaxResult modifyPro(MODEL.Pro pro)
        {
            AjaxResult ajax = new AjaxResult();
            if (string.IsNullOrEmpty(pro.Id) || string.IsNullOrEmpty(pro.Title))
            {
                ajax.Result = false;
                ajax.Msg = "新增索引时候Id和标题不允许为空！";
            }
            else
            {
                try
                {
                    IndexManager.Instance.Mod(pro);
                    ajax.Msg = "修改成功";
                    ajax.Result = true;
                }
                catch (Exception)
                {
                    ajax.Msg = "修改失败";
                    ajax.Result = false;
                }
            }
            return ajax;
        } 
        #endregion

        #region 删除
        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public MODEL.AjaxResult delPro(string id)
        {
            AjaxResult ajax = new AjaxResult();
            try
            {
                IndexManager.Instance.Del(id);
                ajax.Msg = "删除成功";
                ajax.Result = true;
            }
            catch (Exception)
            {
                ajax.Msg = "删除失败";
                ajax.Result = false;
            }
            return ajax;
        } 
        #endregion
    }
}