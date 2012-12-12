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
    /// <param name="context">当前请求上下文</param>
    /// <param name="contentUri">HTML 内容地址</param>
    /// <param name="htmlContent">HTML 内容</param>
    /// <returns>HTML 解析器结果</returns>
    HtmlParserResult GetParser( HttpContextBase context, Uri contentUri, string htmlContent );


    /// <summary>
    /// 释放解析器实例
    /// </summary>
    /// <param name="parser"></param>
    void ReleaseParser( IHtmlParser parser );

  }


  /// <summary>
  /// HTML 解析器结果
  /// </summary>
  public class HtmlParserResult
  {

    /// <summary>
    /// HTML 解析器实例
    /// </summary>
    public IHtmlParser Parser
    {
      get;
      set;
    }

    /// <summary>
    /// 用于构建 HTML DOM 的提供程序
    /// </summary>
    public IHtmlDomProvider DomProvider
    {
      get;
      set;
    }

    /// <summary>
    /// 产生此结果的 HTML 解析器提供程序
    /// </summary>
    public IHtmlParserProvider Provider
    {
      get;
      internal set;
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
    public HtmlParserResult GetParser()
    {
      return new HtmlParserResult()
      {
        Parser = new WebParser(),
        DomProvider = new DomProvider(),
        Provider = this
      };
    }



    HtmlParserResult IHtmlParserProvider.GetParser( HttpContextBase context, Uri contentUri, string htmlContent )
    {
      return GetParser();
    }

    void IHtmlParserProvider.ReleaseParser( IHtmlParser parser )
    {
    }
  }


}
