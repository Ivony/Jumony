using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ivony.Data.SqlServer
{
  public class TableExpression
  {

    public string TableName
    {
      get;
      private set;
    }

    private string[] _fields;

    internal TableExpression( string tableName )
    {
      TableName = tableName;
    }

    public TableExpression Fields( string[] fields )
    {
      _fields = fields;
      return this;
    }


  }
}
