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
    IHtmlParser GetParser( string virtualPath, string htmlContent );


    /// <summary>
    /// 释放解析器实例
    /// </summary>
    /// <param name="parser"></param>
    void ReleaseParser( IHtmlParser parser );

  }
}
