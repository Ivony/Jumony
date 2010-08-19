
#pragma warning disable 1591
#warning 1591 disabled

using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Data;
using System.Collections;
using Ivony.Data.SqlDom;

namespace Ivony.Data
{
  /// <summary>
  /// 分析处理命令表达式的类
  /// </summary>
  public abstract partial class DbExpressionPaser
  {

    private IDictionary _options;



    protected DbExpressionPaser( IDictionary options )
    {
      _options = options;
    }



    private Dictionary<object, string> _parsedExpressions = new Dictionary<object, string>();
    protected IDictionary<object, string> ParsedExpressions
    {
      get { return _parsedExpressions; }
    }

    /// <summary>
    /// 分析一个查询
    /// </summary>
    /// <param name="query">查询表达式</param>
    /// <param name="dataParameters">数据参数</param>
    /// <returns></returns>
    public virtual string Parse( SqlExpression query, out IDictionary<string, object> dataParameters )
    {
      if ( !(query is SqlTemplateExpression) )
        throw new NotSupportedException();

      parameters = new SortedDictionary<string, object>();

      string commandText = ParseTemplate( (SqlTemplateExpression) query );

      dataParameters = parameters;

      return commandText;
    }

    /// <summary>
    /// 分析表达式
    /// </summary>
    /// <param name="expression">表达式</param>
    /// <returns></returns>
    protected virtual string ParseExpression( SqlExpression expression )
    {
      return ParseExpression( expression, false );
    }

    /// <summary>
    /// 分析表达式
    /// </summary>
    /// <param name="expression">表达式</param>
    /// <param name="ignore">选择是否忽略已解析的表达式</param>
    /// <returns></returns>
    protected virtual string ParseExpression( SqlExpression expression, bool ignore )
    {
      if ( expression.Singleton && !ignore )
      {
        if ( ParsedExpressions.ContainsKey( expression ) )
          return ParsedExpressions[expression];
        else
          return ParsedExpressions[expression] = ParseExpression( expression, true );
      }

      if ( expression is SqlParameterExpression )
        return ParseParameter( (SqlParameterExpression) expression );
      else if ( expression is SqlTemplateExpression )
        return ParseTemplate( (SqlTemplateExpression) expression );
      else
        throw new NotSupportedException();
    }

    private int parameterIndex;
    private SortedDictionary<string, object> parameters;

    /// <summary>
    /// 分析参数表达式
    /// </summary>
    /// <param name="expression">参数表达式</param>
    /// <returns></returns>
    protected virtual string ParseParameter( SqlParameterExpression expression )
    {
      string parameterName = GetParameterName( expression.Name );

      parameters.Add( parameterName, expression.Value );
      return parameterName;
    }

    /// <summary>
    /// 获取参数名称
    /// </summary>
    /// <param name="name">用于参考的参数名称</param>
    /// <returns></returns>
    private string GetParameterName( string name )
    {
      string parameterName;

      if ( name == null )
      {
        do
        {
          parameterName = "Param" + parameterIndex;
          parameterIndex++;
        }
        while ( parameters.ContainsKey( parameterName ) );
      }
      else
      {
        if ( parameters.ContainsKey( name ) )
        {
          int i = 0;
          do
          {
            parameterName = name + "_" + i;
            i++;
          }
          while ( parameters.ContainsKey( parameterName ) );
        }
        else
          parameterName = name;
      }

      return GetDataParameterName( parameterName );
    }

    protected abstract string GetDataParameterName( string paramerterName );




    /// <summary>
    /// 解析模板表达式
    /// </summary>
    /// <param name="expression"></param>
    /// <returns></returns>
    public virtual string ParseTemplate( SqlTemplateExpression expression )
    {

      List<string> parameters = new List<string>( expression.Parameters.Length );
      for ( int i = 0; i < expression.Parameters.Length; i++ )
        parameters.Add( null );

      return SqlTemplateExpression.FormatRegexNum.Replace( expression.Template, delegate( Match match )
      {
        string format = match.Groups["format"].ToString();
        int index = int.Parse( match.Groups["index"].ToString() );

        if ( index >= expression.Parameters.Length )
          throw new Exception();

        SqlExpression childExpression = expression.Parameters[index];

        if ( childExpression.Singleton )
        {
          if ( parameters[index] == null )
            parameters[index] = ParseExpression( childExpression );

          return parameters[index];
        }

        return ParseExpression( childExpression );


      }
      );
    }
  }



  [Flags]
  public enum CommandParserOptions
  {
  }
}