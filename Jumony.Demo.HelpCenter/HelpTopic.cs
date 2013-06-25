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


    private static VirtualPathProvider VirtualPathProvider { get { return HostingEnvironment.VirtualPathProvider; } }

    private static HelpTopic CreateTopic( string virtualPath )
    {


      var directory = VirtualPathProvider.GetDirectory( virtualPath );

      if ( directory != null )
      {
        var childPaths = directory.Children.Cast<VirtualFileBase>().Select( f => VirtualPathUtility.ToAppRelative( f.VirtualPath ) );
        var childs = childPaths.ToDictionary( p => GetTitle( p ), p => p );

        return new HelpTopic()
        {
          VirtualPath = virtualPath,
          Childs = childs
        };

      }
      else if ( VirtualPathProvider.FileExists( virtualPath ) )
      {
      
      }

      else
        return null;

    }

    private static string GetTitle( string virtualPath )
    {

      var directory = VirtualPathProvider.GetDirectory( virtualPath );
      if ( directory != null )
        return GetTitle( VirtualPathUtility.Combine( virtualPath, "index.html" ) );

      var document = HtmlProviders.LoadDocument( virtualPath );
      return document.FindFirst( "title" ).InnerHtml();
    }


    public string VirtualPath { get; private set }

    public Dictionary<string, string> Childs { get; private set }


  }
}