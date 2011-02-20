using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ivony.Html
{
  /// <summary>
  /// 定义创建HTML文档对象的工厂
  /// </summary>
  public interface IHtmlNodeFactory
  {

    /// <summary>
    /// 所属的文档
    /// </summary>
    IHtmlDocument Document
    {
      get;
    }


    /// <summary>
    /// 创建 HTML 碎片对象
    /// </summary>
    /// <returns>HTML 碎片，其作为游离节点的容器</returns>
    HtmlFragment CreateFragment();


    /// <summary>
    /// 创建元素对象
    /// </summary>
    /// <param name="name">元素名</param>
    /// <returns></returns>
    IFreeElement CreateElement( string name );

    /// <summary>
    /// 创建文本节点
    /// </summary>
    /// <param name="htmlText">HTML文本</param>
    /// <returns></returns>
    IFreeTextNode CreateTextNode( string htmlText );

    /// <summary>
    /// 创建注释节点
    /// </summary>
    /// <param name="comment">注释文本</param>
    /// <returns></returns>
    IFreeComment CreateComment( string comment );

    /// <summary>
    /// 分析一小段HTML文本并创建一个HTML碎片
    /// </summary>
    /// <param name="html">要分析的HTML文本</param>
    /// <returns></returns>
    HtmlFragment ParseHtml( string html );

  }
}