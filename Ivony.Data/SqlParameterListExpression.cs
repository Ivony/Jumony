using System;
using System.Collections.Generic;
using System.Text;

namespace Ivony.Data
{
  public class SqlParameterListExpression
  {

    private object[] _parameters;

    public SqlParameterListExpression( params object[] parameters )
    {
      _parameters = parameters;
    }

  }
}
