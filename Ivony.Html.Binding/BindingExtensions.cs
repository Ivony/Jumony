using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ivony.Fluent;
using System.ComponentModel;
using Regex = System.Text.RegularExpressions.Regex;

using Ivony.Html.Styles;

namespace Ivony.Html.Binding
{

  /// <summary>
  /// 提供协助数据绑定的扩展方法
  /// </summary>
  public static class BindingExtensions
  {

    /// <summary>
    /// 将一个绑定操作放到绑定上下文
    /// </summary>
    /// <param name="element">要执行绑定操作的元素</param>
    /// <param name="action">要对元素执行的绑定操作</param>
    /// <returns>被执行绑定操作的元素</returns>
    public static IHtmlElement BindAction( this IHtmlElement element, Action<IHtmlElement> action )
    {
      var instance = new HtmlBindingAction<IHtmlElement>()
      {
        Target = element,
        Action = action
      };

      BindingContext.Action( instance );

      return element;
    }


    /// <summary>
    /// 将绑定操作放到绑定上下文
    /// </summary>
    /// <param name="elements">要执行绑定操作的元素列表</param>
    /// <param name="action">要对元素执行的绑定操作</param>
    /// <returns>被执行绑定操作的元素列表</returns>
    public static IEnumerable<IHtmlElement> BindAction( this IEnumerable<IHtmlElement> elements, Action<IHtmlElement> action )
    {
      return elements.ForAll( e => BindAction( e, action ) );
    }




    #region Bind( path, value )



    /// <summary>
    /// 绑定数据到指定位置，集合版本
    /// </summary>
    /// <param name="elements">要绑定数据的元素</param>
    /// <param name="path">绑定路径</param>
    /// <param name="evaluator">用于计算绑定值的函数</param>
    /// <returns>被绑定的元素</returns>
    public static IEnumerable<IHtmlElement> Bind( this IEnumerable<IHtmlElement> elements, string path, Func<IHtmlElement, object> evaluator )
    {
      if ( evaluator == null )
        throw new ArgumentNullException( "evaluator" );

      return elements.ForAll( e => e.Bind( path, evaluator( e ) ) );
    }


    /// <summary>
    /// 绑定数据到指定位置
    /// </summary>
    /// <param name="element">要绑定数据的元素</param>
    /// <param name="path">绑定路径</param>
    /// <param name="evaluator">用于计算绑定值的函数</param>
    /// <returns>被绑定的元素</returns>
    public static IHtmlElement Bind( this IHtmlElement element, string path, Func<IHtmlElement, object> evaluator )
    {
      if ( evaluator == null )
        throw new ArgumentNullException( "evaluator" );

      return Bind( element, path, evaluator( element ) );
    }


    /// <summary>
    /// 绑定数据到指定位置，集合版本
    /// </summary>
    /// <param name="elements">要绑定数据的元素</param>
    /// <param name="path">绑定路径</param>
    /// <param name="data">绑定值</param>
    /// <returns>被绑定的元素</returns>
    public static IEnumerable<IHtmlElement> Bind( this IEnumerable<IHtmlElement> elements, string path, object data )
    {
      if ( data == null )
        throw new ArgumentNullException( "data" );

      return elements.ForAll( e => e.Bind( path, data ) );
    }




    /// <summary>
    /// 绑定数据到指定位置
    /// </summary>
    /// <param name="element">要绑定数据的元素</param>
    /// <param name="path">绑定路径</param>
    /// <param name="data">绑定值</param>
    /// <returns>被绑定的元素</returns>
    public static IHtmlElement Bind( this IHtmlElement element, string path, object data )
    {
      if ( data == null )
        throw new ArgumentNullException( "data" );

      BindCore( element, path, data.ToString(), BindingNullBehavior.Ignore );

      return element;
    }


    #endregion




    #region Bind( path, data, nullBehavior )

    /// <summary>
    /// 绑定数据到指定位置，集合版本
    /// </summary>
    /// <param name="elements">要绑定数据的元素</param>
    /// <param name="path">绑定路径</param>
    /// <param name="evaluator">用于计算绑定值的函数</param>
    /// <param name="nullBehavior">定义当绑定值为空的行为</param>
    /// <returns>被绑定的元素</returns>
    public static IEnumerable<IHtmlElement> Bind( this IEnumerable<IHtmlElement> elements, string path, Func<IHtmlElement, object> evaluator, BindingNullBehavior nullBehavior )
    {
      if ( evaluator == null )
        throw new ArgumentNullException( "evaluator" );

      return elements.ForAll( e => e.Bind( path, evaluator( e ), nullBehavior ) );
    }


