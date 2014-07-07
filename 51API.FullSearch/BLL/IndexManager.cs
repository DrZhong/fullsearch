using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Lucene.Net.Index;
using Lucene.Net.Analysis;
using Lucene.Net.Analysis.PanGu;
using Lucene.Net.Documents;
using Lucene.Net.Search;
using Lucene.Net.Store;
using System.Threading;
using System.IO;
using System.Management;
using _51API.FullSearch.HELP;

namespace _51API.FullSearch.BLL
{
    public class IndexManager
    {
        #region 程序初始化
        private static log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public static readonly IndexManager Instance = new IndexManager();
        public static readonly string indexPath = null;
        private IndexManager()
        {
           
        }
        static IndexManager()
        {
            indexPath = _51API.FullSearch.HELP.StaticPath.IndexManagerPath;
        }
        #endregion

        #region 请求队列 解决索引目录同时操作的并发问题
        //请求队列 解决索引目录同时操作的并发问题
        private Queue<BookViewMode> bookQueue = new Queue<BookViewMode>(); 
        #endregion

        #region 新增Books表信息时 添加邢增索引请求至队列
        /// <summary>
        /// 新增Books表信息时 添加邢增索引请求至队列
        /// </summary>
        /// <param name="books"></param>
        public void Add(MODEL.Pro Pro)
        {
            BookViewMode bvm = new BookViewMode();
            bvm.Id = Pro.Id;
            bvm.ShopId = Pro.ShopId == null ? "" : Pro.ShopId.ToString();
            bvm.Title = Pro.Title;
            bvm.IT = IndexType.Insert;
            bvm.Content =string.IsNullOrEmpty(Pro.Content)?"":Pro.Content;
            bvm.Category = string.IsNullOrEmpty(Pro.Category) ? "" : Pro.Category;
            bvm.Url = string.IsNullOrEmpty(Pro.Url)? "" : Pro.Url;
            string content = HELP.CommonHelp.GetPanguWords(Pro.Title+Pro.Content);// Common.GetPanguWords(Article.Title + Article.Content);
            bvm.PinYin = DXHanZiToPinYin.Convert(content, content.Length);
            bookQueue.Enqueue(bvm);
        } 
        #endregion

        #region 删除Books表信息时 添加删除索引请求至队列
        /// <summary>
        /// 删除Books表信息时 添加删除索引请求至队列
        /// </summary>
        /// <param name="bid"></param>
        public void Del(string bid)
        {
            BookViewMode bvm = new BookViewMode();
            bvm.Id = bid;
            bvm.IT = IndexType.Delete;
            bookQueue.Enqueue(bvm);
        } 
        #endregion

        #region 修改Books表信息时 添加修改索引(实质上是先删除原有索引 再新增修改后索引)请求至队列
        /// <summary>
        /// 修改Books表信息时 添加修改索引(实质上是先删除原有索引 再新增修改后索引)请求至队列
        /// </summary>
        /// <param name="books"></param>
        public void Mod(MODEL.Pro Pro)
        {
            BookViewMode bvm = new BookViewMode();
            bvm.Id = Pro.Id;
            bvm.ShopId = Pro.ShopId == null ? "" : Pro.ShopId.ToString();
            bvm.Title = Pro.Title;
            bvm.IT = IndexType.Modify;
            bvm.Content = string.IsNullOrEmpty(Pro.Content) ? "" : Pro.Content;
            bvm.Category = string.IsNullOrEmpty(Pro.Category) ? "" : Pro.Category;
            bvm.Url = string.IsNullOrEmpty(Pro.Url) ? "" : Pro.Url;
            string content = HELP.CommonHelp.GetPanguWords(Pro.Title + Pro.Content);// Common.GetPanguWords(Article.Title + Article.Content);
            bvm.PinYin = DXHanZiToPinYin.Convert(content, content.Length);
            bookQueue.Enqueue(bvm);
        } 
        #endregion

        #region 应用程序初始化启动这个线程
        /// <summary>
        /// 应用程序初始化启动这个线程
        /// </summary>
        public void StartNewThread()
        {
            ThreadPool.QueueUserWorkItem(new WaitCallback(QueueToIndex));
        } 
        #endregion

        #region 定义一个线程 将队列中的数据取出来 插入索引库中
        /// <summary>
        /// 定义一个线程 将队列中的数据取出来 插入索引库中
        /// </summary>
        /// <param name="para"></param>
        private void QueueToIndex(object para)
        {
            while (true)
            {
                if (bookQueue.Count > 0)
                {
                    CRUDIndex();
                }
                else
                {
                    Thread.Sleep(3000);
                }
            }
        } 
        #endregion

