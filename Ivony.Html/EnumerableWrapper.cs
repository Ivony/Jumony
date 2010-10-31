using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace Ivony.Html
{
  public abstract class EnumerableWrapper : IEnumerable
  {

    protected abstract IEnumerable GetEnumerable();


    IEnumerator IEnumerable.GetEnumerator()
    {
      return GetEnumerable().GetEnumerator();
    }
  }

  public abstract class EnumerableWrapper<T> : IEnumerable<T>
  {

    protected abstract IEnumerable<T> GetEnumerable();


    IEnumerator<T> IEnumerable<T>.GetEnumerator()
    {
      return GetEnumerable().GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return GetEnumerable().GetEnumerator();
    }
  }

}
