using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace Ivony.Data
{
  public interface IDbCommandProvider
  {

    IDbCommand CreateCommand();

    IDbDataParameter CreateParameter();


  }
}
