using System;
using System.Collections.Generic;
using System.Text;

namespace Ivony.Data.SqlDom
{
  /// <summary>
  /// 表名引用表达式
  /// </summary>
  public class TableReferenceExpression : SqlExpression
  {

    private string _tableName;

    public string TableName
    {
      get { return _tableName; }
    }

    private string _databaseName;

    public string DatabaseName
    {
      get { return _databaseName; }
    }

    public TableReferenceExpression( string tablename )
    {
      _tableName = tablename;
    }
  }
}
