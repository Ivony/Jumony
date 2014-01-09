using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ivony.Html.Web.Binding
{
  internal sealed class ListDataContext
  {


    public ListDataContext( IEnumerable listData, ListBindingMode mode )
    {
      ListData = listData.Cast<object>().ToArray();
      BindingMode = mode;
    }


    public ListBindingMode BindingMode { get; private set; }


    public object[] ListData { get; private set; }

  }
}
