using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ivony.Data.SqlDom;

namespace Ivony.Data
{
  public static class Db
  {
  }

  public static class Sql
  {
    public static SqlTemplateExpression Template( string template, params object[] args )
    {
      return SqlTemplateExpression.Create( template, args );
    }
  }
}
