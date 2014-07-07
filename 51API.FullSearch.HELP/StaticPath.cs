using System;
using System.Web;
using System.Configuration;

namespace _51API.FullSearch.HELP
{

    /// <summary>
    /// 静态地址
    /// </summary>
    public static  class StaticPath
    {
        /// <summary>
        /// lucene索引存储地址
        /// </summary>
        public static string IndexManagerPath = HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["IndexData"]);


        /// <summary>
        /// 盘古词库地址
        /// </summary>
        public static string _WordDictPath = HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["Dict"]);


        /// <summary>
        /// AppId的文件储存位置
        /// </summary>
        public static string _AppLoaction = HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["AppLocation"]);
    }
}
