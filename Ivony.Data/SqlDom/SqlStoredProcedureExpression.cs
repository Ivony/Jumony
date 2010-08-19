using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace Ivony.Data.SqlDom
{
  /// <summary>
  /// 存储过程的表达式
  /// </summary>
  public class SqlStoredProcedureExpression : SqlExpression
  {
    private string _name;

    public string Name
    {
      get { return _name; }
    }

    private Dictionary<string, object> ParamaterValues = new Dictionary<string, object>();

    public SqlStoredProcedureExpression( string name )
    {
      _name = name;
    }



  }
}
