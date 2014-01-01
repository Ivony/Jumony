using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Ivony.Html.Web.Binding
{


  /// <summary>
  /// 属性表达式
  /// </summary>
  public sealed class AttributeExpression : BindingExpression
  {

    private static readonly Regex attributeExpressionRegex = new Regex( "^" + AttributeExpressionPattern + "$", RegexOptions.IgnoreCase | RegexOptions.Compiled );
    private string _name;
    private IDictionary<string, string> _args;


    private AttributeExpression( IHtmlAttribute attribute, string expression, string name, IDictionary<string, string> args )
    {
      Attribute = attribute;
      _expression = expression;

      _name = name;
      _args = args;
    }


    /// <summary>
    /// 定义属性绑定表达式的属性
    /// </summary>
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
    public static AttributeExpression ParseExpression( IHtmlAttribute attribute )
    {
      var expression = attribute.Value();

      if ( expression == null )
        return null;

      var match = attributeExpressionRegex.Match( expression );
      if ( match == null || !match.Success )
        return null;

      return ParseExpression( attribute, expression, match );
    }



    private static AttributeExpression ParseExpression( IHtmlAttribute attribute, string expression, Match match )
    {

      var _name = match.Groups["ename"].Value;

      var args = new Dictionary<string, string>( StringComparer.OrdinalIgnoreCase );

      foreach ( Capture capture in match.Groups["args"].Captures )
      {
        var name = capture.FindCaptures( match.Groups["name"] ).First().Value;
        var value = capture.FindCaptures( match.Groups["value"] ).First().Value;

        args[name] = value;
      }

      return new AttributeExpression( attribute, expression, _name, args );

    }


    /// <summary>
    /// 检查并确认属性值没有被修改
    /// </summary>
    protected void CheckAttribute()
    {
      if ( Attribute.AttributeValue != _expression )
        throw new InvalidOperationException( "属性值已经发生变化" );
    }



  }
}
