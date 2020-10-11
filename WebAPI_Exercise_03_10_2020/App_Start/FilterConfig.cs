using System.Web;
using System.Web.Mvc;

namespace WebAPI_Exercise_03_10_2020
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
