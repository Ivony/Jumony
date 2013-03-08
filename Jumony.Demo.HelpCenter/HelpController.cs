using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Caching;
using System.Web.Hosting;
using System.Web.Mvc;
using Ivony.Web;

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

    var topics = Cache.Get( navigationCacheKey ) as Topic[];

    if ( topics == null )
      Cache.Insert( navigationCacheKey, topics = InitializeTopics(), VirtualPathProvider.GetCacheDependency( helpEntriesVirtualPath, null, DateTime.UtcNow ) );

    return View( "Navigation", topics );


  }

  protected VirtualPathProvider VirtualPathProvider
  {
    get { return HostingEnvironment.VirtualPathProvider; }
  }

  protected Cache Cache
  {
    get { return HttpRuntime.Cache; }
  }

  private Topic[] InitializeTopics()
  {

    var directory = VirtualPathProvider.GetDirectory( helpEntriesVirtualPath );
    var topics = directory.EnumerateFiles().Select( file => GetEntry( file ) );

  }

  private Entry GetEntry( VirtualFile file )
  {
    throw new NotImplementedException();
  }

  private Topic GetTopic( VirtualFile file )
  {
    throw new NotImplementedException();
  }
}