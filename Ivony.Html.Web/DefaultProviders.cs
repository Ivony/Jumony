using Ivony.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using Ivony.Fluent;

namespace Ivony.Html.Web
{


  /// <summary>
  /// 提供默认的 HTML 提供程序
  /// </summary>
  public class DefaultProviders
  {


    /// <summary>
    /// 初始化 DefaultProviders 对象
    /// </summary>
    internal DefaultProviders()
    {
      StaticFileContentProvider = new StaticFileContentProvider();
      WebFormPageContentProvider = new WebFormPageContentProvider();
    }


    /// <summary>
    /// 静态 HTML 文件加载提供程序实例
    /// </summary>
    public StaticFileContentProvider StaticFileContentProvider
    {
      get;
      private set;
    }


    /// <summary>
    /// ASP.NET WebForm 文件加载提供程序实例
    /// </summary>
    public WebFormPageContentProvider WebFormPageContentProvider
    {
      get;
      private set;
    }


    /// <summary>
    /// 获取默认的内容服务
    /// </summary>
    /// <param name="virtualPath">要加载内容的虚拟路径</param>
    /// <returns>默认的内容服务列表</returns>
    public IEnumerable<IHtmlContentProvider> GetContentServices( string virtualPath )
    {
      if ( VirtualPathUtility.GetExtension( virtualPath ).EqualsIgnoreCase( ".htm" ) )
        return new IHtmlContentProvider[] { StaticFileContentProvider };

      if ( VirtualPathUtility.GetExtension( virtualPath ).EqualsIgnoreCase( ".html" ) )
        return new IHtmlContentProvider[] { StaticFileContentProvider };

      if ( VirtualPathUtility.GetExtension( virtualPath ).EqualsIgnoreCase( ".aspx" ) )
        return new IHtmlContentProvider[] { WebFormPageContentProvider };

      return Enumerable.Empty<IHtmlContentProvider>();
    }


    /// <summary>
    /// 获取默认的 HTML 解析器实例
    /// </summary>
    /// <param name="virtualPath">要解析的 HTML 文档虚拟路径</param>
    /// <returns>默认的 HTML 解析器</returns>
    public IHtmlParser GetParser( string virtualPath )
    {
      return new WebParser();
    }
  }
}
