using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Ivony.Html.Web
{


  /// <summary>
  /// 属性表达式
  /// </summary>
  public sealed class AttributeBindingExpression : BindingExpression
  {

    /// <summary>
    /// 用于解析属性表达式参数的正则表达式
    /// </summary>
    public static readonly string ExpressionArgumentPattern = @"(?<args>(?<name>\w+)(=(?<value>[^\,]+))?)";

    /// <summary>
    /// 用于解析属性表达式的正则表达式
    /// </summary>
    public static readonly string AttributeExpressionPattern = string.Format( @"(?<expression>\{{(?<ename>\w+)\s*({0}(\,\s*{0})*)?\}})", ExpressionArgumentPattern );


    private static readonly Regex attributeExpressionRegex = new Regex( "^" + AttributeExpressionPattern + "$", RegexOptions.IgnoreCase | RegexOptions.Compiled );
    private string _name;
    private IDictionary<string, string> _args;


    private AttributeBindingExpression( IHtmlAttribute attribute, string expression, string name, IDictionary<string, string> args )
    {
      Attribute = attribute;
      _expression = expression;

      _name = name;
      _args = args;
    }


    public IHtmlAttribute Attribute
    {
      get;
      private set;
    }


    private string _expression;



    /// <summary>
    /// 表达式名称
    /// </summary>
    public override string Name
    {
      get
      {

        CheckAttribute();

        return _name;
      }
    }

    private void CheckAttribute()
    {
      if ( Attribute.AttributeValue != _expression )
        throw new InvalidOperationException( "属性值已经发生变化" );
    }


    /// <summary>
    /// 参数值
    /// </summary>
    public override IDictionary<string, string> Arguments
    {
      get
      {
        CheckAttribute();

        return new Dictionary<string, string>( _args, StringComparer.OrdinalIgnoreCase );
      }
    }



    /// <summary>
    /// 解析属性为绑定表达式
    /// </summary>
    /// <param name="attribute">要解析的属性</param>
    /// <returns>绑定表达式</returns>
    public static AttributeBindingExpression ParseExpression( IHtmlAttribute attribute )
    {
      var expression = attribute.Value();

      if ( expression == null )
        return null;

      var match = attributeExpressionRegex.Match( expression );
      if ( match == null || !match.Success )
        return null;

      return ParseExpression( attribute, expression, match );
    }



    private static AttributeBindingExpression ParseExpression( IHtmlAttribute attribute, string expression, Match match )
    {

      var _name = match.Groups["ename"].Value;

      var args = new Dictionary<string, string>( StringComparer.OrdinalIgnoreCase );

      foreach ( Capture capture in match.Groups["args"].Captures )
      {
        var name = capture.FindCaptures( match.Groups["name"] ).First().Value;
        var value = capture.FindCaptures( match.Groups["value"] ).First().Value;

        args[name] = value;
      }

      return new AttributeBindingExpression( attribute, expression, _name, args );

    }
  }
}
