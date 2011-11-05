using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace Ivony.Html.Web
{

  /// <summary>
  /// 定义 HTTP 客户端缓存策略
  /// </summary>
  public class ClientCachePolicy
  {


    private HttpResponseBase _response;


    internal ClientCachePolicy( HttpResponseBase response )
    {
      _response = response;
    }


    /// <summary>
    /// 应用客户端缓存策略
    /// </summary>
    public void ApplyClientCachePolicy()
    {

    }


    private TimeSpan _maxAge;

    /// <summary>
    /// 设置缓存过期时间
    /// </summary>
    /// <param name="delta">从当前开始最大的过期时间</param>
    public void SetMaxAge( TimeSpan delta )
    {
      if ( delta > TimeSpan.FromDays( 300 ) || delta < TimeSpan.FromSeconds( 1 ) )
        throw new ArgumentOutOfRangeException( "delta" );

      _maxAge = delta;
    }


    private TimeSpan _sMaxAge;

    /// <summary>
    /// 设置代理服务器缓存过期时间
    /// </summary>
    /// <param name="delta"></param>
    public void SetProxyMaxAge( TimeSpan delta )
    {

      if ( delta > TimeSpan.FromDays( 300 ) || delta < TimeSpan.FromSeconds( 1 ) )
        throw new ArgumentOutOfRangeException( "delta" );

      _sMaxAge = delta;
    }



    private HttpCacheability _cacheability;

    /// <summary>
    /// 设置 cache-control 标头为 HttpCacheability 值之一
    /// </summary>
    /// <param name="cacheability">缓存可用性</param>
    public void SetCachability( HttpCacheability cacheability )
    {
      switch ( cacheability )
      {
        case HttpCacheability.NoCache:
        case HttpCacheability.Private:
        case HttpCacheability.Public:
          _cacheability = cacheability;
          break;
        default:
          throw new ArgumentException( "cacheability", "不受支持的 Cacheablity 值" );
      }
    }



    private DateTimeOffset? _expires = null;

    /// <summary>
    /// 设置缓存绝对过期时间
    /// </summary>
    /// <param name="expiresDate">缓存绝对过期时间</param>
    public void SetExpires( DateTimeOffset expiresDate )
    {
      _expires = expiresDate;
    }



    private DateTimeOffset? _lastModified = null;

    /// <summary>
    /// 设置请求内容最后一次被修改的时间
    /// </summary>
    /// <param name="lastModified">请求内容最后一次被修改的时间</param>
    public void SetLastModified( DateTimeOffset lastModified )
    {
      _lastModified = lastModified;
    }


  }
}
