using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Ivony.Fluent;

namespace Ivony.Html
{

  /// <summary>
  /// 提供协助操作 Attribute 的扩展方法
  /// </summary>
  public static class AttributeExtensions
  {

    /// <summary>
    /// 获取指定名称的属性对象
    /// </summary>
    /// <param name="element">元素</param>
    /// <param name="name">属性名</param>
    /// <returns>属性对象，如果没找到，则返回null</returns>
    /// <remarks>
    /// 如果有多个同名的属性，此方法将报错。
    /// </remarks>
    public static IHtmlAttribute Attribute( this IHtmlElement element, string name )
    {
      if ( element == null )
        throw new ArgumentNullException( "element" );

      var attributes = element.Attributes();

      /*
      //即使按字典检索，由于大部分元素属性都很少，也不见得有性能提升
      var dictionary = attributes as IDictionary<string, IHtmlAttribute>;

      if ( dictionary != null )
        return dictionary[name];
      */

      var attributeCollection = attributes as IHtmlAttributeCollection;
      if ( attributeCollection != null )
        return attributeCollection.Get( name );

      return attributes.FirstOrDefault( a => a.Name.EqualsIgnoreCase( name ) );
    }




    /// <summary>
    /// 获取属性值，与 AttributeValue 属性不同，Value 方法在属性对象为 null 时不会抛出异常
    /// </summary>
    /// <param name="attribute">属性对象</param>
    /// <returns>属性值，如果属性对象为null，则返回null</returns>
    public static string Value( this IHtmlAttribute attribute )
    {
      if ( attribute == null )
        return null;

      return attribute.AttributeValue;
    }


    /// <summary>
    /// 设置属性的值，这会产生一个新的属性，并返回
    /// </summary>
    /// <param name="attribute">要设置值的属性</param>
    /// <param name="value">设置的值</param>
    /// <returns>所创建的新属性</returns>
    public static IHtmlAttribute SetValue( this IHtmlAttribute attribute, string value )
    {
      if ( attribute == null )
        throw new ArgumentNullException( "attribute" );

      var element = attribute.Element;
      var name = attribute.Name;

      if ( element == null )
        throw new InvalidOperationException();

      attribute.Remove();

      return element.AddAttribute( name, value );
    }



    /// <summary>
    /// 设置属性为空值
    /// </summary>
    /// <param name="element">要设置属性值的元素</param>
    /// <param name="attributeName">属性名</param>
    /// <returns>设置了属性的元素</returns>
    public static IHtmlElement SetAttribute( this IHtmlElement element, string attributeName )
    {
      return SetAttribute( element, attributeName, value: null );
    }


    /// <summary>
    /// 设置属性值
    /// </summary>
    /// <param name="element">要设置属性值的元素</param>
    /// <param name="attributeName">属性名</param>
    /// <param name="value">属性值</param>
    /// <returns>设置了属性的元素</returns>
    public static IHtmlElement SetAttribute( this IHtmlElement element, string attributeName, string value )
    {
      IHtmlAttribute attribute;
      return SetAttribute( element, attributeName, value, out attribute );
    }


    /// <summary>
    /// 设置属性值
    /// </summary>
    /// <param name="element">要设置属性值的元素</param>
    /// <param name="attributeName">属性名</param>
    /// <param name="value">属性值</param>
    /// <param name="attribute">设置好的属性</param>
    /// <returns>设置了属性的元素</returns>
    public static IHtmlElement SetAttribute( this IHtmlElement element, string attributeName, string value, out IHtmlAttribute attribute )
    {
      if ( element == null )
        throw new ArgumentNullException( "element" );

      if ( attributeName == null )
        throw new ArgumentNullException( "attributeName" );

      lock ( element.SyncRoot )
      {
        var _attribute = element.Attribute( attributeName );

        if ( _attribute != null )
          _attribute.Remove();

        attribute = element.AddAttribute( attributeName, value );
      }

      return element;
    }


    /// <summary>
    /// 设置属性值
    /// </summary>
    /// <param name="element">要设置属性值的元素</param>
    /// <param name="attributeName">属性名</param>
    /// <param name="oldValue">要被替换的字符串</param>
    /// <param name="newValue">用于替换的字符串</param>
    /// <returns>设置了属性的元素</returns>
    public static IHtmlElement SetAttribute( this IHtmlElement element, string attributeName, string oldValue, string newValue )
    {
      if ( element == null )
        throw new ArgumentNullException( "element" );

      if ( attributeName == null )
        throw new ArgumentNullException( "attributeName" );

      if ( oldValue == null )
        throw new ArgumentNullException( "oldValue" );


      return SetAttribute( element, attributeName, value => value.Replace( oldValue, newValue ) );
    }


