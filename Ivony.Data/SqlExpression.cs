using System;
using System.Collections.Generic;
using System.Text;

namespace Ivony.Data
{
  public abstract class SqlExpression
  {
    public virtual bool Singleton
    {
      get { return true; }
    }
  }
}
