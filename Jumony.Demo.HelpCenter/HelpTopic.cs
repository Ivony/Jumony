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
using System.Web.Caching;

namespace Jumony.Demo.HelpCenter
{

  /// <summary>
  /// 代表一个帮助主题
  /// </summary>
  public abstract class HelpTopic
  {

    private static string cachePrefix = "HelpTopicCache_";
    private static KeyedCache<string, HelpTopic> _cache = new KeyedCache<string, HelpTopic>();





    public static HelpTopic GetTopic( string virtualPath )
    {
      var cacheKey = cachePrefix + virtualPath;

      var topic = HttpRuntime.Cache.Get( cachePrefix + virtualPath ) as HelpTopic;
      if ( topic == null )
      {
        topic = CreateTopic( virtualPath );
        HttpRuntime.Cache.Insert( cacheKey, topic, new CacheDependency( HostingEnvironment.MapPath( topic.DocumentPath ) ) );
      }

      return topic;
    }


    private const string helpEntriesVirtualPath = "~/HelpEntries/";

    private static VirtualPathProvider VirtualPathProvider { get { return HostingEnvironment.VirtualPathProvider; } }



    private string _virtualPath;

    private HelpTopic( string virtualPath )
    {
      _virtualPath = virtualPath;
    }


    private class HelpCategory : HelpTopic
    {
      public HelpCategory( string virtualPath )
        : base( virtualPath )
      {
        if ( virtualPath == null )
          throw new ArgumentNullException( "virtualPath" );

        if ( !VirtualPathProvider.DirectoryExists( virtualPath ) )
          throw new ArgumentException( "虚拟路径不是一个目录", "virtualPath" );
      }


      public override bool IsDirectory { get { return true; } }

      public override HelpTopic[] Childs
      {
        get
        {
          var directory = VirtualPathProvider.GetDirectory( VirtualPath );
          return directory.Children.OfType<VirtualFile>()
            .Select( f => VirtualPathUtility.ToAppRelative( f.VirtualPath ) )
            .Where( p => VirtualPathUtility.GetExtension( p ) == ".html" )
            .Where( p => VirtualPathUtility.GetFileName( p ) != "index.html" )
            .Union( directory.Children.OfType<VirtualDirectory>().Select( d => VirtualPathUtility.ToAppRelative( d.VirtualPath ) ).Where( d => VirtualPathProvider.FileExists( VirtualPathUtility.Combine( d, "index.html" ) ) ) )
            .Select( p => GetTopic( VirtualPathUtility.MakeRelative( helpEntriesVirtualPath, p ) ) ).ToArray();
        }
      }

    }



    private class HelpEntry : HelpTopic
    {
      public HelpEntry( string virtualPath )
        : base( virtualPath )
      {
        if ( virtualPath == null )
          throw new ArgumentNullException( "virtualPath" );

        if ( !VirtualPathProvider.FileExists( virtualPath ) )
          throw new ArgumentException( "虚拟路径不是一个文件", "virtualPath" );
      }

      public override bool IsDirectory { get { return false; } }

      public override HelpTopic[] Childs { get { return new HelpTopic[0]; } }

    }


    private static HelpTopic CreateTopic( string virtualPath )
    {

      if ( virtualPath == null )
        throw new ArgumentNullException( "virtualPath" );

      virtualPath = VirtualPathUtility.Combine( helpEntriesVirtualPath, virtualPath );


      if ( VirtualPathProvider.DirectoryExists( virtualPath ) )
        return new HelpCategory( virtualPath );

      else if ( VirtualPathProvider.FileExists( virtualPath ) )
        return new HelpEntry( virtualPath );

      else
        return null;

    }





    public string VirtualPath
    {
      get { return IsDirectory ? VirtualPathUtility.AppendTrailingSlash( _virtualPath ) : VirtualPathUtility.RemoveTrailingSlash( _virtualPath ); }
    }

    public string DocumentPath
    {
      get { return IsDirectory ? VirtualPathUtility.Combine( VirtualPath, "index.html" ) : VirtualPath; }
    }

    public string HelpPath
    {
      get { return VirtualPathUtility.MakeRelative( helpEntriesVirtualPath, VirtualPath ); }
    }

    public abstract HelpTopic[] Childs { get; }

    public IHtmlDocument Document
    {
      get { return HtmlServices.LoadDocument( DocumentPath ); }
    }


    public HelpTopic Parent
    {
      get
      {



        if ( VirtualPath.EqualsIgnoreCase( helpEntriesVirtualPath ) )
          return null;

        var virtualPath = VirtualPathUtility.GetDirectory( VirtualPathUtility.RemoveTrailingSlash( VirtualPath ) );
        if ( virtualPath.EqualsIgnoreCase( helpEntriesVirtualPath ) )
          return GetTopic( "." );

        return GetTopic( VirtualPathUtility.MakeRelative( helpEntriesVirtualPath, virtualPath ) );
      }
    }


    public abstract bool IsDirectory { get; }

    public string Title
    {
      get { return Document.FindFirst( "head" ).FindFirst( "title" ).InnerHtml(); }
    }
  }
}