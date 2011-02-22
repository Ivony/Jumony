using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ivony.Html
{

  /// <summary>
  /// 文档碎片管理器，用于分析和创建文档碎片
  /// </summary>
  public interface IHtmlFragmentManager
  {

    /// <summary>
    /// 文档碎片提供程序所属的文档
    /// </summary>
    IHtmlDocument Document
    {
      get;
    }


    /// <summary>
    /// 获取所有尚未分配的文档碎片
    /// </summary>
    IEnumerable<IHtmlFragment> AllFragments
    {
      get;
    }


    /// <summary>
    /// 创建一个文档碎片
    /// </summary>
    /// <returns></returns>
    IHtmlFragment CreateFragment();

    /// <summary>
    /// 分析并创建一个文档碎片
    /// </summary>
    /// <param name="html">要分析用于创建文档碎片的 HTML</param>
    /// <returns></returns>
    IHtmlFragment ParseFragment( string html );

  }

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
    /// 分析一小段HTML文本并创建一个HTML碎片
    /// </summary>
    /// <param name="html">要分析的HTML文本</param>
    /// <returns>分析好的 HTML 碎片</returns>
    HtmlFragment ParseFragment( string html );

    /// <summary>
    /// 创建 HTML 碎片对象
    /// </summary>
    /// <returns>HTML 碎片，其作为游离节点的容器</returns>
    HtmlFragment CreateFragment();


    /// <summary>
    /// 创建元素对象
    /// </summary>
    /// <param name="name">元素名</param>
    /// <returns>创建的游离元素</returns>
    IFreeElement CreateElement( string name );

    /// <summary>
    /// 创建文本节点
    /// </summary>
    /// <param name="htmlText">HTML文本</param>
    /// <returns>创建的文本节点</returns>
    IFreeTextNode CreateTextNode( string htmlText );

    /// <summary>
    /// 创建注释节点
    /// </summary>
    /// <param name="comment">注释文本</param>
    /// <returns>创建的注释节点</returns>
    IFreeComment CreateComment( string comment );

  }
}