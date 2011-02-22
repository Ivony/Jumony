using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ivony.Html
{

  /// <summary>
  /// 提供修改 DOM 结构的方法
  /// </summary>
  public interface IHtmlDomModifier
  {

    /// <summary>
    /// 向指定容器中添加一个元素
    /// </summary>
    /// <param name="container">要添加元素的容器</param>
    /// <param name="index">添加的位置</param>
    /// <param name="name">元素名</param>
    /// <returns>添加好的元素</returns>
    IHtmlElement AddElement( IHtmlContainer container, int index, string name );

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


    /// <summary>
    /// 从 DOM 中移除一个节点
    /// </summary>
    /// <param name="node">要移除的节点</param>
    void RemoveNode( IHtmlNode node );


  }
}
