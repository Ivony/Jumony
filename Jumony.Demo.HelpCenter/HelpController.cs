using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Caching;
using System.Web.Hosting;
using System.Web.Mvc;
using Ivony.Web;
using Ivony.Fluent;
using Ivony.Html.Web;
using Ivony.Html;
using Ivony.Html.ExpandedAPI;
using System.IO;

namespace Jumony.Demo.HelpCenter
{
  /// <summary>
  /// HelpController 的摘要说明
  /// </summary>
  public class HelpController : Controller
  {



    public ActionResult Entry( string virtualPath = "." )
    {

      var topic = HelpTopic.GetTopic( virtualPath );
      return View( topic.Document );
    }


    private const string navigationCacheKey = "Navigation";

    public ActionResult Navigation( string virtualPath )
    {
      return PartialView( "Navigation", HelpTopic.GetTopic( virtualPath ) );
    }

  }
}