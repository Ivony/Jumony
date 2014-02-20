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

    private const string helpEntriesVirtualPath = "~/HelpEntries/";




    public ActionResult Entry( string path = "." )
    {

      var topic = HelpTopic.GetTopic( path );
      return View( topic.DocumentPath, "frame" );
    }


    private const string navigationCacheKey = "Navigation";

    public ActionResult Navigation( string path = "." )
    {
      var parents = GetParents( path ).Select( GetTopic );


      return PartialView( "Navigation", HelpTopic.GetTopic( path ) );
    }


    private static readonly string _topicCachePrefix = "Help_Topic_";

    private Topic GetTopic( string path )
    {

      var cacheKey = _topicCachePrefix + path;

      var topic = HttpRuntime.Cache.Get( cacheKey ) as Topic;

      if ( topic == null )
      {
        CacheDependency dependency;
        IHtmlDocument document;

        if ( path.EndsWith( "/" ) )
          document = HtmlServices.LoadDocument( path + "index.html", out dependency );
        else
          document = HtmlServices.LoadDocument( path, out dependency );

        var title = document.FindFirst( "title" ).InnerText();
        topic = new Topic
        {
          VirtualPath = path,
          Title = title
        };

        if ( dependency != null )
          HttpRuntime.Cache.Insert( cacheKey, topic, dependency );
      }

      return topic;
    }

    private IEnumerable<string> GetParents( string path )
    {

      if ( path.EqualsIgnoreCase( helpEntriesVirtualPath ) )
        yield break;

      var parent = VirtualPathHelper.GetParentDirectory( path );
      yield return parent;

      foreach ( var _path in GetParents( parent ) )
        yield return _path;
    }
  }

  public class Topic
  {
    public string VirtualPath { get; set; }

    public string Title { get; set; }
  }
}