using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ivony.Html
{

  /// <summary>
  /// 用于创建 HTML DOM 的提供程序
  /// </summary>
  public interface IHtmlDomProvider
  {

    /// <summary>
    /// 创建一个空白文档
    /// </summary>
    /// <param name="uri">文档的统一资源路径</param>
    /// <returns>空白文档</returns>
    IHtmlDocument CreateDocument( Uri uri );

    /// <summary>
    /// 向指定容器中添加一个元素
    /// </summary>
    /// <param name="container">要添加元素的容器</param>
    /// <param name="name">元素名</param>
    /// <param name="attributes">元素属性</param>
    /// <returns>添加好的元素</returns>
    IHtmlElement AddElement( IHtmlContainer container, string name, IDictionary<string, string> attributes );

    /// <summary>
    /// 向指定容器中添加一个文本节点
    /// </summary>
    /// <param name="container">要添加节点的容器</param>
    /// <param name="htmlText">HTML 文本</param>
    /// <returns>添加好的文本节点</returns>
    IHtmlTextNode AddTextNode( IHtmlContainer container, string htmlText );

    /// <summary>
    /// 向指定容器中添加一个注释
    /// </summary>
    /// <param name="container">要添加注释的容器</param>
    /// <param name="comment">HTML 注释内容</param>
    /// <returns>添加好的注释节点</returns>
    IHtmlComment AddComment( IHtmlContainer container, string comment );

    /// <summary>
    /// 向指定容器添加一个特殊标签
    /// </summary>
    /// <param name="container">要添加特殊标签的容器</param>
    /// <param name="html">特殊标签的 HTML</param>
    /// <returns>如果特殊标签作为一个节点而存在，则返回特殊节点，否则返回null。</returns>
    IHtmlSpecial AddSpecial( IHtmlContainer container, string html );

    /// <summary>
    /// 完成文档创建
    /// </summary>
    /// <param name="document">正在被创建的文档</param>
    /// <returns>创建好的文档对象</returns>
    IHtmlDocument CompleteDocument( IHtmlDocument document );
  }


}
