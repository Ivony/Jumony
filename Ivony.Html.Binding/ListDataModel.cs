using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ivony.Html.Binding
{
  internal sealed class ListDataModel : ICollection, ICustomBindingContextModel
  {

    public ListDataModel( IEnumerable listData, CssElementSelector selector, ListBindingMode mode )
    {
      RawObject = listData;
      list = listData.Cast<object>().ToArray();
      Selector = selector;
      BindingMode = mode;
    }


    public ListBindingMode BindingMode { get; private set; }

    public CssElementSelector Selector { get; private set; }


    private object[] list;


    public void CopyTo( Array array, int index )
    {
      list.CopyTo( array, index );
    }

    public int Count
    {
      get { return list.Length; }
    }

    public bool IsSynchronized
    {
      get { return false; }
    }

    public object SyncRoot
    {
      get { return list.SyncRoot; }
    }

    public IEnumerator GetEnumerator()
    {
      return list.GetEnumerator();
    }

    public object this[int index]
    {
      get { return list[index]; }
    }

    public object RawObject { get; private set; }

    HtmlBindingContext ICustomBindingContextModel.CreateBindingContext( HtmlBindingContext context, IHtmlContainer scope )
    {

      var element = scope as IHtmlElement;
      if ( element == null )
        return null;

      return new HtmlListBindingContext( context, element, this );
    }
  }
}
