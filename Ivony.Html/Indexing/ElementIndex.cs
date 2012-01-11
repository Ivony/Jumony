using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ivony.Fluent;

namespace Ivony.Html.Indexing
{
  /// <summary>
  /// 所有元素索引的基类型
  /// </summary>
  public abstract class ElementIndex
  {


    /// <summary>
    /// 创建元素索引实例
    /// </summary>
    /// <param name="document">所依附的文档</param>
    protected ElementIndex( IHtmlDocument document )
    {
      Document = document;

      var dataContainer = document as IDataContainer;

      if ( dataContainer == null )
        throw new NotSupportedException();

      var data = dataContainer.Data;

      lock ( data.SyncRoot )
      {

        var key = GetType();

        if ( dataContainer.Data.Contains( key ) )
          throw new InvalidOperationException( "索引已创建，无法重复创建" );


        data.Add( key, this );

      }

      var notifyChanged = document as INotifyDomChanged;

      if ( notifyChanged == null )
        throw new NotSupportedException();

      notifyChanged.HtmlDomChanged += OnHtmlDomChanged;
    }


    /// <summary>
    /// 索引所依附的文档
    /// </summary>
    public IHtmlDocument Document
    {
      get;
      private set;
    }


    /// <summary>
    /// 重建索引
    /// </summary>
    public void Rebuild()
    {

      InitializeData();

      Document.Descendants().ForAll( element => AddElement( element ) );

    }


    protected virtual void InitializeData()
    {
    }



    /// <summary>
    /// 添加一个元素到索引
    /// </summary>
    /// <param name="element">要添加到索引的元素</param>
    protected abstract void AddElement( IHtmlElement element );

    /// <summary>
    /// 从索引中移除一个元素
    /// </summary>
    /// <param name="element">要从索引中移除的元素</param>
    protected abstract void RemoveElement( IHtmlElement element );


    /// <summary>
    /// 当元素被添加属性时
    /// </summary>
    /// <param name="element">添加属性的元素</param>
    /// <param name="attribute">被添加的属性</param>
    protected abstract void OnAddAttribute( IHtmlElement element, IHtmlAttribute attribute );

    /// <summary>
    /// 当元素被移除属性时
    /// </summary>
    /// <param name="element">移除属性的元素</param>
    /// <param name="attribute">被移除的属性</param>
    protected abstract void OnRemoveAttribute( IHtmlElement element, IHtmlAttribute attribute );




    private void OnHtmlDomChanged( object sender, HtmlDomChangedEventArgs e )
    {
      var element = e.Node as IHtmlElement;
      if ( element == null )//如果引发修改事件的不是元素，则忽略。
        return;

      if ( !e.IsAttributeChanged )
        OnElementChanged( sender, e.Action, element );
      else
        OnAttributeChanged( sender, e.Action, e.Attribute, element );
    }



    /// <summary>
    /// 当属性被修改
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="action"></param>
    /// <param name="attribute"></param>
    protected virtual void OnAttributeChanged( object sender, HtmlDomChangedAction action, IHtmlAttribute attribute, IHtmlElement element )
    {

      switch ( action )
      {

        case HtmlDomChangedAction.Add:
          OnAddAttribute( element, attribute );
          break;

        case HtmlDomChangedAction.Remove:
          OnRemoveAttribute( element, attribute );
          break;

        default:
          throw new InvalidOperationException( "未知的 DOM 结构变化" );
      }
    }



    /// <summary>
    /// 当元素被修改
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="element"></param>
    /// <param name="e"></param>
    protected virtual void OnElementChanged( object sender, HtmlDomChangedAction action, IHtmlElement element )
    {
      switch ( action )
      {

        case HtmlDomChangedAction.Add:
          AddElement( element );
          break;

        case HtmlDomChangedAction.Remove:
          RemoveElement( element );
          break;

        default:
          throw new InvalidOperationException( "未知的 DOM 结构变化" );
      }
    }





  }
}
