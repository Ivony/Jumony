using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Ivony.Data
{
  /// <summary>
  /// 模板表达式
  /// </summary>
  public class SqlTemplateExpression : SqlExpression
  {

    /// <summary>
    /// 表达式模板
    /// </summary>
    public virtual string Template
    {
      get;
      protected set;
    }


    /// <summary>
    /// 表达式参数
    /// </summary>
    public virtual SqlExpression[] Parameters
    {
      get;
      protected set;
    }

    protected SqlTemplateExpression( string template, SqlExpression[] parameters )
    {
      Template = template;
      Parameters = parameters;
    }

    /// <summary>
    /// 创建一个模板表达式
    /// </summary>
    /// <param name="template">表达式模板</param>
    /// <param name="parameters">模板参数</param>
    public static SqlTemplateExpression Create( string template, params object[] parameters )
    {

      string _template = template.Replace( "{...}", ParseParameterListSymbol( parameters.Length ) );

      _template = FormatTemplate( _template, parameters );

      SqlExpression[] _parameters = new SqlExpression[parameters.Length];
      for ( int i = 0; i < parameters.Length; i++ )
      {
        SqlExpression expression = parameters[i] as SqlExpression;
        if ( expression != null )
          _parameters[i] = expression;
        else
          _parameters[i] = new SqlParameterExpression( parameters[i] );
      }

      return new SqlTemplateExpression( _template, _parameters );
    }

    /// <summary>
    /// 处理参数列表表达式“{...}”
    /// </summary>
    /// <param name="amount">参数个数</param>
    /// <returns></returns>
    private static string ParseParameterListSymbol( int amount )
    {
      StringBuilder builder = new StringBuilder();

      bool begin = true;
      for ( int i = 0; i < amount; i++ )
      {
        if ( !begin )
          builder.Append( " , " );
        builder.Append( "{" + i + "}" );
        begin = false;
      }

      return builder.ToString();
    }

    internal static readonly Regex FormatRegexNum = new Regex( @"\{(?<index>[0-9]*)(,(?<alignment>[0-9]+[a-zA-Z]*))?(\:(?<format>[^{}]*))?\}", RegexOptions.Compiled );

    private static string FormatObject( object obj, string formatstring )
    {
      if ( obj == null )
        throw new ArgumentNullException( "obj" );


      IFormattable formattable = obj as IFormattable;

      if ( formattable != null )
        return formattable.ToString( formatstring, null );
      else
        return obj.ToString();
    }


    private static string FormatTemplate( string template, object[] parameters )
    {
      return FormatRegexNum.Replace( template, delegate( Match match )
      {
        int index = int.Parse( match.Groups["index"].ToString() );
        if ( index >= parameters.Length )
          throw new ArgumentOutOfRangeException( "template" );

        string format = null;
        if ( match.Groups["format"].Success )
          format = match.Groups["format"].Value;

        if ( format != null )
          return FormatObject( parameters[index], format );
        else
          return match.ToString();

      }
      );
    }
  }
}
