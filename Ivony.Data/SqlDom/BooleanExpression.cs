using System;
using System.Collections.Generic;
using System.Text;

namespace Ivony.Data.SqlDom
{
  /// <summary>
  /// 布尔值表达式
  /// </summary>
  public abstract class BooleanExpression : SqlExpression
  {
    public static BooleanExpression CreateExpression( LogicalOperator op, params BooleanExpression[] expressions )
    {
      if ( expressions.Length < 2 )
        throw new ArgumentException( "expressions" );

      if ( expressions.Length == 2 )
        return CreateExpression( op, expressions[0], expressions[1] );

      Queue<BooleanExpression> queue = new Queue<BooleanExpression>( expressions );

      return CreateExpression( op, queue.Dequeue(), queue );
    }

    private static BooleanExpression CreateExpression( LogicalOperator op, BooleanExpression expression, Queue<BooleanExpression> queue )
    {
      if ( queue.Count == 1 )
        return CreateExpression( op, expression, queue.Dequeue() );

      return CreateExpression( op, CreateExpression( op, expression, queue.Dequeue() ), queue );
    }

    public static BooleanExpression CreateExpression( LogicalOperator op, BooleanExpression expression1, BooleanExpression expression2 )
    {
      return new LogicalExpression( op, expression1, expression2 );
    }

    public static BooleanExpression CreateExpression( FieldReferenceExpression leftExpression, ComparisionOperator op, ParameterExpression rightExpression )
    {
      return new ComparisionExpression( op, leftExpression, rightExpression );
    }

    public static BooleanExpression CreateExpression( FieldReferenceExpression leftExpression, ComparisionOperator op, FieldReferenceExpression rightExpression )
    {
      return new ComparisionExpression( op, leftExpression, rightExpression );
    }

    public static BooleanExpression CreateExpression( ParameterExpression leftExpression, ComparisionOperator op, FieldReferenceExpression rightExpression )
    {
      return new ComparisionExpression( op, leftExpression, rightExpression );
    }

    public static BooleanExpression CreateLikeExpression( FieldReferenceExpression fieldExpression, ParameterExpression parameterExpression )
    {
      return new LikeExpression( fieldExpression, parameterExpression );
    }




    /*
    protected bool IsComparisionOperator( LogicalOperator op )
    {
      switch ( op )
      {
        case LogicalOperator.GreaterThan:
        case LogicalOperator.LessThan:
        case LogicalOperator.Equality:
        case LogicalOperator.Inequality:
        case LogicalOperator.GreaterThanOrEqual:
        case LogicalOperator.LessThanOrEqual:
          return true;
        default:
          return false;
      }
    }

    protected static bool IsLogicalOperator( LogicalOperator op )
    {
      switch ( op )
      {                                                 
        case LogicalOperator.And:
        case LogicalOperator.Or:
          return true;
        default:
          return false;
      }
    }
    */


  }


  /// <summary>
  /// 逻辑运算符
  /// </summary>
  public enum LogicalOperator
  {
    /// <summary>代表逻辑与，并且</summary>
    And,
    /// <summary>代表逻辑或，或者</summary>
    Or
  }
  public enum ComparisionOperator
  {
    /// <summary>大于</summary>
    GreaterThan = 5,
    /// <summary>小于</summary>
    LessThan,
    /// <summary>等于</summary>
    Equality,
    /// <summary>不等于</summary>
    Inequality,
    /// <summary>大于或等于</summary>
    GreaterThanOrEqual,
    /// <summary>小于或等于</summary>
    LessThanOrEqual,
  }

}
