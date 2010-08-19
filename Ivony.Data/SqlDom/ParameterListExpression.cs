using System;
using System.Collections.Generic;
using System.Text;

namespace Ivony.Data.SqlDom
{
  public class ParameterListExpression
  {

    private object[] _parameters;

    public ParameterListExpression( params object[] parameters )
    {
      _parameters = parameters;
    }

  }
}
