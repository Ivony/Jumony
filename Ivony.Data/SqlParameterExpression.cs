using System;
using System.Collections.Generic;
using System.Text;

namespace Ivony.Data
{
  /// <summary>
  /// 所有参数表达式的基类
  /// </summary>
  public class SqlParameterExpression : SqlExpression
  {
    private string _name;
    /// <summary>
    /// 参数名
    /// </summary>
    public string Name
    {
      get { return _name; }
    }


    private object _value;
    /// <summary>
    /// 参数值
    /// </summary>
    public object Value
    {
      get { return _value; }
    }

    public SqlParameterExpression( object value ) : this( null, value ) { }

    public SqlParameterExpression( string name, object value )
    {
      _name = name;
      _value = value;
    }

    internal static SqlParameterExpression[] CreateArray( Array array )
    {
      if ( array.Rank != 1 )
        throw new ArgumentException( "array" );

      if ( array == null )
        throw new ArgumentNullException( "array" );


      SqlParameterExpression[] parameters = new SqlParameterExpression[array.Length];

      for ( int i = 0; i < array.Length; i++ )
        parameters[i] = new SqlParameterExpression( array.GetValue( i ) );

      return parameters;
    }
  }
}
