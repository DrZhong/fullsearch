using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using _51API.FullSearch.MODEL;
using _51API.FullSearch.Cache;
using PanGu.Dict;
using _51API.FullSearch.Attrs;

namespace _51API.FullSearch.Controllers
{
    /// <summary>
    /// 商品列表（get-查询，post-修改，put-添加，delete-删除）string keyWord, bool ResultMsg, string SearchCategory, int PageIndex=1, int PageSize=10
    /// </summary>
    [UserCenter.HTTPBasicAuthorize]
    public class ProListController : ApiController
    {
        private const string KEY = "Nop.stateprovince.all-{0}";
        _51API.FullSearch.Cache.ICacheManager Icache = new _51API.FullSearch.Cache.MemoryCacheManager();
        WordDictionary _WordDict = null;
        
        #region 获取关键词列表
        /// <summary>
        /// 获取关键词列表
        /// </summary>
        /// <param name="kw"></param>
        /// <returns></returns>
        [AllowAnonymous]
        public List<SearchWordResult> Get(string kw)
        {
            _WordDict = Icache.Get(KEY,60*24,() =>
            {
                WordDictionary _WordDictTemp = new WordDictionary();
                _WordDictTemp.Load(_51API.FullSearch.HELP.StaticPath._WordDictPath);
                return  _WordDictTemp;
            });
            List<SearchWordResult> result = null;
            if (string.IsNullOrEmpty(kw))
            {
                result = new List<SearchWordResult>();
            }
            else
            {
                result = _WordDict.Search(kw.Trim());
                result.Sort();
                result = result.Take(10).ToList();
            }
            return result;
        } 
        #endregion
      
        #region 1.0 Get查询（分页）
        /// <summary>
        ///get 查询(分页)
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        public MODEL.ProListResult Get(string keyWord, string Category,int? ShopId=null, bool ResultMsg=true, int PageIndex=1, int PageSize=10)
        {
            
            MODEL.SearchConditionModel model = new SearchConditionModel()
            {
                keyWord = keyWord,
                Category = Category,
                ResultMsg = ResultMsg,
                ShopId=ShopId,
                PageIndex = PageIndex,
                PageSize = PageSize
            };
           return new BLL.ProListBLL().getList(model);
        }
        #endregion

        #region  2.0 post 新增
        /// <summary>
        /// post 新增
        /// </summary>
        /// <param name="value"></param>
        [HttpPost]
        public MODEL.AjaxResult Post([FromBody]MODEL.Pro Pro)
        {
            return new BLL.ProListBLL().addPro(Pro);
        } 
        #endregion

        #region 3.0 PUT修改
        // PUT
        public MODEL.AjaxResult Put(string id, [FromBody]MODEL.Pro Pro)
        {
            return new BLL.ProListBLL().modifyPro(Pro);
        } 
        #endregion

        #region 4.0 DELETE删除
        /// <summary>
        /// 4.0 DELETE删除 
        /// </summary>
        /// <param name="id"></param>
        public MODEL.AjaxResult Delete(string id)
        {
            return new BLL.ProListBLL().delPro(id);
        } 
        #endregion
    }
}