using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ivony.Html.Web
{
  public class ListDataContext : IListDataContext
  {

    private object[] _list;

    public ListDataContext( IEnumerable list )
    {
      _list = list.Cast<object>().ToArray();
    }

    public object[] ListData { get { return _list; } }

    public string TargetSelector { get { return null; } }
  }
}
