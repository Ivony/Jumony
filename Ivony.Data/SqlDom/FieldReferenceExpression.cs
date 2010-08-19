using System;
using System.Collections.Generic;
using System.Text;

namespace Ivony.Data.SqlDom
{
  /// <summary>
  /// 通用的字段表达式
  /// </summary>
  public class FieldReferenceExpression : SqlExpression
  {
    private TableReferenceExpression _table;

    public TableReferenceExpression Table
    {
      get { return _table; }
    }
    private string _fieldName;

    public string FieldName
    {
      get { return _fieldName; }
    }

    public FieldReferenceExpression( TableReferenceExpression table, string fieldName )
    {
      _table = table;
      _fieldName = fieldName;
    }
  }


}
