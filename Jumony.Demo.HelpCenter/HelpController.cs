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



    public ActionResult Entry( string name = "index" )
    {

      var entry = GetEntries().FirstOrDefault( e => e.Name == name );
      if ( entry == null )
        return HttpNotFound();

      return View( entry.VirtualPath, "frame" );
    }


    private const string navigationCacheKey = "Navigation";
    private const string helpEntriesVirtualPath = "~/HelpEntries";

    public ActionResult Navigation()
    {
      return PartialView( "Navigation", GetEntries() );
    }

    private HelpEntry[] GetEntries()
    {
      var entries = Cache.Get( navigationCacheKey ) as HelpEntry[];

      if ( entries == null )
        Cache.Insert( navigationCacheKey, entries = InitializeEntries(), VirtualPathProvider.GetCacheDependency( helpEntriesVirtualPath, null, DateTime.UtcNow ), DateTime.UtcNow.AddMinutes( 5 ), Cache.NoSlidingExpiration );
      return entries;
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
      var virtualPath = VirtualPathUtility.ToAppRelative( file.VirtualPath );
      var name = Path.GetFileNameWithoutExtension( file.Name );

      if ( !VirtualPathUtility.GetExtension( virtualPath ).EqualsIgnoreCase( ".html" ) )
        return null;

      var document = HtmlProviders.LoadDocument( virtualPath );

      return new HelpEntry()
      {
        Title = document.FindFirst( "title" ).InnerHtml(),
        Name = name,
        VirtualPath = virtualPath,
        Category = document.FindFirstOrDefault( "meta[name=category]" ).IfNull( null, element => element.Attribute( "content" ).Value() ),
        SubTitles = document.Find( "h3[id]" ).ToDictionary( element => element.Attribute( "id" ).Value(), element => element.InnerText() )
      };



    }

  }
}