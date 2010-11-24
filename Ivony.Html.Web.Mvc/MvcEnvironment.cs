using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

[assembly: PreApplicationStartMethod( typeof( Ivony.Html.Web.Mvc.MvcEnvironment ), "Initialize" )]

namespace Ivony.Html.Web.Mvc
{

  public class MvcEnvironment
  {

    public static void Initialize()
    {
      ViewEngines.Engines.Add( new JumonyViewEngine() );
    }

  }
}
