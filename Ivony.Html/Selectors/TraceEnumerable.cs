using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using System.Diagnostics;

namespace Ivony.Html
{
  internal class TraceEnumerable<T> : IEnumerable<T>
  {

    private CssSelector _selector;
    private IEnumerable<T> _enumerable;

    public TraceEnumerable( CssSelector selector, IEnumerable<T> enumerable )
    {
      _selector = selector;
      _enumerable = enumerable;
    }

    private class Enumerator : IEnumerator<T>
    {

      private CssSelector coreSelector;
      private IEnumerator<T> coreEnumerator;

      public Enumerator( CssSelector selector, IEnumerator<T> enumerator )
      {
        coreSelector = selector;
        coreEnumerator = enumerator;

        Trace.Write( "Selector", string.Format( CultureInfo.InvariantCulture, "Begin Enumerate Search \"{0}\"", coreSelector.ExpressionString ) );
      }


      #region IEnumerator<T> 成员

      T IEnumerator<T>.Current
      {
        get { return coreEnumerator.Current; }
      }

      #endregion

      #region IDisposable 成员

      void IDisposable.Dispose()
      {
        coreEnumerator.Dispose();
        Trace.Write( "Selector", string.Format( CultureInfo.InvariantCulture, "End Enumerate Search \"{0}\"", coreSelector.ExpressionString ) );
      }

      #endregion

      #region IEnumerator 成员

      object System.Collections.IEnumerator.Current
      {
        get { return coreEnumerator.Current; }
      }

      bool System.Collections.IEnumerator.MoveNext()
      {
        return coreEnumerator.MoveNext();
      }

      void System.Collections.IEnumerator.Reset()
      {
        Trace.Write( "Selector", string.Format( CultureInfo.InvariantCulture, "Begin Enumerate Search \"{0}\"", coreSelector.ExpressionString ) );
        coreEnumerator.Reset();
      }

      #endregion
    }



    #region IEnumerable<T> 成员

    IEnumerator<T> IEnumerable<T>.GetEnumerator()
    {
      return new Enumerator( _selector, _enumerable.GetEnumerator() );
    }

    #endregion

    #region IEnumerable 成员

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
    {
      throw new NotImplementedException();
    }

    #endregion
  }
}
