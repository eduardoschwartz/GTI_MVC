﻿using System.Web;
using System.Web.Mvc;

namespace GTI_Mvc {
    public static class FilterConfig {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters) {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
