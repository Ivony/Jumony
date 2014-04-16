using Ivony.Fluent;
using Ivony.Html.ExpandedAPI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;

namespace Ivony.Html.Binding
{

  /// <summary>
  /// 绑定上下文
  /// </summary>
  public class HtmlBindingContext : IBindingExpressionEvaluator
  {


    /// <summary>
    /// 创建 HtmlBindingContext 实例
    /// </summary>
    /// <param name="htmlBinders">要使用的 HTML 绑定器</param>
    /// <param name="expressionBinders">要使用的绑定表达式绑定器</param>
    /// <param name="scope">要进行数据绑定的范畴</param>
    /// <param name="dataContext">数据上下文</param>
    /// <param name="dataValues">数据字典</param>
    public static HtmlBindingContext Create( IHtmlElementBinder[] htmlBinders, IExpressionBinder[] expressionBinders, IHtmlContainer scope, object dataContext = null )
    {
      return new HtmlBindingContext( htmlBinders, expressionBinders, scope, dataContext );

    }


    private HtmlBindingContext( IHtmlElementBinder[] htmlBinders, IExpressionBinder[] expressionBinders, IHtmlContainer scope, object dataContext )
    {

      if ( htmlBinders == null )
        throw new ArgumentNullException( "binders" );

      if ( scope == null )
        throw new ArgumentNullException( "scope" );

      Binders = htmlBinders;
      BindingScope = scope;
      DataContext = dataContext;
      _expressionBinders = new ExpressionBinderCollection( expressionBinders );
    }


    /// <summary>
    /// 创建 HtmlBindingContext 实例
    /// </summary>
    /// <param name="scope">要进行数据绑定的范畴</param>
    /// <param name="bindingContext">父级数据绑定上下文</param>
    /// <param name="dataContext">数据上下文</param>
    protected HtmlBindingContext( HtmlBindingContext bindingContext, IHtmlContainer scope, object dataContext = null )
    {

      if ( bindingContext == null )
        throw new ArgumentNullException( "bindingContext" );

      if ( scope == null )
        throw new ArgumentNullException( "scope" );

      ParentContext = bindingContext;
      BindingScope = scope;
      DataContext = dataContext ?? bindingContext.DataContext;

      Binders = bindingContext.Binders;
      _expressionBinders = new ExpressionBinderCollection( bindingContext._expressionBinders );

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
    /// 元素绑定器
    /// </summary>
    public IHtmlElementBinder[] Binders { get; private set; }


    /// <summary>
    /// 进行数据绑定
    /// </summary>
    public virtual void DataBind()
    {

      if ( ParentContext == null )
      {

        var modifier = BindingScope.Document.DomModifier as ISynchronizedDomModifier;

        if ( modifier == null )
          throw new InvalidOperationException( "需要绑定的范畴是只读的，或者无法进行同步访问" );

        lock ( modifier.SyncRoot )
        {
          DataBindInternal();
        }
      }

      else
        DataBindInternal();
    }


    private void DataBindInternal()
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
      foreach ( var element in container.Elements().ToArray() )
      {
        object dataModel;
        if ( TryGetDataModel( element, out dataModel ) )
        {

          if ( dataModel == null )//如果获取到的数据对象是 null ，则直接移除整个元素
            element.Remove();

          else
            CreateBindingContext( this, element, dataModel ).DataBind();//若存在新的数据上下文，创建一个新的绑定上下文并绑定。

        }

        else
          DataBind( element );
      }
    }



    /// <summary>
    /// 创建一个绑定上下文
    /// </summary>
    /// <param name="bindingContext">目前的绑定上下文</param>
    /// <param name="element">目前正在进行绑定的元素</param>
    /// <param name="dataModel">元素上绑定的数据对象</param>
    /// <returns></returns>
    protected HtmlBindingContext CreateBindingContext( HtmlBindingContext bindingContext, IHtmlElement element, object dataModel )
    {

      var listData = dataModel as ListDataModel;
      if ( listData != null )
        return new HtmlListBindingContext( bindingContext, element, listData );

      return new HtmlBindingContext( bindingContext, element, dataModel );
    }



    private class NoneDataContext { }

