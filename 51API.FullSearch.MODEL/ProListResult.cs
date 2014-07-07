using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace _51API.FullSearch.MODEL
{
    public class ProListResult
    {
        /// <summary>
        /// 总个数
        /// </summary>
        public int Total { get; set; }

        /// <summary>
        /// 返回查找到的Id集合,建议使用guid
        /// </summary>
        public List<string> IdList { get; set; }

        /// <summary>
        ///返回内容集合
        /// </summary>
        public List<Pro> ContentList { get; set; }
    }
}