using System.Web;
using System.Web.Mvc;

namespace _51API.FullSearch
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
           // filters.Add(new Attrs.CheckAccessAttribute());
        }
    }
}