    /// <summary>
    /// 绑定数据到指定位置
    /// </summary>
    /// <param name="element">要绑定数据的元素</param>
    /// <param name="path">绑定路径</param>
    /// <param name="evaluator">用于计算绑定值的函数</param>
    /// <param name="nullBehavior">定义当绑定值为空的行为</param>
    /// <returns>被绑定的元素</returns>
    public static IHtmlElement Bind( this IHtmlElement element, string path, Func<IHtmlElement, object> evaluator, BindingNullBehavior nullBehavior )
    {
      if ( evaluator == null )
        throw new ArgumentNullException( "evaluator" );

      return Bind( element, path, evaluator( element ), nullBehavior );
    }


    /// <summary>
    /// 绑定数据到指定位置，集合版本
    /// </summary>
    /// <param name="elements">要绑定数据的元素</param>
    /// <param name="path">绑定路径</param>
    /// <param name="data">绑定值</param>
    /// <param name="nullBehavior">定义当绑定值为空的行为</param>
    /// <returns>被绑定的元素</returns>
    public static IEnumerable<IHtmlElement> Bind( this IEnumerable<IHtmlElement> elements, string path, object data, BindingNullBehavior nullBehavior )
    {
      return elements.ForAll( e => e.Bind( path, data, nullBehavior ) );
    }


    /// <summary>
    /// 绑定数据到指定位置
    /// </summary>
    /// <param name="element">要绑定数据的元素</param>
    /// <param name="path">绑定路径</param>
    /// <param name="data">绑定值</param>
    /// <param name="nullBehavior">定义当绑定值为空的行为</param>
    /// <returns>被绑定的元素</returns>
    public static IHtmlElement Bind( this IHtmlElement element, string path, object data, BindingNullBehavior nullBehavior )
    {
      BindCore( element, path, data, nullBehavior );

      return element;
    }

    #endregion




    /// <summary>
    /// 格式化值
    /// </summary>
    /// <param name="format">格式化表达式</param>
    /// <param name="value">要被格式化的值</param>
    /// <returns>值格式化后的形式</returns>
    private static string FormatValue( string format, object value )
    {

      if ( value == null )
        return null;

      if ( format == null )
        return string.Format( format, value );
      else
        return value.ToString();
    }





    private static readonly Regex attributePathRegex = new Regex( @"@(?<name>\w+)" );

    /// <summary>
    /// 提供数据绑定的核心方法。
    /// </summary>
    /// <param name="context">绑定上下文</param>
    /// <param name="path">绑定路径</param>
    /// <param name="value">绑定值</param>
    /// <param name="nullBehavior">为空时执行的操作</param>
    private static void BindCore( this IHtmlElement element, string path, object data, BindingNullBehavior nullBehavior )
    {

      string value = GetValue( element, path, data );


      if ( value == null )
      {
        switch ( nullBehavior )
        {
          case BindingNullBehavior.Ignore:
            break;
          case BindingNullBehavior.Hidden:
            BindingContext.Action( element, e => e.Style().Set( "visibility", "hidden" ) );
            break;
          case BindingNullBehavior.Remove:
            BindingContext.Action( element, e => e.Remove() );
            break;
          case BindingNullBehavior.DisplayNone:
            BindingContext.Action( element, e => e.Style().Set( "display", "none" ) );
            break;
          default:
            throw new NotSupportedException();
        }

        return;
      }

      var attributeMatch = attributePathRegex.Match( path );
      if ( attributeMatch.Success )
      {
        string attributeName = attributeMatch.Groups["name"].Value;
        BindingContext.Action( element, e => element.SetAttribute( attributeName ).Value( value ) );
      }

      if ( path == "@:text" )
        BindingContext.Action( element, e => e.InnerText( value ) );


      if ( path == "@:html" )
        BindingContext.Action( element, e => e.InnerHtml( value ) );

      return;
    }

    /// <summary>
    /// 将对象转换为字符串表达形式
    /// </summary>
    /// <param name="data">数据对象</param>
    /// <param name="element">绑定目标元素</param>
    /// <param name="path">绑定目标路径</param>
    /// <returns></returns>
    private static string GetValue( IHtmlElement element, string path, object data )
    {
      throw new NotImplementedException();
    }



    /// <summary>
    /// 从数据源绑定数据列表到每一个元素的 DataContext
    /// </summary>
    /// <typeparam name="T">数据项类型</typeparam>
    /// <param name="elements">需要绑定数据的元素列表</param>
    /// <param name="dataSource">数据源</param>
    /// <returns>绑定后的元素列表</returns>
    public static IEnumerable<IHtmlElement> DataFrom<T>( this IEnumerable<IHtmlElement> elements, IEnumerable<T> dataSource )
    {

      using ( var sourceIterator = dataSource.GetEnumerator() )
      {
        using ( var targetIterator = elements.GetEnumerator() )
        {
          while ( sourceIterator.MoveNext() && targetIterator.MoveNext() )
            BindingContext.Current.SetDataContext( targetIterator.Current, sourceIterator.Current );
        }
      }

      return elements;
    }

