using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using Ivony.Html.Parser;

namespace Ivony.Html.Web
{

  /// <summary>
  /// HTML 解析器提供程序
  /// </summary>
  public interface IHtmlParserProvider
  {

    /// <summary>
    /// 获取一个 HTML 解析器
    /// </summary>
    /// 
    /// <param name="virtualPath">HTML 内容虚拟路径</param>
    /// <param name="htmlContent">HTML 内容</param>
    /// <returns>HTML 解析器结果</returns>
    HtmlParserResult GetParser( string virtualPath, string htmlContent );


    /// <summary>
    /// 释放解析器实例
    /// </summary>
    /// <param name="parser"></param>
    void ReleaseParser( HtmlParserResult parser );

  }


  /// <summary>
  /// HTML 解析器结果
  /// </summary>
  public class HtmlParserResult
  {

    /// <summary>
    /// 创建 HtmlParserResult 对象
    /// </summary>
    /// <param name="parser">得到此解析结果的解析器</param>
    /// <param name="domProvider">HTML DOM 提供程序</param>
    /// <param name="provider">解析器提供程序</param>
    /// <param name="virtualPath">文档的虚拟路径</param>
    public HtmlParserResult( IHtmlParser parser, IHtmlDomProvider domProvider, IHtmlParserProvider provider, string virtualPath )
    {
      Parser = parser;
      DomProvider = domProvider;
      Provider = provider;
      VirtualPath = virtualPath;
    }


    /// <summary>
    /// HTML 解析器实例
    /// </summary>
    public IHtmlParser Parser
    {
      get;
      private set;
    }

    /// <summary>
    /// 用于构建 HTML DOM 的提供程序
    /// </summary>
    public IHtmlDomProvider DomProvider
    {
      get;
      private set;
    }

    /// <summary>
    /// 产生此结果的 HTML 解析器提供程序
    /// </summary>
    public IHtmlParserProvider Provider
    {
      get;
      private set;
    }

    /// <summary>
    /// 所要解析内容的虚拟路径
    /// </summary>
    public string VirtualPath
    {
      get;
      private set;
    }

  }


  /// <summary>
  /// 默认的 HTML 解析器提供程序
  /// </summary>
  public class DefaultParserProvider : IHtmlParserProvider
  {



    /// <summary>
    /// 获取默认的解析器结果
    /// </summary>
    /// <returns>包含默认解析器的结果</returns>
    public HtmlParserResult GetParser( string virtualPath )
    {
      var parser = new JumonyParser();
      return new HtmlParserResult( parser, new DomProvider( parser ), this, virtualPath );
    }



    HtmlParserResult IHtmlParserProvider.GetParser( string virtualPath, string htmlContent )
    {
      return GetParser( virtualPath );
    }

    void IHtmlParserProvider.ReleaseParser( HtmlParserResult parser )
    {
    }
  }
}
