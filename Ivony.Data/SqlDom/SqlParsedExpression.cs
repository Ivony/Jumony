using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Data;

namespace Ivony.Data.SqlDom
{
  public abstract class SqlParsedExpression
  {
    public abstract string Template
    {
      get;
    }

    public abstract IDictionary Args
    {
      get;
    }
  }
}
