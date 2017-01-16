using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;

namespace MVC.GSWeb.Models
{
    public class BaseContext<TContext> : DbContext where TContext : DbContext
    {
        static BaseContext()
        {
            Database.SetInitializer<TContext>(null);
        }

        //protected BaseContext()
        //    : base("name = AbcDBContext")
        //{ }
    }

    //Stored Procedure results
    public class FunctionsContext : BaseContext<FunctionsContext>
    {

        public FunctionsContext()
        {
            //Static List of Amount Codes
            //this.AmountCodeList = abcDataGetter.AmountCodes.ToList();
        }
    }
}