using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _51API.FullSearch.MODEL
{
    /// <summary>
    /// 插入到luencen的商品、新闻信息
    /// </summary>
    public class Pro:IMODEL.IPro
    {
        /// <summary>
        /// id信息，建议是一个GUID
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 商品或者新闻的类别，方便在指定范围类搜索
        /// </summary>
        public string Category { get; set; }

        /// <summary>
        /// 店铺Id，不要重复
        /// </summary>
        public int? ShopId { get; set; }

        /// <summary>
        /// 商品色名称，或者新闻的标题
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 商品的描述，或者新闻的正文
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// 商品或者新闻的Url
        /// </summary>
        public string Url { get; set; }
    }
}