    /// <summary>
    /// 设置属性值
    /// </summary>
    /// <param name="element">要设置属性的元素</param>
    /// <param name="attributeName">属性名</param>
    /// <param name="pattern">用于在属性值中查找匹配字符串的正则表达式对象</param>
    /// <param name="replacement">替换字符串</param>
    /// <returns>返回元素便于链式调用</returns>
    public static IHtmlElement SetAttribute( this IHtmlElement element, string attributeName, Regex pattern, string replacement )
    {
      if ( element == null )
        throw new ArgumentNullException( "element" );

      if ( attributeName == null )
        throw new ArgumentNullException( "attributeName" );

      if ( pattern == null )
        throw new ArgumentNullException( "pattern" );

      return SetAttribute( element, attributeName, value => pattern.Replace( value, replacement ) );
    }


    /// <summary>
    /// 设置属性值
    /// </summary>
    /// <param name="element">要设置属性的元素</param>
    /// <param name="attributeName">属性名</param>
    /// <param name="pattern">用于在属性值中查找匹配字符串的正则表达式对象</param>
    /// <param name="evaluator">替换字符串</param>
    /// <returns>返回元素便于链式调用</returns>
    public static IHtmlElement SetAttribute( this IHtmlElement element, string attributeName, Regex pattern, MatchEvaluator evaluator )
    {
      if ( element == null )
        throw new ArgumentNullException( "element" );

      if ( attributeName == null )
        throw new ArgumentNullException( "attributeName" );

      if ( pattern == null )
        throw new ArgumentNullException( "pattern" );

      return SetAttribute( element, attributeName, value => pattern.Replace( value, evaluator ) );
    }


    /// <summary>
    /// 设置属性值
    /// </summary>
    /// <param name="element">要设置属性的元素</param>
    /// <param name="attributeName">属性名</param>
    /// <param name="evaluator">用于替换属性值的计算函数</param>
    /// <returns>返回元素便于链式调用</returns>
    public static IHtmlElement SetAttribute( this IHtmlElement element, string attributeName, Func<string, string> evaluator )
    {
      if ( element == null )
        throw new ArgumentNullException( "element" );

      if ( evaluator == null )
        throw new ArgumentNullException( "evaluator" );


      var attribute = element.Attribute( attributeName );
      var value = attribute.Value();

      if ( value == null )
        return element;

      value = evaluator( value );
      attribute.Remove();

      element.AddAttribute( attributeName, value );

      return element;
    }




    /// <summary>
    /// 设置属性值
    /// </summary>
    /// <param name="elements">要设置属性值的元素列表</param>
    /// <param name="attributeName">属性名</param>
    /// <param name="value">属性值</param>
    /// <returns>返回元素列表便于链式调用</returns>
    public static IEnumerable<IHtmlElement> SetAttribute( this IEnumerable<IHtmlElement> elements, string attributeName, string value )
    {
      if ( elements == null )
        throw new ArgumentNullException( "elements" );

      if ( attributeName == null )
        throw new ArgumentNullException( "attributeName" );

      elements.NotNull().ForAll( e => e.SetAttribute( attributeName, value ) );

      return elements;
    }


    /// <summary>
    /// 设置属性值
    /// </summary>
    /// <param name="elements">要设置属性值的元素列表</param>
    /// <param name="attributeName">属性名</param>
    /// <param name="oldValue">要被替换的字符串</param>
    /// <param name="newValue">用于替换的字符串</param>
    /// <returns>返回元素列表便于链式调用</returns>
    public static IEnumerable<IHtmlElement> SetAttribute( this IEnumerable<IHtmlElement> elements, string attributeName, string oldValue, string newValue )
    {
      if ( elements == null )
        throw new ArgumentNullException( "elements" );

      if ( attributeName == null )
        throw new ArgumentNullException( "attributeName" );

      if ( oldValue == null )
        throw new ArgumentNullException( "oldValue" );


      elements.NotNull().ForAll( e => e.SetAttribute( attributeName, value => value.Replace( oldValue, newValue ) ) );

      return elements;
    }


