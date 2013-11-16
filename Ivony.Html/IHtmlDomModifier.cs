using System;
using System.Collections;
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


  /// <summary>
  /// HTML DOM 的实现通过实现此接口使得可以监视 HTML DOM 的变化。
  /// </summary>
  public interface INotifyDomChanged
  {
    /// <summary>
    /// 当 HTML DOM 结构发生改变时引发此事件
    /// </summary>
    event EventHandler<HtmlDomChangedEventArgs> HtmlDomChanged;
  }


  /// <summary>
  /// 同步修改 DOM 结构的 DomModifier
  /// </summary>
  public interface ISynchronizedDomModifier : IHtmlDomModifier
  {

    /// <summary>
    /// 同于同步操作的对象
    /// </summary>
    object SyncRoot { get; }
  }


  /// <summary>
  /// 此接口用于框架，请勿在代码中使用。
  /// </summary>
  public interface IVersionControl : IHtmlDomModifier
  {

    /// <summary>
    /// 获取文档当前的版本号，每修改一次文档该版本号便累加。
    /// </summary>
    int Version { get; }
  }


  /// <summary>
  /// 此接口用于框架，请勿在代码中使用。
  /// </summary>
  public interface IVersionCacheContainer : IHtmlDocument
  {

    /// <summary>
    /// 获取当前版本缓存的数据。
    /// </summary>
    Hashtable CurrenctVersionCache { get; }
  }




  /// <summary>
  /// 为 HTML DOM 节点事件提供参数
  /// </summary>
  public class HtmlDomChangedEventArgs : EventArgs
  {
    /// <summary>
    /// 构建 HtmlDomChangedEventArgs 对象
    /// </summary>
    /// <param name="node">发生变化的节点</param>
    /// <param name="container">节点所属的容器</param>
    /// <param name="action">节点所发生的操作</param>
    public HtmlDomChangedEventArgs( IHtmlNode node, IHtmlContainer container, HtmlDomChangedAction action )
    {
      IsAttributeChanged = false;
      Node = node;
      Container = container;
      Action = action;
    }


    /// <summary>
    /// 构建 HtmlDomChangedEventArgs 对象
    /// </summary>
    /// <param name="attribute">发生变化的属性</param>
    /// <param name="element">属性所属的元素</param>
    /// <param name="action">属性所发生的操作</param>
    public HtmlDomChangedEventArgs( IHtmlAttribute attribute, IHtmlElement element, HtmlDomChangedAction action )
    {
      IsAttributeChanged = true;
      Attribute = attribute;
      Container = element;
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
    /// 引发事件的属性，如果是因为属性所引发事件。
    /// </summary>
    public IHtmlAttribute Attribute
    {
      get;
      private set;
    }


    /// <summary>
    /// 确认是否由属性修改所引发的事件
    /// </summary>
    public bool IsAttributeChanged
    {
      get;
      private set;
    }




    /// <summary>
    /// 节点被移除前，节点所属的容器。
    /// </summary>
    /// <remarks>
    /// 根据引发事件的是节点还是属性，Container 属性和 Node 属性的取值如下：
    /// 若引发事件的是节点，则 Node 属性引用引发事件的节点， Container 属性引用引发事件之前节点所属的容器；
    /// 若引发事件的是属性，则 Node 属性和 Container 属性都引用引发事件之前属性所属的元素。
    /// </remarks>
    public IHtmlContainer Container
    {
      get;
      private set;
    }



    /// <summary>
    /// 对节点或对象的操作
    /// </summary>
    public HtmlDomChangedAction Action
    {
      get;
      private set;
    }

  }


  /// <summary>
  /// DOM 对象所发生的操作
  /// </summary>
  public enum HtmlDomChangedAction
  {
    /// <summary>节点或对象被新增</summary>
    Add,
    /// <summary>节点或对象被移除</summary>
    Remove
  }



}
