using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ivony.Fluent;
using System.Collections;

using Ivony.Html.ExpandedAPI;
using System.Web.UI;
using Ivony.Web;

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
    /// <param name="dataValues">数据字典</param>

    public static HtmlBindingContext CreateInstance( IHtmlElementBinder[] binders, IHtmlContainer scope, object dataContext = null, IDictionary<string, object> dataValues = null )
    {
      return new HtmlBindingContext( binders, scope, dataContext, dataValues ?? new Dictionary<string, object>() );

    }

    private HtmlBindingContext( IHtmlElementBinder[] binders, IHtmlContainer scope, object dataContext, IDictionary<string, object> dataValues )
    {

      if ( binders == null )
        throw new ArgumentNullException( "binders" );

      if ( scope == null )
        throw new ArgumentNullException( "scope" );

      Binders = binders;
      BindingScope = scope;
      DataContext = dataContext;
      Data = dataValues;
    }


    /// <summary>
    /// 创建 HtmlBindingContext 实例
    /// </summary>
    /// <param name="scope">要进行数据绑定的范畴</param>
    /// <param name="bindingContext">父级数据绑定上下文</param>
    /// <param name="dataContext">数据上下文</param>
    /// <param name="data">数据字典</param>
    protected HtmlBindingContext( HtmlBindingContext bindingContext, IHtmlContainer scope, object dataContext = null, IDictionary<string, object> dataValues = null )
    {

      if ( bindingContext == null )
        throw new ArgumentNullException( "bindingContext" );

      if ( scope == null )
        throw new ArgumentNullException( "scope" );

      ParentContext = bindingContext;
      BindingScope = scope;
      DataContext = dataContext ?? bindingContext.DataContext;
      Data = dataValues ?? bindingContext.Data;

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
      if ( dataContext == null )
      {
        bindingContext.DataBind( element );
        return;
      }


      if ( dataContext is NoneDataContext )//如果绑定的 DataContext 是空，则直接移除整个元素
      {
        element.Remove();
        return;
      }


      CreateBindingContext( this, element, dataContext ).DataBind();
    }

    protected HtmlBindingContext CreateBindingContext( HtmlBindingContext bindingContext, IHtmlElement element, object dataContext )
    {
      var services = WebServiceLocator.GetServices<IHtmlBindingContextProvider>();

      foreach ( var provider in services )
      {
        var context = provider.CreateBindingContext( bindingContext, element, dataContext, null );
        if ( context != null )
          return context;
      }

      var listData = dataContext as IEnumerable;
      if ( listData != null )
        return new HtmlRepeatBindingContext( bindingContext, element, listData );

      return new HtmlBindingContext( bindingContext, element, dataContext );
    }



    private class NoneDataContext { }

    /// <summary>
    /// 获取数据上下文
    /// </summary>
    /// <param name="element">当前正在处理的元素</param>
    /// <returns>数据上下文，如果在当前元素被设置的话。</returns>
    protected virtual object GetDataContext( IHtmlElement element )
    {
      var expression = AttributeExpression.ParseExpression( element.Attribute( "datacontext" ) );
      if ( expression == null )
        return null;


      var dataContext = GetDataContext( expression );
      //此处不检测dataContext是否等于当前dataContext的原因是，如果元素中写下datacontext属性并指向当前datacontext，表示需要在此处重新创建一个绑定上下文。

      if ( dataContext == null )
        return new NoneDataContext();//如果 dataContext 获取失败，通知直接删除整个元素。


      element.RemoveAttribute( "datacontext" );//成功取得dataContext后，应删除该属性。
      return dataContext;
    }


    protected virtual object GetDataContext( AttributeExpression expression )
    {
      var dataContext = GetDataObject( expression, this );

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
