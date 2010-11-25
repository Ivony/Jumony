using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ivony.Html.Parser;

namespace Ivony.Html.Web
{



  public static class HtmlProviders
  {

    static HtmlProviders()
    {
      ParserProviders = new SynchronizedCollection<IHtmlParserProvider>( _parserProvidersSync );
      HtmlContentProviders = new SynchronizedCollection<IHtmlContentProvider>( _contentProvidersSync );
    }


    private static readonly object _parserProvidersSync = new object();

    public static ICollection<IHtmlParserProvider> ParserProviders
    {
      get;
      private set;
    }

    private static readonly object _contentProvidersSync = new object();

    public static ICollection<IHtmlContentProvider> HtmlContentProviders
    {
      get;
      private set;
    }


    /// <summary>
    /// 获取用于分析 HTML 文档的 Parser
    /// </summary>
    /// <param name="virtualPath">请求的虚拟路径</param>
    /// <param name="htmlContent">HTML 文档内容</param>
    /// <returns>分析后的 HTML 文档</returns>
    public static IHtmlParser GetParser( string virtualPath, string htmlContent )
    {
      lock ( _parserProvidersSync )
      {
        foreach ( var provider in ParserProviders )
        {
          var parser = provider.GetParser( virtualPath, htmlContent );

          if ( parser != null )
            return parser;
        }
      }

      return new JumonyHtmlParser();
    }

  
    /// <summary>
    /// 加载 HTML 文档内容
    /// </summary>
    /// <param name="virtualPath">请求的虚拟路径</param>
    /// <returns>HTML 文档内容</returns>
    public static string LoadHtmlContent( string virtualPath )
    {
      lock ( _contentProvidersSync )
      {
        foreach ( var provider in HtmlContentProviders )
        {
          var content = provider.LoadContent( virtualPath );

          if ( content != null )
            return content;
        }
      }

      return null;
    }

    /// <summary>
    /// 加载 HTML 文档
    /// </summary>
    /// <param name="virtualPath">请求的虚拟路径</param>
    /// <returns>HTML 文档对象</returns>
    public static IHtmlDocument LoadDocument( string virtualPath )
    {
      var content = LoadHtmlContent( virtualPath );
      if ( content == null )
        return null;

      var parser = GetParser( virtualPath, content );
      
      return parser.Parse( content );
    }



  }

  public interface IHtmlContentProvider
  {

    string LoadContent( string virtualPath );

  }


  public interface IHtmlParserProvider
  {

    IHtmlParser GetParser( string virtualPath, string htmlContent );

  }


}
