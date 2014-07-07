using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace _51API.FullSearch.MODEL
{
    /// <summary>
    /// 搜索条件模型
    /// </summary>
    public class SearchConditionModel:IMODEL.ISearchConditionModel
    {

        /// <summary>
        /// 搜索关键字
        /// </summary>
        public string keyWord { get; set; }

        /// <summary>
        /// 在哪个类别下搜索，为空的话就是所有类别的全文搜索
        /// </summary>
        public string Category { get; set; }

        /// <summary>
        /// 是否返回搜索的信息，为空时返回搜索的所有信息
        /// </summary>
        public bool ResultMsg { get; set; }

        /// <summary>
        /// 页码
        /// </summary>
        public int PageIndex { get; set; }

        /// <summary>
        /// 页容量
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// 店铺Id
        /// </summary>
        public int? ShopId{ get;set;}
    }
}