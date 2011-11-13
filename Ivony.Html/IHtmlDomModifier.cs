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


    //void ResolveUri( IHtmlDocument document, Uri uri );



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




    /// <summary>
    /// 为元素添加一个属性
    /// </summary>
    /// <param name="element">要添加属性的元素</param>
    /// <param name="name">属性名</param>
    /// <param name="value">属性值</param>
    /// <returns>被添加的属性对象</returns>
    IHtmlAttribute AddAttribute( IHtmlElement element, string name, string value );

    /// <summary>
    /// 从元素中移除一个属性
    /// </summary>
    /// <param name="attribute">要移除的属性</param>
    void RemoveAttribute( IHtmlAttribute attribute );


  }



  public interface INotifyDomChanged : IHtmlContainer
  {
    event EventHandler<HtmlNodeEventArgs> HtmlDomChanged;
  }



  /// <summary>
  /// 为 HTML DOM 节点事件提供参数
  /// </summary>
  public class HtmlNodeEventArgs : EventArgs
  {
    /// <summary>
    /// 构建 HtmlNodeEventArgs 对象
    /// </summary>
    /// <param name="node"></param>
    public HtmlNodeEventArgs( IHtmlNode node, HtmlDomChangedAction action )
    {
      Node = node;
      Action = action;
    }

    /// <summary>
    /// 引发事件的节点
    /// </summary>
    public IHtmlNode Node
    {
      get;
      private set;
    }


    /// <summary>
    /// 对节点的操作
    /// </summary>
    public HtmlDomChangedAction Action
    {
      get;
      private set;
    }

  }


  public enum HtmlDomChangedAction
  {
    /// <summary>
    /// 节点被新增
    /// </summary>
    Add,
    /// <summary>
    /// 节点被移除
    /// </summary>
    Remove
  }



}
