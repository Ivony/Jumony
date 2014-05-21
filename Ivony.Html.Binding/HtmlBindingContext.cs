using Ivony.Fluent;
using Ivony.Html.ExpandedAPI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
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
    /// <param name="dataModel">数据上下文</param>
    public static HtmlBindingContext Create( IHtmlBinder[] htmlBinders, IExpressionBinder[] expressionBinders, IHtmlContainer scope, object dataModel )
    {
      return new HtmlBindingContext( htmlBinders, expressionBinders, scope, dataModel );

    }


    private HtmlBindingContext( IHtmlBinder[] htmlBinders, IExpressionBinder[] expressionBinders, IHtmlContainer scope, object dataModel )
    {

      if ( htmlBinders == null )
        throw new ArgumentNullException( "binders" );

      if ( scope == null )
        throw new ArgumentNullException( "scope" );

      Binders = htmlBinders;
      BindingScope = scope;
      DataModel = dataModel;

      _expressionBinders = new ExpressionBinderCollection( expressionBinders );
    }


    /// <summary>
    /// 创建 HtmlBindingContext 实例
    /// </summary>
    /// <param name="scope">要进行数据绑定的范畴</param>
    /// <param name="bindingContext">父级数据绑定上下文</param>
    /// <param name="dataModel">数据模型，若不提供则使用当前的数据模型</param>
    protected HtmlBindingContext( HtmlBindingContext bindingContext, IHtmlContainer scope, object dataModel = null )
    {

      if ( bindingContext == null )
        throw new ArgumentNullException( "bindingContext" );

      if ( scope == null )
        throw new ArgumentNullException( "scope" );

      ParentContext = bindingContext;
      BindingScope = scope;
      DataModel = dataModel ?? bindingContext.DataModel;

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
    public object DataModel { get; private set; }


    /// <summary>
    /// 元素绑定器
    /// </summary>
    public IHtmlBinder[] Binders { get; private set; }


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

          if ( dataModel == null )//如果获取到的数据模型是 null ，则直接移除整个元素
            element.Remove();

          else
            CreateBindingContext( this, element, dataModel ).DataBind();//若在当前元素定义了数据模型，创建一个新的绑定上下文并绑定。

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



    /// <summary>
    /// 尝试获取数据模型
    /// </summary>
    /// <param name="element">当前正在处理的元素</param>
    /// <param name="dataModel">获取到的数据模型</param>
    /// <returns>是否成功获取数据模型</returns>
    protected virtual bool TryGetDataModel( IHtmlElement element, out object dataModel )
    {
      dataModel = null;

      var expression = BindingExpression.ParseExpression( element.Attribute( "datamodel" ).Value() );
      if ( expression == null )//若datamodel属性不是一个合法的数据绑定表达式，则忽略之。
        return false;


      element.RemoveAttribute( "datamodel" );//成功取得dataModel后，应删除该属性。


      if ( !TryGetValue( expression, out dataModel ) )
        throw new InvalidOperationException( "处理元素数据上下文绑定时出现错误，绑定表达式不支持获取数据对象" );
      //此处不检测dataContext是否等于当前dataContext的原因是，如果元素中写下datacontext属性并指向当前datacontext，表示需要在此处重新创建一个绑定上下文。

      return true;
    }



    /// <summary>
    /// 对元素进行数据绑定
    /// </summary>
    /// <param name="element">要绑定数据的元素</param>
    public virtual void DataBind( IHtmlElement element )
    {

      var attributes = element.Attributes().ToArray();
      attributes.ForAll( a => BindAttribute( a ) );

      if ( BindExpressionElement( element ) )
        return;

      BindElement( element );
    }



    protected bool BindExpressionElement( IHtmlElement element )
    {
      var expression = new ElementExpression( element );

      var expressionBinder = GetExpressionBinder( expression ) as IElementExpressionBinder;

      if ( expressionBinder == null )
        return false;

      var value = expressionBinder.GetValue( this, expression );

      if ( value == null )
        element.Remove();

      else
        element.ReplaceWith( HttpUtility.HtmlEncode( value ) );

      return true;
    }



    /// <summary>
    /// 进行元素绑定
    /// </summary>
    /// <param name="element">要进行绑定的元素</param>
    protected virtual void BindElement( IHtmlElement element )
    {

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

      var expression = BindingExpression.ParseExpression( attribute.Value() );
      if ( expression == null )
        return;

      var value = GetValue( expression );
      if ( value == null )
        attribute.Remove();

      else
        attribute.SetValue( value.ToString() );
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
    /// 获取绑定表达式的值
    /// </summary>
    /// <param name="expression">绑定表达式</param>
    /// <returns>绑定值</returns>
    public object GetValue( BindingExpression expression )
    {
      object value;
      if ( TryGetValue( expression, out value ) )
        return value;

      else
        return null;
    }


    /// <summary>
    /// 尝试获取绑定表达式的值
    /// </summary>
    /// <param name="expression">绑定表达式</param>
    /// <param name="value">绑定值</param>
    /// <returns>是否成功获取</returns>
    public bool TryGetValue( BindingExpression expression, out object value )
    {
      var expressionBinder = GetExpressionBinder( expression );

      if ( expressionBinder == null )
      {
        value = null;
        return false;
      }

      else
      {
        value = expressionBinder.GetValue( this, expression );
        return true;
      }
    }


    public bool TryConvertValue<T>( object obj, out T value )
    {
      if ( typeof( T ).IsAssignableFrom( obj.GetType() ) )
      {
        value = (T) obj;
        return true;
      }
      else
      {
        value = default( T );
        return false;
      }


    }

  }
}
