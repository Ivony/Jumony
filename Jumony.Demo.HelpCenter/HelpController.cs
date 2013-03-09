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

namespace Jumony.Demo.HelpCenter
{
  /// <summary>
  /// HelpController 的摘要说明
  /// </summary>
  public class HelpController : Controller
  {



    public ActionResult Show( string name = "index" )
    {
      return View( name );
    }


    private const string navigationCacheKey = "Navigation";
    private const string helpEntriesVirtualPath = "~/Help";

    public ActionResult Navigation()
    {

      var entries = Cache.Get( navigationCacheKey ) as HelpEntry[];

      if ( entries == null )
        Cache.Insert( navigationCacheKey, entries = InitializeEntries(), VirtualPathProvider.GetCacheDependency( helpEntriesVirtualPath, null, DateTime.UtcNow ) );

      return View( "Navigation", entries );


    }

    protected VirtualPathProvider VirtualPathProvider
    {
      get { return HostingEnvironment.VirtualPathProvider; }
    }

    protected Cache Cache
    {
      get { return HttpRuntime.Cache; }
    }

    private HelpEntry[] InitializeEntries()
    {
      var directory = VirtualPathProvider.GetDirectory( helpEntriesVirtualPath );
      var entries = directory.EnumerateFiles().Select( file => GetEntry( file ) ).NotNull();

      return entries.OrderBy( e => e.Category ).ToArray();
    }

    private HelpEntry GetEntry( VirtualFile file )
    {

      if ( !VirtualPathUtility.GetExtension( file.VirtualPath ).EqualsIgnoreCase( ".html" ) )
        return null;

      var document = HtmlProviders.LoadDocument( file.VirtualPath );

      return new HelpEntry()
      {
        Title = document.FindFirst( "title" ).InnerText(),
        Name = file.Name,
        VirtualPath = file.VirtualPath,
        Category = document.FindFirstOrDefault( "meta[category]" ).IfNull( null, element => element.Attribute( "parent" ).Value() )
      };



    }

  }
}