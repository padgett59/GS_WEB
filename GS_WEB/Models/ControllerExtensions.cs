using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
//using System.Data.Entity;

namespace MVC.GSWeb.Models
{
    public static class ControllerExtensions
    {
        public static GsDataContext GsDataContext(this Controller instance)
        {
            GsDataContext result =
              instance.HttpContext.Items[typeof(GsDataContext).Name] as GsDataContext;

            if (result == null)
            {
                result = new GsDataContext ();
                instance.HttpContext.Items[typeof(GsDataContext).Name] = result;
            }

            if (result == null)
            {
                throw new InvalidOperationException("No GsDataContext in HttpContext.");
            }

            return result;
        }
    }
}