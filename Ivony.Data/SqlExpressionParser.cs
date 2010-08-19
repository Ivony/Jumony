using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using Ivony.Data.SqlDom;
using System.Collections;

namespace Ivony.Data
{
  public class SqlExpressionParser
  {


    public SqlExpressionParser()
    {
    }


    public virtual SqlParsedExpression Parse( SqlExpression expression )
    {

      throw new NotImplementedException();
    }


    protected virtual SqlParsedExpression ParseTemplate( SqlTemplateExpression expression )
    {
      throw new NotImplementedException();
    }


    protected IDictionary ParsedExpression
    {
      get;
      private set;

    }


    private object _sync = new object();

    public object SyncRoot
    {
      get { return _sync; }
    }

  }
}
