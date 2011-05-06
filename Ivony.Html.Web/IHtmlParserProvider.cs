using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

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


}
