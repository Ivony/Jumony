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
    /// <returns>空白文档</returns>
    IHtmlDocument CreateDocument();


    /// <summary>
    /// 向指定容器中添加一个元素
    /// </summary>
    /// <param name="container">要添加元素的容器</param>
    /// <param name="index">添加的位置</param>
    /// <param name="name">元素名</param>
    /// <param name="attributes">元素属性</param>
    /// <returns>添加好的元素</returns>
    IHtmlElement AddElement( IHtmlContainer container, int index, string name, IDictionary<string, string> attributes );

    /// <summary>
    /// 向指定容器中添加一个文本节点
    /// </summary>
    /// <param name="container">要添加节点的容器</param>
    /// <param name="index">添加的位置</param>
    /// <param name="htmlText">HTML 文本</param>
    /// <returns>添加好的文本节点</returns>
    IHtmlTextNode AddTextNode( IHtmlContainer container, int index, string htmlText );

    /// <summary>
    /// 向指定容器中添加一个注释
    /// </summary>
    /// <param name="container">要添加注释的容器</param>
    /// <param name="index">添加的位置</param>
    /// <param name="comment">HTML 注释</param>
    /// <returns>添加好的注释节点</returns>
    IHtmlComment AddComment( IHtmlContainer container, int index, string comment );

    /// <summary>
    /// 向指定容器添加一个特殊标签
    /// </summary>
    /// <param name="container">要添加特殊标签的容器</param>
    /// <param name="index">添加的位置</param>
    /// <param name="html">特殊标签的HTML</param>
    /// <returns>如果特殊标签作为一个节点而存在，则返回特殊节点，否则返回null。</returns>
    IHtmlSpecial AddSpecial( IHtmlContainer container, int index, string html );

  }


  public interface IHtmlProvider<TDocument, TElement, TTextNode, TComment, TContainer>
    where TDocument : IHtmlDocument, TContainer
    where TElement : IHtmlElement, TContainer
    where TTextNode : IHtmlTextNode
    where TComment : IHtmlComment
    where TContainer : IHtmlContainer
  {
    /// <summary>
    /// 创建一个空白文档
    /// </summary>
    /// <returns>空白文档</returns>
    TDocument CreateDocument();


    /// <summary>
    /// 向指定容器中添加一个元素
    /// </summary>
    /// <param name="container">要添加元素的容器</param>
    /// <param name="index">添加的位置</param>
    /// <param name="name">元素名</param>
    /// <param name="attributes">元素属性</param>
    /// <returns>添加好的元素</returns>
    TElement AddElement( TContainer container, int index, string name, IDictionary<string, string> attributes );

    /// <summary>
    /// 向指定容器中添加一个文本节点
    /// </summary>
    /// <param name="container">要添加节点的容器</param>
    /// <param name="index">添加的位置</param>
    /// <param name="htmlText">HTML 文本</param>
    /// <returns>添加好的文本节点</returns>
    TTextNode AddTextNode( TContainer container, int index, string htmlText );

    /// <summary>
    /// 向指定容器中添加一个注释
    /// </summary>
    /// <param name="container">要添加注释的容器</param>
    /// <param name="index">添加的位置</param>
    /// <param name="comment">HTML 注释</param>
    /// <returns>添加好的注释节点</returns>
    TComment AddComment( TContainer container, int index, string comment );

  }
}