    /// <summary>
    /// 获取数据上下文
    /// </summary>
    /// <param name="element">当前正在处理的元素</param>
    /// <returns>数据上下文，如果在当前元素被设置的话。</returns>
    protected virtual bool TryGetDataModel( IHtmlElement element, out object dataModel )
    {
      dataModel = null;

      var expression = BindingExpression.ParseExpression( this, element.Attribute( "datamodel" ).Value() );
      if ( expression == null )//若datamodel属性不是一个合法的数据绑定表达式，则忽略之。
        return false;


      element.RemoveAttribute( "datamodel" );//成功取得dataModel后，应删除该属性。


      if ( !TryGetDataModel( expression, out dataModel ) )
        throw new InvalidOperationException( "处理元素数据上下文绑定时出现错误，绑定表达式不支持获取数据对象" );
      //此处不检测dataContext是否等于当前dataContext的原因是，如果元素中写下datacontext属性并指向当前datacontext，表示需要在此处重新创建一个绑定上下文。

      return true;
    }



    /// <summary>
    /// 对元素进行数据绑定
    /// </summary>
    /// <param name="element">要绑定数据的元素</param>
    protected virtual void DataBind( IHtmlElement element )
    {
      var attributes = element.Attributes().ToArray();
      attributes.ForAll( a => BindAttribute( a ) );

      BindElement( element );
    }


    /// <summary>
    /// 进行元素绑定
    /// </summary>
    /// <param name="element">要进行绑定的元素</param>
    protected virtual void BindElement( IHtmlElement element )
    {

      var expression = new ElementExpression( element );

      var expressionBinder = GetExpressionBinder( expression ) as IElementExpressionBinder;

      if ( expressionBinder != null )
      {
        var value = expressionBinder.GetValue( this, expression.Arguments );

        if ( value == null )
          element.Remove();

        else
          element.ReplaceWith( value );

        return;
      }


      CancelChildsBinding = BindCompleted = false;//重置绑定状态


      foreach ( var binder in Binders )
      {
        binder.BindElement( this, element );

        if ( BindCompleted )
          break;
      }

      if ( !CancelChildsBinding )
        BindChilds( element );
    }


    /// <summary>
    /// 获取或设置一个值，指定元素绑定已经完成，无需遍历后面的绑定处理程序。
    /// </summary>
    public bool BindCompleted { get; set; }

    /// <summary>
    /// 获取或设置一个值，指定是否取消子元素的绑定。
    /// </summary>
    public bool CancelChildsBinding { get; set; }




    /// <summary>
    /// 进行属性绑定
    /// </summary>
    /// <param name="attribute">要绑定的属性</param>
    protected virtual void BindAttribute( IHtmlAttribute attribute )
    {

      var expression = BindingExpression.ParseExpression( this, attribute.Value() );
      if ( expression == null )
        return;

      string value = GetValue( expression );


      if ( value == null )
        attribute.Remove();

      else
        attribute.SetValue( value );
    }


    /// <summary>
    /// 获取绑定表达式绑定器
    /// </summary>
    /// <param name="expression">绑定表达式</param>
    /// <returns>绑定表达式绑定器</returns>
    protected IExpressionBinder GetExpressionBinder( BindingExpression expression )
    {
      var name = expression.Name;
      lock ( _expressionBinders.SyncRoot )
      {
        if ( _expressionBinders.Contains( name ) )
          return _expressionBinders[expression.Name];

        else
          return null;
      }
    }



    private ExpressionBinderCollection _expressionBinders;

    /// <summary>
    /// 当前上下文可用于处理绑定表达式的绑定器
    /// </summary>
    protected ICollection<IExpressionBinder> ExpressionBinders
    {
      get { return _expressionBinders; }
    }





    /// <summary>
    /// 从 BindingExpression 获取需要绑定的值
    /// </summary>
    /// <param name="expression">绑定表达式</param>
    /// <returns>绑定值</returns>
    public string GetValue( BindingExpression expression )
    {
      var expressionBinder = GetExpressionBinder( expression );

      if ( expressionBinder == null )
        return null;

      return expressionBinder.GetValue( this, expression.Arguments );

    }


    /// <summary>
    /// 从绑定表达式中获取数据对象
    /// </summary>
    /// <param name="expression">绑定表达式</param>
    /// <param name="dataModel">数据对象</param>
    /// <returns></returns>
    public virtual bool TryGetDataModel( BindingExpression expression, out object dataModel )
    {
      dataModel = null;
      var binder = GetExpressionBinder( expression ) as IDataObjectExpressionBinder;
      if ( binder == null )
        return false;

      dataModel = binder.GetDataObject( this, expression.Arguments );
      return true;
    }


  }
}
