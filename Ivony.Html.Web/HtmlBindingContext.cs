using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ivony.Fluent;
using System.Collections;
using System.Linq;
using Ivony.Html.ExpandedAPI;

namespace Ivony.Html.Web
{
  
  /// <summary>
  /// 绑定上下文
  /// </summary>
  public class HtmlBindingContext
  {
    /// <summary>
    /// 创建 HtmlBindingContext 实例
    /// </summary>
    /// <param name="binders">所有可以用于绑定的绑定器</param>
    public HtmlBindingContext( IHtmlElementBinder[] binders )
    {
      Binders = binders;
    }

    /// <summary>
    /// 进行绑定的范畴
    /// </summary>
    public IHtmlContainer BindingScope { get; private set; }

    /// <summary>
    /// 进行绑定的范畴的数据容器
    /// </summary>
    public IDictionary<string, object> Data { get; private set; }

    /// <summary>
    /// 当前的数据上下文
    /// </summary>
    public object DataContext
    {
      get { return _bindingDataContexts.Peek().DataContext; }
    }

    /// <summary>
    /// 当前数据上下文应用的范畴
    /// </summary>
    public IHtmlContainer DataContextScope
    {
      get { return _bindingDataContexts.Peek().Scope; }
    }

    /// <summary>
    /// 元素绑定器
    /// </summary>
    public IHtmlElementBinder[] Binders { get; private set; }


    /// <summary>
    /// 进行数据绑定
    /// </summary>
    /// <param name="scope">要进行数据绑定的范畴</param>
    /// <param name="dataContext">数据上下文</param>
    /// <param name="data">数据字典</param>
    public void DataBind( IHtmlContainer scope, object dataContext, IDictionary<string, object> data )
    {

      _bindingDataContexts = new Stack<BindingDataContext>();
      _bindingDataContexts.Push( new BindingDataContext { DataContext = dataContext, Scope = scope } );

      BindingScope = scope;
      Data = data;

      foreach ( var element in scope.Elements().ToArray() )
        BindElement( element );
    }


    /// <summary>
    /// 对元素进行数据绑定
    /// </summary>
    /// <param name="element">要绑定数据的元素</param>
    private void BindElement( IHtmlElement element )
    {


      element.Attributes().ToArray().ForAll( a => BindAttribute( a ) );

      object dataContext = null;
      Binders.FirstOrDefault( b => b.BindElement( element, this, out dataContext ) );


      if ( dataContext != null )
      {
        _bindingDataContexts.Push( new BindingDataContext { DataContext = dataContext, Scope = element } );

        var listData = dataContext as IEnumerable;
        if ( listData != null && IsListItem( element ) )
          BindElementList( element, listData.Cast<object>().ToArray() );

        else
          BindChilds( element );

        _bindingDataContexts.Pop();
      }
      else
        BindChilds( element );
    }


    /// <summary>
    /// 遍历绑定所有子元素
    /// </summary>
    /// <param name="element">要绑定子元素的元素</param>
    private void BindChilds( IHtmlElement element )
    {
      foreach ( var child in element.Elements().ToArray() )
        BindElement( child );
    }


    /// <summary>
    /// 绑定元素列表
    /// </summary>
    /// <param name="element">要绑定元素列表的元素</param>
    /// <param name="data">列表数据</param>
    private void BindElementList( IHtmlElement element, object[] data )
    {
      var elementList = element.Repeat( data.Length );

      for ( int i = 0; i < data.Length; i++ )
      {
        var d = data[i];
        var e = elementList[i];
        _bindingDataContexts.Push( new BindingDataContext { DataContext = d, Scope = e } );

        BindChilds( e );

        _bindingDataContexts.Pop();
      }
    }



    private bool IsListItem( IHtmlElement element )
    {
      return element.Name.EqualsIgnoreCase( "li" ) || element.Name.EqualsIgnoreCase( "tr" ) || element.Name.EqualsIgnoreCase( "view" ) || element.Name.EqualsIgnoreCase( "binding" );
    }


    /// <summary>
    /// 进行属性绑定
    /// </summary>
    /// <param name="attribute">要绑定的属性</param>
    private void BindAttribute( IHtmlAttribute attribute )
    {
      Binders.FirstOrDefault( b => b.BindAttribute( attribute, this ) );
    }



    private Stack<BindingDataContext> _bindingDataContexts;


    private class BindingDataContext
    {
      /// <summary>
      /// 数据上下文
      /// </summary>
      public object DataContext { get; set; }

      /// <summary>
      /// 数据上下文所存在的范畴
      /// </summary>
      public IHtmlContainer Scope { get; set; }
    }

  }
}
