using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;
using Ivony.Fluent;

namespace Ivony.Html.Web.Binding
{


  /// <summary>
  /// 数据绑定管理器
  /// </summary>
  public abstract class BindingManager
  {

    /// <summary>
    /// 所管理的文档
    /// </summary>
    public IHtmlDocument Document
    {
      get;
      private set;
    }

    /// <summary>
    /// 创建 BindingManager 对象
    /// </summary>
    /// <param name="document">要进行数据绑定的文档</param>
    public BindingManager( IHtmlDocument document )
    {
      Document = document;

      var modifier = document.DomModifier as ISynchronizedDomModifier;
      if ( modifier == null )
        throw new NotSupportedException();

      SyncRoot = modifier.SyncRoot;
    }


    /// <summary>
    /// 用于同步的对象
    /// </summary>
    protected object SyncRoot
    {
      get;
      private set;
    }


    /// <summary>
    /// 进行数据绑定
    /// </summary>
    public void DataBind( object dataContext )
    {
      lock ( SyncRoot )
      {
        DataBind( Document, dataContext );
      }
    }


    /// <summary>
    /// 对文档进行数据绑定
    /// </summary>
    /// <param name="document">要进行数据绑定的文档</param>
    protected virtual void DataBind( IHtmlDocument document, object dataContext )
    {
      var bindings = FindBindings( document ).OrderBy( b => b.Priority );

      var context = new BindingContext( this, dataContext );
      bindings.ForAll( b => b.DataBind( context ) );

      document.Elements().ForAll( e => DataBind( e, context ) );
    }

    /// <summary>
    /// 对元素进行数据绑定
    /// </summary>
    /// <param name="element">要进行数据绑定的元素</param>
    protected virtual void DataBind( IHtmlElement element, BindingContext context )
    {
      var bindings = FindBindings( element ).OrderBy( b => b.Priority );

      context.Enter( element );
      bindings.ForAll( b => b.DataBind( context ) );
      element.Elements().ForAll( e => DataBind( e, context ) );
      context.Exit( element );
    }


    /// <summary>
    /// 搜索所有针对文档的 Binding 对象
    /// </summary>
    /// <param name="document">要搜索的文档</param>
    /// <returns>搜索到的 Bingding 对象</returns>
    protected abstract IEnumerable<IBinding> FindBindings( IHtmlDocument document );


    /// <summary>
    /// 搜索所有针对元素的 Binding 对象
    /// </summary>
    /// <param name="document">要搜索的元素</param>
    /// <returns>搜索到的 Bingding 对象</returns>
    protected abstract IEnumerable<IBinding> FindBindings( IHtmlElement element );


    /// <summary>
    /// 获取值转换器
    /// </summary>
    /// <param name="dataObject">数据对象</param>
    /// <param name="targetType">目标值类型</param>
    /// <param name="converterName">转换器名称</param>
    /// <returns>值转换器</returns>
    public virtual IValueConverter GetConverter( object dataObject, Type targetType, string converterName = null )
    {
      return null;
    }

    /// <summary>
    /// 转换值对象到指定类型
    /// </summary>
    /// <typeparam name="T">目标类型</typeparam>
    /// <param name="dataObject">要转换的对象</param>
    /// <param name="converterName">值转换器名称</param>
    /// <returns>转换后的值</returns>
    public object ConvertValue( object dataObject, Type targetType, string converterName = null )
    {
      var converter = GetConverter( dataObject, targetType, converterName );

      if ( converter != null )
        return converter.Convert( dataObject );


      if ( dataObject.GetType().IsSubclassOf( targetType ) )
        return dataObject;

      else
        return null;
    }

    /// <summary>
    /// 获取绑定器
    /// </summary>
    /// <param name="bindingHost">绑定宿主</param>
    /// <param name="binderName">绑定器名称</param>
    /// <returns>绑定器</returns>
    public virtual IValueBinder GetValueBinder( IHtmlDomObject bindingHost, string binderName = null )
    {
      return null;
    }


    /// <summary>
    /// 获取绑定目标
    /// </summary>
    /// <param name="bindingHost">绑定宿主</param>
    /// <returns>绑定目标</returns>
    public virtual IBindingTarget GetTarget( IHtmlDomObject bindingHost )
    {
      var attribute = bindingHost as IHtmlAttribute;

      if ( attribute != null )
      {
        if ( MarkupAttribute( attribute ) )
          return new MarkupAttributeBidningTarget( attribute );

        else
          return new TextAttributeBindingTarget( attribute );
      }

      var element = bindingHost as IHtmlElement;
      if ( element != null )
        return new TextElementBindingTarget( element );

      throw new NotSupportedException();

    }

    private bool MarkupAttribute( IHtmlAttribute attribute )
    {
      return attribute.Document.HtmlSpecification.IsMarkupAttribute( attribute );
    }


    /// <summary>
    /// 在运行时计算数据绑定表达式。
    /// </summary>
    /// <param name="dataContext">表达式根据其进行计算的对象引用。此标识符必须是以页的指定语言表示的有效对象标识符</param>
    /// <param name="path">从 container 对象到要放置在绑定控件属性中的公共属性值的导航路径。此路径必须是以点分隔的属性或字段名称字符串，如 C# 中的 Tables[0].DefaultView.[0].Price 或 Visual Basic 中的 Tables(0).DefaultView.(0).Price</param>
    /// <returns>System.Object 实例，它是数据绑定表达式的计算结果</returns>
    /// <exception cref="System.ArgumentNullException">path 为 null 或修整后变成空字符串</exception>
    public virtual object Eval( object dataContext, string path )
    {
      if ( string.IsNullOrEmpty( path ) )
        throw new ArgumentNullException( "path" );

      return DataBinder.Eval( dataContext, path );
    }




  }


  public static class BindingExtensions
  {
    public static BindingManager BindingManager( this IHtmlDocument document )
    {
      return null;
    }


  }

}