        #region 更新索引库操作
        /// <summary>
        /// 更新索引库操作
        /// </summary>
        private void CRUDIndex()
        {
            FSDirectory directory = FSDirectory.Open(new DirectoryInfo(indexPath), new NativeFSLockFactory());
            bool isExist = IndexReader.IndexExists(directory);
            if (isExist)
            {
                if (IndexWriter.IsLocked(directory))
                {
                    IndexWriter.Unlock(directory);
                }
            }
            PanGuAnalyzer pangu = new PanGuAnalyzer();
            
            IndexWriter writer = new IndexWriter(directory, new PanGuAnalyzer() , !isExist, IndexWriter.MaxFieldLength.UNLIMITED);
            while (bookQueue.Count > 0)
            {
                Document document = new Document();
                BookViewMode book = bookQueue.Dequeue();
                if (book.IT == IndexType.Insert)
                {
                    try
                    {
                        document.Add(new Field("Id", book.Id, Field.Store.YES, Field.Index.NOT_ANALYZED));

                        document.Add(new Field("Category", book.Category.ToString(), Field.Store.YES, Field.Index.ANALYZED,
                                               Field.TermVector.WITH_POSITIONS_OFFSETS));
                        document.Add(new Field("ShopId", book.ShopId, Field.Store.YES, Field.Index.ANALYZED,
                                               Field.TermVector.WITH_POSITIONS_OFFSETS));
                        document.Add(new Field("Url", book.Url, Field.Store.YES, Field.Index.NOT_ANALYZED));
                        document.Add(new Field("Title", book.Title, Field.Store.YES, Field.Index.ANALYZED,
                                               Field.TermVector.WITH_POSITIONS_OFFSETS));
                        document.Add(new Field("Content", book.Content, Field.Store.YES, Field.Index.ANALYZED,
                                               Field.TermVector.WITH_POSITIONS_OFFSETS));
                        document.Add(new Field("PinYin", book.PinYin, Field.Store.YES, Field.Index.ANALYZED,
                           Field.TermVector.WITH_POSITIONS_OFFSETS));
                        writer.AddDocument(document);
                    }
                    catch (Exception ex)
                    {
                        log.Error(ex.ToString());
                        throw;
                    }
                }
                else if (book.IT == IndexType.Delete)
                {
                    writer.DeleteDocuments(new Term("Id", book.Id));
                }
                else if (book.IT == IndexType.Modify)
                {
                    //先删除 再新增

                    try
                    {
                        writer.DeleteDocuments(new Term("Id", book.Id.ToString()));

                        document.Add(new Field("Id", book.Id, Field.Store.YES, Field.Index.NOT_ANALYZED));

                        document.Add(new Field("Category", book.Category, Field.Store.YES, Field.Index.ANALYZED,
                                               Field.TermVector.WITH_POSITIONS_OFFSETS));
                        document.Add(new Field("ShopId", book.ShopId, Field.Store.YES, Field.Index.ANALYZED,
                                              Field.TermVector.WITH_POSITIONS_OFFSETS));
                        document.Add(new Field("Url", book.Url, Field.Store.YES, Field.Index.NOT_ANALYZED));

                        document.Add(new Field("Title", book.Title, Field.Store.YES, Field.Index.ANALYZED,
                                               Field.TermVector.WITH_POSITIONS_OFFSETS));
                        document.Add(new Field("Content", book.Content, Field.Store.YES, Field.Index.ANALYZED,
                                               Field.TermVector.WITH_POSITIONS_OFFSETS));
                        document.Add(new Field("PinYin", book.PinYin, Field.Store.YES, Field.Index.ANALYZED,
                           Field.TermVector.WITH_POSITIONS_OFFSETS));
                        writer.AddDocument(document);
                    }
                    catch (Exception ex)
                    {
                        log.Error(ex.ToString());
                        throw;
                    }
                }
            }
            writer.Close();
            directory.Close();
        } 
        #endregion
    }

    public class BookViewMode
    {
        public string Id{ get;set;}
        public string Category { get; set; }
        public string ShopId { get; set; }
        public string Title{get;set;}
        public string Content{get;set;}
        public IndexType IT {get; set;}
        public string PinYin { get; set; }
        public string  Url { get; set; }

        /// <summary>
        /// 随你行索引存储的位置
        /// </summary>
        public string path { get; set; }
    }
    //操作类型枚举
    public enum IndexType
    {
        Insert,
        Modify,
        Delete
    }
}