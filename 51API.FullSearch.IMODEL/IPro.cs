using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _51API.FullSearch.IMODEL
{
    /// <summary>
    /// 新增产品或者新闻的实体
    /// </summary>
    public interface IPro
    {
        /// <summary>
        /// id信息，建议是一个GUID(不允许为空)
        /// </summary>
        string Id { get; set; }

        /// <summary>
        /// 商品或者新闻的类别，方便在指定范围类搜索
        /// </summary>
        string Category { get; set; }

        /// <summary>
        /// 店铺Id
        /// </summary>
        int? ShopId { get; set; }

        /// <summary>
        /// 商品色名称，或者新闻的标题
        /// </summary>
        
        string Title { get; set; }

        /// <summary>
        /// 商品的描述，或者新闻的正文
        /// </summary>
        string Content { get; set; }

        /// <summary>
        /// 商品或者新闻的Url
        /// </summary>
        string Url { get; set; }
    }
}
