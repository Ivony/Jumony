using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Hosting;
using System.IO;
using System.Web.Caching;
using System.Collections.ObjectModel;

namespace Ivony.Html.Web
{

  /// <summary>
  /// 定义 HTML 内容提供程序
  /// </summary>
  public interface IHtmlContentProvider
  {

    /// <summary>
    /// 加载 HTML 内容
    /// </summary>
    /// <param name="virtualPath">要加载内容的虚拟路径</param>
    /// <returns>加载的 HTML 内容</returns>
    HtmlContentResult LoadContent( string virtualPath );

  }


  /// <summary>
  /// IHtmlContentProvider 的内容加载结果
  /// </summary>
  public class HtmlContentResult
  {


    /// <summary>
    /// 创建 HtmlContentResult 实例
    /// </summary>
    /// <param name="provider">负责加载的提供程序</param>
    /// <param name="content">加载的内容</param>
    /// <param name="virtualPath">内容的虚拟路径</param>
    public HtmlContentResult( IHtmlContentProvider provider, string content, string virtualPath ) : this( provider, content, virtualPath, null ) { }

    /// <summary>
    /// 创建 HtmlContentResult 实例
    /// </summary>
    /// <param name="provider">负责加载的提供程序</param>
    /// <param name="content">加载的内容</param>
    /// <param name="virtualPath">内容的虚拟路径</param>
    /// <param name="cacheKey">缓存内容所使用的缓存键</param>
    public HtmlContentResult( IHtmlContentProvider provider, string content, string virtualPath, string cacheKey )
    {

      if ( provider == null )
        throw new ArgumentNullException( "provider" );

      if ( content == null )
        throw new ArgumentNullException( "content" );

      if ( virtualPath == null )
        throw new ArgumentNullException( "virtualPath" );

      if ( !VirtualPathUtility.IsAppRelative( virtualPath ) )
        throw HtmlHelper.VirtualPathFormatError( "virtualPath" );

      Provider = provider;
      Content = content;
      VirtualPath = virtualPath;
      CacheKey = cacheKey;
    }


    /// <summary>
    /// HTML 文本内容
    /// </summary>
    public string Content
    {
      get;
      private set;
    }


    /// <summary>
    /// 获取产生此结果的 HtmlContentProvider
    /// </summary>
    public IHtmlContentProvider Provider
    {
      get;
      private set;
    }


    /// <summary>
    /// 获取缓存时使用的索引键
    /// </summary>
    public string CacheKey
    {
      get;
      private set;
    }

    /// <summary>
    /// 加载内容的虚拟路径
    /// </summary>
    public string VirtualPath
    {
      get;
      private set;
    }

  }




}
