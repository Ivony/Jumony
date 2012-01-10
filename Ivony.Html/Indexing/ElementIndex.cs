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



    protected abstract void AddElement( IHtmlElement element );

    protected abstract void RemoveElement( IHtmlElement element );

    protected abstract void UpdateElement( IHtmlElement element, IHtmlAttribute attribute, HtmlDomChangedAction action );


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
      UpdateElement( element, attribute, action );
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
