using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ivony.Html.Binding
{
  internal sealed class ListDataModel
  {

    public ListDataModel( IEnumerable listData, CssElementSelector selector, ListBindingMode mode )
    {
      ListData = listData.Cast<object>().ToArray();
      Selector = selector;
      BindingMode = mode;
    }


    public ListBindingMode BindingMode { get; private set; }


    public CssElementSelector Selector { get; private set; }


    public object[] ListData { get; private set; }

  }
}
