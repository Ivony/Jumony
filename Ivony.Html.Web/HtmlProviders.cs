using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ivony.Html.Parser;
using System.Web.Hosting;
using System.Web;
using System.IO;

namespace Ivony.Html.Web
{


  public static class HtmlProviders
  {

    static HtmlProviders()
    {
      ParserProviders = new SynchronizedCollection<IHtmlDocumentProvider>( _parserProvidersSync );
      ContentProviders = new SynchronizedCollection<IHtmlContentProvider>( _contentProvidersSync );
      RequestMappers = new SynchronizedCollection<IRequestMapper>( _mappersSync );


      ContentProviders.Add( new StaticFileLoader() );
      ContentProviders.Add( new AspxFileLoader() );

      RequestMappers.Add( new DefaultRequestMapper() );
    }


    private static readonly object _parserProvidersSync = new object();

    public static ICollection<IHtmlDocumentProvider> ParserProviders
    {
      get;
      private set;
    }


    private static readonly object _contentProvidersSync = new object();

    public static ICollection<IHtmlContentProvider> ContentProviders
    {
      get;
      private set;
    }


    private static readonly object _mappersSync = new object();

    public static ICollection<IRequestMapper> RequestMappers
    {
      get;
      private set;
    }


    /// <summary>
    /// 映射请求
    /// </summary>
    /// <param name="request">当前 HTTP 请求信息</param>
    /// <returns>请求映射信息</returns>
    public static MapInfo MapRequest( HttpRequest request )
    {

      lock ( _mappersSync )
      {
        foreach ( var mapper in RequestMappers )
        {
          var result = mapper.MapRequest( request );
          if ( result != null )
          {
            result.Mapper = mapper;
            return result;
          }
        }
      }


      return null;
    }




    /// <summary>
    /// 加载 HTML 文档内容
    /// </summary>
    /// <param name="virtualPath">请求的虚拟路径</param>
    /// <returns>HTML 文档内容</returns>
    public static HtmlContentResult LoadContent( HttpContextBase context, string virtualPath )
    {

      if ( context == null )
        throw new ArgumentNullException( "context" );


      lock ( _contentProvidersSync )
      {
        foreach ( var provider in ContentProviders )
        {
          var content = provider.LoadContent( context, virtualPath );

          if ( content != null )
            return content;
        }
      }


      return null;
    }

    /// <summary>
    /// 加载 HTML 文档
    /// </summary>
    /// <param name="context">当前请求的 HttpContext 对象</param>
    /// <param name="virtualPath">请求的虚拟路径</param>
    /// <returns>HTML 文档对象</returns>
    public static IHtmlDocument LoadDocument( HttpContextBase context, string virtualPath )
    {

      if ( context == null )
        throw new ArgumentNullException( "context" );


      var content = LoadContent( context, virtualPath );
      if ( content == null )
        return null;

      return ParseDocument( context, virtualPath, content );
    }


    /// <summary>
    /// 分析 HTML 文档
    /// </summary>
    /// <param name="context">当前请求的 HttpContext 对象</param>
    /// <param name="virtualPath">请求的虚拟路径</param>
    /// <param name="result">文档加载结果</param>
    /// <returns>HTML 文档对象</returns>
    public static IHtmlDocument ParseDocument( HttpContextBase context, string virtualPath, HtmlContentResult result )
    {
      var document = ParseDocument( context, virtualPath, result.Content );
      return document;
    }


    /// <summary>
    /// 分析 HTML 文档
    /// </summary>
    /// <param name="context">当前请求的 HttpContext 对象</param>
    /// <param name="virtualPath">请求的虚拟路径</param>
    /// <param name="htmlContent">文档内容</param>
    /// <returns>HTML 文档对象</returns>
    public static IHtmlDocument ParseDocument( HttpContextBase context, string virtualPath, string htmlContent )
    {
      if ( context == null )
        throw new ArgumentNullException( "context" );

      lock ( _parserProvidersSync )
      {
        foreach ( var provider in ParserProviders )
        {
          var document = provider.ParseDocument( context, virtualPath, htmlContent );

          if ( document != null )
            return document;
        }
      }

      var parser = new JumonyHtmlParser();

      return parser.Parse( htmlContent );
    }
  }

}
