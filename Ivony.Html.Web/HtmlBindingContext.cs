using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ivony.Fluent;
using System.Collections;

using Ivony.Html.ExpandedAPI;
using System.Web.UI;

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
    /// <param name="scope">要进行数据绑定的范畴</param>
    /// <param name="dataContext">数据上下文</param>
    /// <param name="data">数据字典</param>
    public HtmlBindingContext( IHtmlElementBinder[] binders, IHtmlContainer scope, object dataContext = null, IDictionary<string, object> data = null )
    {
      Binders = binders;
      BindingScope = scope;
      DataContext = dataContext;
      Data = data;
    }


    /// <summary>
    /// 创建 HtmlBindingContext 实例
    /// </summary>
    /// <param name="scope">要进行数据绑定的范畴</param>
    /// <param name="bindingContext">父级数据绑定上下文</param>
    /// <param name="dataContext">数据上下文</param>
    /// <param name="data">数据字典</param>
    protected HtmlBindingContext( HtmlBindingContext bindingContext, IHtmlContainer scope, object dataContext = null, IDictionary<string, object> data = null )
    {
      ParentContext = bindingContext;
      BindingScope = scope;
      DataContext = dataContext ?? bindingContext.DataContext;
      Data = data ?? bindingContext.Data;

      Binders = bindingContext.Binders;

    }



    /// <summary>
    /// 父级绑定上下文
    /// </summary>
    public HtmlBindingContext ParentContext { get; private set; }


    /// <summary>
    /// 进行绑定的范畴
    /// </summary>
    public IHtmlContainer BindingScope { get; private set; }


    /// <summary>
    /// 当前的数据上下文
    /// </summary>
    public object DataContext { get; private set; }


    /// <summary>
    /// 进行绑定的范畴的数据容器
    /// </summary>
    public IDictionary<string, object> Data { get; private set; }

    /// <summary>
    /// 元素绑定器
    /// </summary>
    public IHtmlElementBinder[] Binders { get; private set; }


    /// <summary>
    /// 进行数据绑定
    /// </summary>
    public virtual void DataBind()
    {

      var element = BindingScope as IHtmlElement;
      if ( element != null )
        DataBind( element );

      else
        BindChilds( BindingScope );

    }


    /// <summary>
    /// 遍历绑定所有子元素
    /// </summary>
    /// <param name="container">要绑定子元素的容器</param>
    protected virtual void BindChilds( IHtmlContainer container )
    {
      foreach ( var child in container.Elements().ToArray() )
        BindElement( child );
    }



    /// <summary>
    /// 对元素进行数据绑定
    /// </summary>
    /// <param name="element">要绑定数据的元素</param>
    protected virtual void DataBind( IHtmlElement element )
    {
      var attributes = element.Attributes().ToArray();
      attributes.ForAll( a => BindAttribute( Binders, a ) );

      BindElement( Binders, element );

      BindChilds( element );
    }


    /// <summary>
    /// 对元素进行数据绑定
    /// </summary>
    /// <param name="element">要绑定数据的元素</param>
    protected virtual void BindElement( IHtmlElement element )
    {
      var dataContext = GetDataContext( element );

      var bindingContext = this;
      if ( dataContext != null )
      {
        var listData = dataContext as IListDataContext;
        if ( listData != null )
          bindingContext = new HtmlListBindingContext( this, element, listData );

        else
          bindingContext = new HtmlBindingContext( this, element, dataContext );
      }

      bindingContext.DataBind( element );
    }



    /// <summary>
    /// 获取数据上下文
    /// </summary>
    /// <param name="element">当前正在处理的元素</param>
    /// <returns>数据上下文，如果在当前元素被设置的话。</returns>
    protected virtual object GetDataContext( IHtmlElement element )
    {
      var expression = AttributeExpression.ParseExpression( element.Attribute( "datacontext" ) );

      object dataContext = null;

      if ( expression != null )
        dataContext = GetDataObject( expression, this );

      var list = dataContext as IEnumerable;
      if ( list != null )
        dataContext = new ListDataContext( list );

      element.RemoveAttribute( "datacontext" );

      return dataContext;
    }


    /// <summary>
    /// 解析属性表达式，获取数据对象
    /// </summary>
    /// <param name="expression">属性表达式</param>
    /// <param name="context">当前绑定上下文</param>
    /// <returns>数据对象</returns>
    internal static object GetDataObject( AttributeExpression expression, HtmlBindingContext context )
    {
      //获取绑定数据源

      string key;
      object dataObject;

      if ( expression.Arguments.TryGetValue( "key", out key ) || expression.Arguments.TryGetValue( "name", out key ) )
        context.Data.TryGetValue( key, out dataObject );
      else
        dataObject = context.DataContext;

      if ( dataObject == null )
        return null;


      string path;

      if ( expression.Arguments.TryGetValue( "path", out path ) )
        dataObject = DataBinder.Eval( dataObject, path );

      return dataObject;
    }





    /// <summary>
    /// 进行元素绑定
    /// </summary>
    /// <param name="binders"></param>
    /// <param name="element"></param>
    protected virtual void BindElement( IHtmlElementBinder[] binders, IHtmlElement element )
    {
      foreach ( var binder in binders )
      {
        if ( binder.BindElement( this, element ) )
          break;
      }
    }



    /// <summary>
    /// 进行属性绑定
    /// </summary>
    /// <param name="attribute">要绑定的属性</param>
    protected virtual void BindAttribute( IHtmlElementBinder[] binders, IHtmlAttribute attribute )
    {
      foreach ( var binder in binders )
      {
        if ( binder.BindAttribute( this, attribute ) )
          break;
      }
    }
  }
}
