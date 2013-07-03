using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using Ivony.Fluent;
using Ivony;
using Ivony.Html;
using Ivony.Html.Web;
using System.Web.Hosting;

namespace Jumony.Demo.HelpCenter
{

  /// <summary>
  /// 代表一个帮助主题
  /// </summary>
  public class HelpTopic
  {


    private static KeyedCache<string, HelpTopic> _cache = new KeyedCache<string, HelpTopic>();

    public static HelpTopic GetTopic( string virtualPath )
    {
      if ( !VirtualPathUtility.IsAppRelative( virtualPath ) )
        throw new ArgumentException( "虚拟路径必须是应用程序根相对路径", "virtualPath" );

      return _cache.FetchOrCreateItem( virtualPath, () => CreateTopic( virtualPath ) );
    }


    private const string helpEntriesVirtualPath = "~/HelpEntries";


    private static VirtualPathProvider VirtualPathProvider { get { return HostingEnvironment.VirtualPathProvider; } }

    private static HelpTopic CreateTopic( string virtualPath )
    {

      virtualPath = VirtualPathUtility.Combine( helpEntriesVirtualPath, virtualPath );

      var directory = VirtualPathProvider.GetDirectory( virtualPath );

      if ( directory != null )
      {
        var childPaths = directory.Children.Cast<VirtualFileBase>().Select( f => VirtualPathUtility.ToAppRelative( f.VirtualPath ) );
        var childs = childPaths.ToDictionary( p => GetTopic( p ).Title, p => p );


        var document = HtmlProviders.LoadDocument( VirtualPathUtility.Combine( virtualPath, "index.html" ) );

        return new HelpTopic()
        {
          VirtualPath = virtualPath,
          IsDirectory = true,
          Document = document,
          Childs = childs
        };

      }
      else if ( VirtualPathProvider.FileExists( virtualPath ) )
      {
        var document = HtmlProviders.LoadDocument( virtualPath );

        return new HelpTopic()
        {
          VirtualPath = virtualPath,
          IsDirectory = false,
          Document = document,
          Childs = new Dictionary<string, string>(),
          Title = document.FindFirst( "title" ).InnerHtml()

        };
      }

      else
        return null;

    }


    public string VirtualPath { get; private set; }

    public Dictionary<string, string> Childs { get; private set; }

    public IHtmlDocument Document { get; private set; }

    public bool IsDirectory { get; private set; }

    public string Title { get; private set; }
  }
}