using System;
using System.Collections.Generic;
using System.Text;

namespace Ivony.Data.SqlDom
{
  /// <summary>
  /// 逻辑运算表达式
  /// </summary>
  public class LogicalExpression : BooleanExpression
  {


    private BooleanExpression _leftExpression;
    /// <summary>构成逻辑表达式的左表达式</summary>
    public BooleanExpression LeftExpression
    {
      get { return _leftExpression; }
    }


    private BooleanExpression _rightExpression;
    /// <summary>构成逻辑表达式的右表达式</summary>
    public BooleanExpression RightExpression
    {
      get { return _rightExpression; }
    }


    private LogicalOperator _operator;
    /// <summary>逻辑表达式的运算符</summary>
    public LogicalOperator Operator
    {
      get { return _operator; }
    }


    public LogicalExpression( LogicalOperator op, BooleanExpression leftExpression, BooleanExpression rightExpression )
    {
      _leftExpression = leftExpression;
      _rightExpression = rightExpression;
      _operator = op;
    }

  }

}
