using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace _51API.FullSearch.IMODEL
{
    /// <summary>
    /// 搜索条件模型
    /// </summary>
    public interface ISearchConditionModel
    {
        /// <summary>
        /// 搜索关键字
        /// </summary>
        string keyWord { get; set; }

        /// <summary>
        /// 在哪个类别下搜索，为空的话就是所有类别的全文搜索
        /// </summary>
        string Category{ get; set; }

        /// <summary>
        /// 商品Id（可选，为空为搜索所有的店铺）
        /// </summary>
        int? ShopId { get; set; }

        /// <summary>
        /// 是否返回搜索的信息，为空时返回搜索的所有信息
        /// </summary>
        bool ResultMsg { get; set; }

        /// <summary>
        /// 页码
        /// </summary>
        int PageIndex { get; set; }

        /// <summary>
        /// 页容量
        /// </summary>
        int PageSize { get; set; }

    }
}