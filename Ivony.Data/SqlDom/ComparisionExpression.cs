using System;
using System.Collections.Generic;
using System.Text;

namespace Ivony.Data.SqlDom
{
  /// <summary>
  /// 比较运算表达式
  /// </summary>
  public class ComparisionExpression : BooleanExpression
  {

    private SqlExpression _leftExpression;
    /// <summary>左侧表达式</summary>
    public SqlExpression LeftExpression
    {
      get { return _leftExpression; }
    }



    private SqlExpression _rightExpression;
    /// <summary>右侧表达式</summary>
    public SqlExpression RightExpression
    {
      get { return _rightExpression; }
    }


    private ComparisionOperator _operator;
    /// <summary>比较运算符</summary>
    public ComparisionOperator Operator
    {
      get { return _operator; }
    }




    /// <summary>
    /// 构造一个比较表达式
    /// </summary>
    /// <param name="op">比较运算符</param>
    /// <param name="leftExpression">左侧表达式</param>
    /// <param name="rightExpression">右侧表达式</param>
    internal ComparisionExpression( ComparisionOperator op, SqlExpression leftExpression, SqlExpression rightExpression )
    {
      _operator = op;
      _leftExpression = leftExpression;
      _rightExpression = rightExpression;
    }

  }
}
