using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Ivony.Html.Web
{
  public interface ICacheDependency
  {

    bool HasChanged { get; }

  }

  [Serializable]
  public class CompositeCacheDependency : ICacheDependency
  {

    private ICacheDependency[] _cacheDependencies;

    public CompositeCacheDependency( params ICacheDependency[] cacheDependencies )
    {
      _cacheDependencies = cacheDependencies;
    }


    private bool _hasChanged;

    public bool HasChanged
    {
      get
      {
        if ( _hasChanged )
          return true;

        return _hasChanged = _cacheDependencies.Any( d => d.HasChanged );
      }
    }
  }
}