    /// <summary>
    /// 设置属性值
    /// </summary>
    /// <param name="elements">要设置属性值的元素列表</param>
    /// <param name="attributeName">属性名</param>
    /// <param name="pattern">用于在属性值中查找匹配字符串的正则表达式对象</param>
    /// <param name="replacement">替换字符串</param>
    /// <returns>返回元素列表便于链式调用</returns>
    public static IEnumerable<IHtmlElement> SetAttribute( this IEnumerable<IHtmlElement> elements, string attributeName, Regex pattern, string replacement )
    {
      if ( elements == null )
        throw new ArgumentNullException( "elements" );

      if ( attributeName == null )
        throw new ArgumentNullException( "attributeName" );

      if ( pattern == null )
        throw new ArgumentNullException( "pattern" );

      elements.NotNull().ForAll( e => e.SetAttribute( attributeName, value => pattern.Replace( value, replacement ) ) );

      return elements;
    }


    /// <summary>
    /// 设置属性值
    /// </summary>
    /// <param name="elements">要设置属性值的元素列表</param>
    /// <param name="attributeName">属性名</param>
    /// <param name="pattern">用于在属性值中查找匹配字符串的正则表达式对象</param>
    /// <param name="evaluator">替换字符串</param>
    /// <returns>返回元素列表便于链式调用</returns>
    public static IEnumerable<IHtmlElement> SetAttribute( this IEnumerable<IHtmlElement> elements, string attributeName, Regex pattern, MatchEvaluator evaluator )
    {
      if ( elements == null )
        throw new ArgumentNullException( "elements" );

      if ( attributeName == null )
        throw new ArgumentNullException( "attributeName" );

      if ( pattern == null )
        throw new ArgumentNullException( "pattern" );

      elements.NotNull().ForAll( e => e.SetAttribute( attributeName, value => pattern.Replace( value, evaluator ) ) );

      return elements;
    }


    /// <summary>
    /// 设置属性值
    /// </summary>
    /// <param name="elements">要设置属性值的元素列表</param>
    /// <param name="attributeName">属性名</param>
    /// <param name="evaluator">用于替换属性值的计算函数</param>
    /// <returns>返回元素列表便于链式调用</returns>
    public static IEnumerable<IHtmlElement> SetAttribute( this IEnumerable<IHtmlElement> elements, string attributeName, Func<string, string> evaluator )
    {
      if ( elements == null )
        throw new ArgumentNullException( "elements" );

      if ( elements == null )
        throw new ArgumentNullException( "attributeName" );

      if ( evaluator == null )
        throw new ArgumentNullException( "evaluator" );


      elements.NotNull().ForAll( e => e.SetAttribute( attributeName, evaluator ) );

      return elements;

    }


    /// <summary>
    /// 设置属性值
    /// </summary>
    /// <param name="elements">要设置属性值的元素列表</param>
    /// <param name="attributeName">属性名</param>
    /// <param name="evaluator">用于替换属性值的计算函数</param>
    /// <returns>返回元素列表便于链式调用</returns>
    public static IEnumerable<IHtmlElement> SetAttribute( this IEnumerable<IHtmlElement> elements, string attributeName, Func<int, string, string> evaluator )
    {
      if ( elements == null )
        throw new ArgumentNullException( "elements" );

      if ( elements == null )
        throw new ArgumentNullException( "attributeName" );

      if ( evaluator == null )
        throw new ArgumentNullException( "evaluator" );


      elements.ForAll( ( e, index ) => e.SetAttribute( attributeName, value => evaluator( index, value ) ) );

      return elements;

    }


    /// <summary>
    /// 移除指定的属性
    /// </summary>
    /// <param name="element">要移除属性的元素</param>
    /// <param name="attributeName">要移除的属性名称</param>
    /// <returns>返回元素以便于链式调用</returns>
    public static T RemoveAttribute<T>( this T element, string attributeName ) where T : IHtmlElement
    {
      if ( element == null )
        throw new ArgumentNullException( "element" );

      if ( attributeName == null )
        throw new ArgumentNullException( "attributeName" );

      lock ( element.SyncRoot )
      {

        var attribute = element.Attribute( attributeName );

        if ( attribute != null )
          attribute.Remove();

        return element;
      }
    }

    /// <summary>
    /// 移除指定的属性
    /// </summary>
    /// <param name="elements">要移除属性的元素列表</param>
    /// <param name="attributeName">要移除的属性名称</param>
    /// <returns>返回元素列表便于链式调用</returns>
    public static IEnumerable<IHtmlElement> RemoveAttribute( this IEnumerable<IHtmlElement> elements, string attributeName )
    {
      if ( elements == null )
        throw new ArgumentNullException( "elements" );

      if ( attributeName == null )
        throw new ArgumentNullException( "attributeName" );

      return elements.ForAll( e => e.RemoveAttribute( attributeName ) );
    }
  }
}
