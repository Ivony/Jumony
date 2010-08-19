using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Ivony.Fluent;

namespace Ivony.Web.Html
{

  /// <summary>
  /// 提供协助操作Attribute的扩展方法
  /// </summary>
  public static class AttributeExtensions
  {

    /// <summary>
    /// 获取指定名称的属性对象
    /// </summary>
    /// <param name="element">元素</param>
    /// <param name="name">属性名</param>
    /// <returns>属性对象，如果没找到，则返回null</returns>
    public static IHtmlAttribute Attribute( this IHtmlElement element, string name )
    {
      return element.Attributes().Where( a => string.Equals( a.Name, name, StringComparison.InvariantCultureIgnoreCase ) ).FirstOrDefault();
    }



    /// <summary>
    /// 获取指定名称的属性对象
    /// </summary>
    /// <param name="element">元素</param>
    /// <param name="name">属性名</param>
    /// <param name="defaultValue">如果属性没找到，则为属性设置默认值</param>
    /// <returns>属性对象</returns>
    public static IHtmlAttribute Attribute( this IHtmlElement element, string name, string defaultValue )
    {
      var attribute = element.Attributes().Where( a => string.Equals( a.Name, name, StringComparison.InvariantCultureIgnoreCase ) ).FirstOrDefault();
      if ( attribute == null )
      {
        attribute = element.AddAttribute( name );
        attribute.Value = defaultValue;
      }


      return attribute;
    }


    /// <summary>
    /// 获取属性值，与Value属性不同，Value方法在属性对象为null时不会抛出异常
    /// </summary>
    /// <param name="attribute">属性对象</param>
    /// <returns>属性值，如果属性对象为null，则返回null</returns>
    public static string Value( this IHtmlAttribute attribute )
    {
      if ( attribute == null )
        return null;

      return attribute.Value;
    }


    /// <summary>
    /// 设置属性值
    /// </summary>
    /// <param name="attribute">属性对象</param>
    /// <param name="value">要设置的属性值</param>
    /// <returns>被设置的属性对象</returns>
    public static IHtmlAttribute Value( this IHtmlAttribute attribute, string value )
    {
      attribute.Value = value;
      return attribute;
    }


    /// <summary>
    /// 设置属性值
    /// </summary>
    /// <param name="attribute">属性对象</param>
    /// <param name="oldValue">要被替换的字符串</param>
    /// <param name="newValue">用于替换的字符串</param>
    /// <returns>被设置的属性对象</returns>
    public static IHtmlAttribute Replace( this IHtmlAttribute attribute, string oldValue, string newValue )
    {
      attribute.Value = attribute.Value.Replace( oldValue, newValue );
      return attribute;
    }

    /// <summary>
    /// 设置属性值
    /// </summary>
    /// <param name="attribute">属性对象</param>
    /// <param name="pattern">用于在属性值中查找匹配字符串的正则表达式对象</param>
    /// <param name="replacement">替换字符串</param>
    /// <returns></returns>
    public static IHtmlAttribute Replace( this IHtmlAttribute attribute, Regex pattern, string replacement )
    {
      attribute.Value = pattern.Replace( attribute.Value, replacement );
      return attribute;
    }

    /// <summary>
    /// 设置属性值
    /// </summary>
    /// <param name="attribute">属性对象</param>
    /// <param name="pattern">用于在属性值中查找匹配字符串的正则表达式对象</param>
    /// <param name="evaluator">用于每一步替换的计算函数</param>
    /// <returns></returns>
    public static IHtmlAttribute Replace( this IHtmlAttribute attribute, Regex pattern, MatchEvaluator evaluator )
    {
      attribute.Value = pattern.Replace( attribute.Value, evaluator );
      return attribute;
    }

    /// <summary>
    /// 设置属性值
    /// </summary>
    /// <param name="attribute">属性对象</param>
    /// <param name="pattern">用于在属性值中查找匹配字符串的正则表达式</param>
    /// <param name="evaluator">用于每一步替换的计算函数</param>
    /// <returns></returns>
    public static IHtmlAttribute Replace( this IHtmlAttribute attribute, string pattern, MatchEvaluator evaluator )
    {
      attribute.Value = Regex.Replace( attribute.Value, pattern, evaluator );
      return attribute;
    }

    /// <summary>
    /// 设置属性值
    /// </summary>
    /// <param name="attribute">属性对象</param>
    /// <param name="evaluator">用于替换属性值的计算函数</param>
    /// <returns></returns>
    public static IHtmlAttribute Replace( this IHtmlAttribute attribute, Func<string, string> evaluator )
    {
      attribute.Value = evaluator( attribute.Value );
      return attribute;
    }



    public static string AttributeValue( this IHtmlElement element, string name )
    {
      var attribute = element.Attribute( name );
      if ( attribute == null )
        return null;

      return attribute.Value;
    }



    /// <summary>
    /// 设置属性值
    /// </summary>
    /// <param name="element">要设置属性值的元素</param>
    /// <param name="attributeName">属性名</param>
    /// <returns>属性设置器</returns>
    public static AttributeValueSetter SetAttribute( this IHtmlElement element, string attributeName )
    {
      return new AttributeValueSetter( element, attributeName );
    }


    /// <summary>
    /// 属性设置器，提供Value和Replace方法方便的设置属性值
    /// </summary>
    public class AttributeValueSetter
    {
      private IHtmlElement _element;
      private string _attributeName;

      private IHtmlAttribute attribute;


      internal AttributeValueSetter( IHtmlElement element, string attributeName )
      {
        if ( attributeName == null )
          throw new ArgumentNullException( "attributeName" );

        _element = element;
        _attributeName = attributeName;


        attribute = _element.Attribute( attributeName );
        if ( attribute == null )
          attribute = _element.AddAttribute( attributeName );
      }

      /// <summary>
      /// 将属性值设置为空
      /// </summary>
      /// <returns>设置属性值的元素</returns>
      public IHtmlElement Value()
      {
        attribute.Value = null;
        return _element;
      }

      /// <summary>
      /// 将属性值设置为指定字符串
      /// </summary>
      /// <param name="value">要设置的属性值</param>
      /// <returns>设置属性值的元素</returns>
      public IHtmlElement Value( string value )
      {
        attribute.Value = value;
        return _element;
      }

      public IHtmlElement Replace( string oldValue, string newValue )
      {
        attribute.Value = attribute.Value.Replace( oldValue, newValue );
        return _element;
      }

      public IHtmlElement Replace( Regex pattern, string replacement )
      {
        attribute.Value = pattern.Replace( attribute.Value, replacement );
        return _element;
      }

      public IHtmlElement Replace( Regex pattern, MatchEvaluator evaluator )
      {
        attribute.Value = pattern.Replace( attribute.Value, evaluator );
        return _element;
      }

      public IHtmlElement Replace( string pattern, MatchEvaluator evaluator )
      {
        attribute.Value = Regex.Replace( attribute.Value, pattern, evaluator );
        return _element;
      }

      public IHtmlElement Replace( Func<string, string> evaluator )
      {
        attribute.Value = evaluator( attribute.Value );
        return _element;
      }

    }
  }
}
