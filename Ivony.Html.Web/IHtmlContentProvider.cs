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
    /// <param name="content">加载的内容</param>
    public HtmlContentResult( string content ) : this( content,  null ) { }

    /// <summary>
    /// 创建 HtmlContentResult 实例
    /// </summary>
    /// <param name="content">加载的内容</param>
    /// <param name="cacheKey">缓存内容所使用的缓存键</param>
    public HtmlContentResult(  string content, string cacheKey )
    {

      if ( content == null )
        throw new ArgumentNullException( "content" );

      Content = content;
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
    /// 获取缓存时使用的索引键
    /// </summary>
    public string CacheKey
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
      internal set;
    }


    /// <summary>
    /// 加载内容的虚拟路径
    /// </summary>
    public string VirtualPath
    {
      get;
      internal set;
    }

  }




}
