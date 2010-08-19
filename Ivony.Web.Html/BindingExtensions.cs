using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ivony.Web.Html
{

  /// <summary>
  /// 提供协助数据绑定的扩展方法
  /// </summary>
  public static class BindingExtensions
  {

    public static T BindAction<T>( this T element, Action<T> action ) where T : IHtmlElement
    {
      var instance = new HtmlBindingAction<T>()
      {
        Target = element,
        Action = action
      };

      HtmlBindingContext.Current.Action( instance );

      return element;
    }



    /// <summary>
    /// 绑定数据到指定位置
    /// </summary>
    /// <param name="element">要绑定数据的元素</param>
    /// <param name="path">绑定路径</param>
    /// <param name="value">绑定值</param>
    /// <returns>被绑定的元素</returns>
    public static IHtmlElement Bind( this IHtmlElement element, string path, object value )
    {
      if ( value == null )
        throw new ArgumentNullException();

      return Bind( element, path, value, null );
    }

    public static IEnumerable<IHtmlElement> Bind( this IEnumerable<IHtmlElement> elements, string path, object value )
    {
      if ( value == null )
        throw new ArgumentNullException();

      return elements.ForAll( e => e.Bind( path, value ) );
    }

    /// <summary>
    /// 绑定数据到指定位置
    /// </summary>
    /// <param name="element">要绑定数据的元素</param>
    /// <param name="path">绑定路径</param>
    /// <param name="value">绑定值</param>
    /// <param name="format">用于绑定值的格式化字符串</param>
    /// <returns>被绑定的元素</returns>
    public static IHtmlElement Bind( this IHtmlElement element, string path, object value, string format )
    {
      if ( value == null )
        throw new ArgumentNullException();

      return Bind( element, path, value, format, BindingNullBehavior.Ignore );
    }

    public static IEnumerable<IHtmlElement> Bind( this IEnumerable<IHtmlElement> elements, string path, object value, string format )
    {
      if ( value == null )
        throw new ArgumentNullException();

      return elements.ForAll( e => e.Bind( path, value, format ) );
    }

    /// <summary>
    /// 绑定数据到指定位置
    /// </summary>
    /// <param name="element">要绑定数据的元素</param>
    /// <param name="path">绑定路径</param>
    /// <param name="value">绑定值</param>
    /// <param name="nullBehavior">定义当绑定值为空的行为</param>
    /// <returns>被绑定的元素</returns>
    public static IHtmlElement Bind( this IHtmlElement element, string path, object value, BindingNullBehavior nullBehavior )
    {
      return Bind( element, path, value, null, nullBehavior );
    }

    public static IEnumerable<IHtmlElement> Bind( this IEnumerable<IHtmlElement> elements, string path, object value, BindingNullBehavior nullBehavior )
    {
      return elements.ForAll( e => e.Bind( path, value, null, nullBehavior ) );
    }

    /// <summary>
    /// 绑定数据到指定位置
    /// </summary>
    /// <param name="element">要绑定数据的元素</param>
    /// <param name="path">绑定路径</param>
    /// <param name="value">绑定值</param>
    /// <param name="format">用于绑定值的格式化字符串</param>
    /// <param name="nullBehavior">定义当绑定值为空的行为</param>
    /// <returns>被绑定的元素</returns>
    public static IHtmlElement Bind( this IHtmlElement element, string path, object value, string format, BindingNullBehavior nullBehavior )
    {
      return Bind( element, HtmlBindingContext.Current, path, value, format, nullBehavior );
    }

    public static IEnumerable<IHtmlElement> Bind( this IEnumerable<IHtmlElement> elements, string path, object value, string format, BindingNullBehavior nullBehavior )
    {
      return elements.ForAll( e => e.Bind( path, value, format, nullBehavior ) );
    }

    /// <summary>
    /// 绑定数据到指定位置
    /// </summary>
    /// <param name="element">要绑定数据的元素</param>
    /// <param name="context">要绑定到的绑定上下文</param>
    /// <param name="path">绑定路径</param>
    /// <param name="value">绑定值</param>
    /// <param name="format">用于绑定值的格式化字符串</param>
    /// <param name="nullBehavior">定义当绑定值为空的行为</param>
    /// <returns>被绑定的元素</returns>
    public static IHtmlElement Bind( this IHtmlElement element, HtmlBindingContext context, string path, object value, string format, BindingNullBehavior nullBehavior )
    {
      if ( format == null )
        format = "{0}";

      string _value = null;
      if ( value != null )
        _value = string.Format( format, value );

      element.BindCore( context, path, _value, nullBehavior );
      return element;
    }


    /// <summary>
    /// 从数据源绑定数据列表到每一个元素的 DataContext
    /// </summary>
    /// <typeparam name="T">数据项类型</typeparam>
    /// <param name="elements">需要绑定数据的元素列表</param>
    /// <param name="dataSource">数据源</param>
    /// <returns>绑定后的元素列表</returns>
    public static IEnumerable<IHtmlElement> BindContextFrom<T>( this IEnumerable<IHtmlElement> elements, IEnumerable<T> dataSource )
    {

      using ( var sourceIterator = dataSource.GetEnumerator() )
      {
        using ( var targetIterator = elements.GetEnumerator() )
        {
          while ( sourceIterator.MoveNext() && targetIterator.MoveNext() )
            HtmlBindingContext.Current.DataContext( targetIterator.Current, sourceIterator.Current );
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
    /// <returns></returns>
    public static IEnumerable<IHtmlElement> BindContextFrom<T>( this IEnumerable<IHtmlElement> elements, IEnumerable<T> dataSource, T defaultValue )
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

            HtmlBindingContext.Current.DataContext( targetItem, dataItem );

          }

        }
      }

      return elements;
    }


    public static IHtmlElement ContextBind( this IHtmlElement element, string bindPath, string sourcePath )
    {
      return Bind( element, bindPath, Eval( element.DataContext(), sourcePath ), "{0}" );
    }

    public static IHtmlElement ContextBind( this IHtmlElement element, string bindPath, string sourcePath, string format )
    {
      Bind( element, bindPath, Eval( HtmlBindingContext.Current.DataContext( element ), sourcePath ), format );

      return element;
    }


    public static object DataContext( this IHtmlContainer container )
    {
      return HtmlBindingContext.Current.DataContext( container );
    }

    public static IHtmlContainer DataContext( this IHtmlContainer container, object dataContext )
    {
      HtmlBindingContext.Current.DataContext( container, dataContext );
      return container;
    }




    public static object Eval( object dataItem, string expression )
    {
      return System.Web.UI.DataBinder.Eval( dataItem, expression );
    }

  }
}
