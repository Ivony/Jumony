using System;
using System.Collections.Generic;
using System.Text;

namespace Ivony.Data.SqlDom
{
  /// <summary>
  /// 表示Like表达式
  /// </summary>
  public class LikeExpression : BooleanExpression
  {

    private FieldReferenceExpression _fieldExpression;

    public FieldReferenceExpression FieldExpression
    {
      get { return _fieldExpression; }
    }



    private ParameterExpression _parameterExpression;

    public ParameterExpression ParameterExpression
    {
      get { return _parameterExpression; }
    }

    /// <summary>
    /// 构造一个Like表达式
    /// </summary>
    /// <param name="fieldExpression">字段表达式</param>
    /// <param name="parameterExpression">变量表达式</param>
    public LikeExpression( FieldReferenceExpression fieldExpression, ParameterExpression parameterExpression )
    {
      _fieldExpression = fieldExpression;
      _parameterExpression = parameterExpression;
    }

  }
}