    /// <summary>
    /// 从数据源绑定数据列表到每一个元素的 DataContext
    /// </summary>
    /// <typeparam name="T">数据项类型</typeparam>
    /// <param name="elements">需要绑定数据的元素列表</param>
    /// <param name="dataSource">数据源</param>
    /// <param name="defaultValue">当数据源中的数据不足时，应采用的默认值</param>
    /// <returns>已绑定数据的元素列表</returns>
    public static IEnumerable<IHtmlElement> DataFrom<T>( this IEnumerable<IHtmlElement> elements, IEnumerable<T> dataSource, T defaultValue )
    {

      using ( var sourceIterator = dataSource.GetEnumerator() )
      {
        using ( var targetIterator = elements.GetEnumerator() )
        {
          bool sourceEnded = false;

          while ( targetIterator.MoveNext() )
          {

            if ( !sourceEnded )
              sourceEnded = !sourceIterator.MoveNext();


            var dataItem = sourceEnded ? defaultValue : sourceIterator.Current;
            var targetItem = targetIterator.Current;

            BindingContext.Current.SetDataContext( targetItem, dataItem );

          }

        }
      }

      return elements;
    }


    /// <summary>
    /// 从 DataContext 绑定数据
    /// </summary>
    /// <param name="element">绑定数据的对象</param>
    /// <param name="bindPath">绑定路径</param>
    /// <param name="sourcePath">数据源路径</param>
    /// <returns></returns>
    public static IHtmlElement DataBind( this IHtmlElement element, string bindPath, string sourcePath )
    {
      if ( sourcePath == null )
        throw new ArgumentNullException( "sourcePath" );

      return DataBind( element, bindPath, sourcePath, null );
    }

    /// <summary>
    /// 从 DataContext 绑定数据
    /// </summary>
    /// <param name="element">绑定数据的对象</param>
    /// <param name="bindPath">绑定路径</param>
    /// <param name="sourcePath">数据源路径</param>
    /// <param name="format">格式化字符串</param>
    /// <returns></returns>
    public static IHtmlElement DataBind( this IHtmlElement element, string bindPath, string sourcePath, string format )
    {
      if ( sourcePath == null )
        throw new ArgumentNullException( "sourcePath" );

      return DataBind( element, bindPath, sourcePath, format, null );
    }

    /// <summary>
    /// 从 DataContext 绑定数据
    /// </summary>
    /// <param name="element">绑定数据的对象</param>
    /// <param name="bindPath">绑定路径</param>
    /// <param name="sourcePath">数据源路径</param>
    /// <param name="format">格式化字符串</param>
    /// <param name="defaultValue">如果找不到数据，所要使用的默认值</param>
    /// <returns></returns>
    public static IHtmlElement DataBind( this IHtmlElement element, string bindPath, string sourcePath, string format, object defaultValue )
    {
      if ( sourcePath == null )
        throw new ArgumentNullException( "sourcePath" );

      if ( format == null )
        format = "{0}";

      var context = BindingContext.Current;

      var value = Eval( element.Data(), sourcePath );
      if ( value == null )
        value = defaultValue;

      var str = string.Format( format, value );

      BindCore( element, bindPath, str, BindingNullBehavior.Ignore );

      return element;
    }



    public static IEnumerable<IHtmlElement> DataBind( this IEnumerable<IHtmlElement> elements, string bindPath, string sourcePath )
    {
      elements.ForAll( e => DataBind( e, bindPath, sourcePath ) );
      return elements;
    }

    public static IEnumerable<IHtmlElement> DataBind( this IEnumerable<IHtmlElement> elements, string bindPath, string sourcePath, string format )
    {
      elements.ForAll( e => DataBind( e, bindPath, sourcePath, format ) );
      return elements;
    }

    public static IEnumerable<IHtmlElement> DataBind( this IEnumerable<IHtmlElement> elements, string bindPath, string sourcePath, string format, object defaultValue )
    {
      elements.ForAll( e => DataBind( e, bindPath, sourcePath, format, defaultValue ) );
      return elements;
    }

    /// <summary>
    /// 获取 DataContext
    /// </summary>
    /// <param name="container">要获取 DataContext 的节点</param>
    /// <returns></returns>
    public static object Data( this IHtmlNode container )
    {
      return BindingContext.Current.GetDataContext( container );
    }

    /// <summary>
    /// 设置 DataContext
    /// </summary>
    /// <param name="element">要设置 DataContext 的元素</param>
    /// <param name="dataContext">要设置的数据</param>
    /// <returns></returns>
    public static TElement Data<TElement>( this TElement element, object dataContext ) where TElement : IHtmlElement
    {
      BindingContext.Current.SetDataContext( element, dataContext );
      return element;
    }




    public static object Eval( object dataItem, string expression )
    {
      return System.Web.UI.DataBinder.Eval( dataItem, expression );
    }

  }
}
