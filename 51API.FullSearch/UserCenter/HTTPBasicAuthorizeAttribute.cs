using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Http;
using System.Xml.Linq;
using _51API.FullSearch.Cache;

namespace _51API.FullSearch.UserCenter
{
    /// <summary>
    /// 自定义权限验证
    /// </summary>
    public class HTTPBasicAuthorizeAttribute : System.Web.Http.AuthorizeAttribute
    {
        private static log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private const string KEY = "Nsdjkfgfjsdgfjs";
        _51API.FullSearch.Cache.ICacheManager Icache = new _51API.FullSearch.Cache.MemoryCacheManager();
        #region 初始化AppId字典(缓存机制)
        Dictionary<string, string> dict = null;
        public HTTPBasicAuthorizeAttribute()
        {
            dict = Icache.Get(KEY, 60 * 24, () =>
            {
                var  dictTemp = new Dictionary<string, string>();
                XElement root = XElement.Load(_51API.FullSearch.HELP.StaticPath._AppLoaction);
                IEnumerable<XElement> query =                  //query:查询的结构集
                          from ele in root.Elements("App")  //ele 表示通过rootE.Elements("book"）查询后的一项数据
                          select ele;
                foreach (var item in query)
                {
                    dictTemp.Add(item.Attribute("Id").Value, item.Attribute("Key").Value);
                }
                return dictTemp;
            });
        }
        #endregion

        #region AppId和AppKey验证
        /// <summary>
        /// AppId和AppKey验证
        /// </summary>
        /// <param name="actionContext"></param>
        public override void OnAuthorization(System.Web.Http.Controllers.HttpActionContext actionContext)
        {
            var attr = actionContext.ActionDescriptor.GetCustomAttributes<Attribute>(true);
            //1.0第一步胖短是否为匿名访问的Action
            if (attr.Any(x => x is AllowAnonymousAttribute))
            {
              
                base.OnAuthorization(actionContext);
            }
            else
            {
                if (actionContext.Request.Headers.Authorization != null)
                {
                    string AppId = actionContext.Request.Headers.Authorization.Scheme;
                    string AppKey = actionContext.Request.Headers.Authorization.Parameter;
                    //验证
                    if (dict.Keys.Contains(AppId) && dict[AppId].Equals(AppKey))
                    {
                        IsAuthorized(actionContext);
                    }
                    else
                    { 
                        log.Error("请求失败，身份验证失败");
                        HandleUnauthorizedRequest(actionContext);
                    }
                }
                else
                {
                    log.Error("请求失败，身份验证失败");
                    HandleUnauthorizedRequest(actionContext);
                }
            }
        } 
        #endregion

        #region 验证不通过
        /// <summary>
        /// 验证不通过
        /// </summary>
        /// <param name="actionContext"></param>
        protected override void HandleUnauthorizedRequest(System.Web.Http.Controllers.HttpActionContext actionContext)
        {
            try
            {
                var challengeMessage = new System.Net.Http.HttpResponseMessage(System.Net.HttpStatusCode.Unauthorized);
                challengeMessage.Headers.Add("WWW-Authenticate", "Basic");
                throw new System.Web.Http.HttpResponseException(challengeMessage);
            }
            catch (Exception ex)
            {
               
                throw;
            }
        } 
        #endregion

    }
}