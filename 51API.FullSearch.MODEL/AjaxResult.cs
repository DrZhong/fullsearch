using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _51API.FullSearch.MODEL
{
    public class AjaxResult
    {
        /// <summary>
        /// 成功与否
        /// </summary>
        public bool Result { get; set; }

        /// <summary>
        /// 响应状态码
        /// </summary>
        public int? StateCode { get; set; }

        /// <summary>
        /// 提示信息
        /// </summary>
        public string Msg { get; set; }

        /// <summary>
        /// 额外的参数
        /// </summary>
        public object Obj { get; set; }
    }
}
