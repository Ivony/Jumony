using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ivony.Html.Web
{
  public class ListDataContext
  {

    public ListDataContext( IEnumerable data )
    {
      ListData = data.Cast<object>().ToArray();
    }


    public object[] ListData
    {
      get;
      private set;
    }

  }
}
