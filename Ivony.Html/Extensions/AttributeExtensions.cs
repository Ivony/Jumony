using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Ivony.Fluent;
using System.ComponentModel;

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

      return element.Attributes().SingleOrDefault( a => a.Name.EqualsIgnoreCase( name ) );
    }




    /// <summary>
    /// 获取属性值，与 AttributeValue 属性不同，Value 方法在属性对象为null时不会抛出异常
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
    /// 设置属性值
    /// </summary>
    /// <param name="attribute">属性对象</param>
    /// <param name="value">要设置的属性值</param>
    /// <returns>被设置的属性对象</returns>
    public static IHtmlAttribute Value( this IHtmlAttribute attribute, string value )
    {

      if ( attribute == null )
        throw new ArgumentNullException( "attribute" );

      attribute.AttributeValue = value;
      return attribute;
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
      if ( element == null )
        throw new ArgumentNullException( "element" );

      if ( attributeName == null )
        throw new ArgumentNullException( "attributeName" );

      var attribute = element.Attribute( attributeName );

      if ( attribute != null )
        attribute.Remove();

      element.AddAttribute( attributeName, value );


      return element;
    }


    public IEnumerable<IHtmlElement> SetAttribute( this IEnumerable<IHtmlElement> elements, string attributeName, string value )
    {
      if ( elements == null )
        throw new ArgumentNullException( "elements" );

      if ( attributeName == null )
        throw new ArgumentNullException( "attributeName" );

      elements.ForAll( e => e.SetAttribute( attributeName, value );
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
    /// <returns></returns>
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
    /// <param name="replacement">替换字符串</param>
    /// <returns></returns>
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
    /// <returns></returns>
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
    /// 批量设置属性值
    /// </summary>
    /// <param name="elements">要设置属性值的元素</param>
    /// <param name="attributeName">属性名</param>
    /// <returns>属性设置器</returns>
    public static AttributeSetValueSetter SetAttribute( this IEnumerable<IHtmlElement> elements, string attributeName )
    {
      return new AttributeSetValueSetter( elements, attributeName );
    }


    /// <summary>
    /// 批量设置属性值
    /// </summary>
    /// <param name="elements">要设置属性值的元素</param>
    /// <param name="attributeName">属性名</param>
    /// <param name="value">属性值</param>
    /// <returns>设置了属性的元素</returns>
    public static IEnumerable<IHtmlElement> SetAttribute( this IEnumerable<IHtmlElement> elements, string attributeName, string value )
    {
      elements.ForAll( e => e.SetAttribute( attributeName, value ) );

      return elements;
    }


    /// <summary>
    /// 批量属性设置器，提供Value方法方便的批量设置属性值
    /// </summary>
    public class AttributeSetValueSetter
    {
      private readonly IEnumerable<IHtmlElement> _elements;
      private readonly AttributeValueSetter[] _setters;
      //      private readonly string _attributeName;


      internal AttributeSetValueSetter( IEnumerable<IHtmlElement> elements, string attributeName )
      {
        if ( attributeName == null )
          throw new ArgumentNullException( "attributeName" );

        _elements = elements;
        //        _attributeName = attributeName;

        _setters = _elements.Select( e => e.SetAttribute( attributeName ) ).ToArray();
      }

      /// <summary>
      /// 将属性值设置为空
      /// </summary>
      /// <returns>设置属性值的元素</returns>
      public IEnumerable<IHtmlElement> Null()
      {
        _setters.ForAll( s => s.Null() );
        return _elements;
      }

      /// <summary>
      /// 将属性值设置为指定字符串
      /// </summary>
      /// <param name="value">要设置的属性值</param>
      /// <returns>设置属性值的元素</returns>
      public IEnumerable<IHtmlElement> Value( string value )
      {
        _setters.ForAll( s => s.Value( value ) );
        return _elements;
      }

      /// <summary>
      /// 设置属性值
      /// </summary>
      /// <param name="oldValue">要被替换的字符串</param>
      /// <param name="newValue">用于替换的字符串</param>
      /// <returns>被设置的属性对象</returns>
      public IEnumerable<IHtmlElement> Value( string oldValue, string newValue )
      {
        _setters.ForAll( s => s.Value( oldValue, newValue ) );
        return _elements;
      }

      /// <summary>
      /// 设置属性值
      /// </summary>
      /// <param name="pattern">用于在属性值中查找匹配字符串的正则表达式对象</param>
      /// <param name="replacement">替换字符串</param>
      /// <returns></returns>
      public IEnumerable<IHtmlElement> Value( Regex pattern, string replacement )
      {
        _setters.ForAll( s => s.Value( pattern, replacement ) );
        return _elements;
      }

      /// <summary>
      /// 设置属性值
      /// </summary>
      /// <param name="pattern">用于在属性值中查找匹配字符串的正则表达式对象</param>
      /// <param name="evaluator">用于每一步替换的计算函数</param>
      /// <returns></returns>
      public IEnumerable<IHtmlElement> Value( Regex pattern, MatchEvaluator evaluator )
      {
        _setters.ForAll( s => s.Value( pattern, evaluator ) );
        return _elements;
      }

      /// <summary>
      /// 设置属性值
      /// </summary>
      /// <param name="pattern">用于在属性值中查找匹配字符串的正则表达式</param>
      /// <param name="evaluator">用于每一步替换的计算函数</param>
      /// <returns></returns>
      public IEnumerable<IHtmlElement> Value( string pattern, MatchEvaluator evaluator )
      {
        _setters.ForAll( s => s.Value( pattern, evaluator ) );
        return _elements;
      }

      /// <summary>
      /// 设置属性值
      /// </summary>
      /// <param name="evaluator">用于替换属性值的计算函数</param>
      /// <returns></returns>
      public IEnumerable<IHtmlElement> Value( Func<string, string> evaluator )
      {
        _setters.ForAll( s => s.Value( evaluator ) );
        return _elements;
      }

      /// <summary>
      /// 设置属性值
      /// </summary>
      /// <param name="evaluator">用于替换属性值的计算函数</param>
      /// <returns></returns>
      public IEnumerable<IHtmlElement> Value( Func<IHtmlElement, string, string> evaluator )
      {
        _setters.ForAll( s => s.Value( evaluator ) );
        return _elements;
      }

      /// <summary>
      /// 设置属性值
      /// </summary>
      /// <param name="evaluator">用于替换属性值的计算函数</param>
      /// <returns></returns>
      public IEnumerable<IHtmlElement> Value( Func<int, string, string> evaluator )
      {
        _setters.ForAll( ( s, i ) => s.Value( evaluator( i, s.AttributeValue ) ) );
        return _elements;
      }


      /// <summary>
      /// 删除这个属性
      /// </summary>
      /// <returns></returns>
      public IEnumerable<IHtmlElement> Remove()
      {
        _setters.ForAll( ( s, i ) => s.Remove() );
        return _elements;
      }


      /// <summary>
      /// 复制属性值
      /// </summary>
      /// <param name="element">要复制属性值的元素</param>
      /// <returns></returns>
      public IEnumerable<IHtmlElement> From( IHtmlElement element )
      {
        _setters.ForAll( s => s.From( element ) );
        return _elements;
      }

    }

  }
}
