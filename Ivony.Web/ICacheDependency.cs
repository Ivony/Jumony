using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Ivony.Web
{
  /// <summary>
  /// 定义缓存依赖项
  /// </summary>
  public interface ICacheDependency
  {

    /// <summary>
    /// 指示缓存依赖项是否已经被修改（已过期）
    /// </summary>
    bool HasChanged { get; }

  }

  
  /// <summary>
  /// 复合缓存依赖项
  /// </summary>
  /// <remarks>
  /// 复合缓存依赖项由多个缓存依赖项组成，任何一个缓存依赖项过期，则整个复合缓存依赖项过期。
  /// </remarks>
  [Serializable]
  public class CompositeCacheDependency : ICacheDependency
  {

    private ICacheDependency[] _cacheDependencies;

    /// <summary>
    /// 创建 CompositeCacheDependency 对象
    /// </summary>
    /// <param name="cacheDependencies">组成复合缓存依赖项的依赖项</param>
    public CompositeCacheDependency( params ICacheDependency[] cacheDependencies )
    {
      _cacheDependencies = cacheDependencies;
    }


    private bool _hasChanged;

    /// <summary>
    /// 指示缓存依赖项是否已经被修改（已过期）
    /// </summary>
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
