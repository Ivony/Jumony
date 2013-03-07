using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace Ivony.Web
{



  /// <summary>
  /// 客户端缓存策略基类
  /// </summary>
  public abstract class ClientCachePolicyBase
  {

    /// <summary>
    /// 设置缓存过期时间
    /// </summary>
    /// <param name="delta">从当前开始最大的过期时间</param>
    public abstract void SetMaxAge( TimeSpan delta );



    /// <summary>
    /// 设置代理服务器缓存过期时间
    /// </summary>
    /// <param name="delta">从当前开始最大的过期时间</param>
    public abstract void SetProxyMaxAge( TimeSpan delta );



    /// <summary>
    /// 设置 cache-control 标头为 HttpCacheability 值之一
    /// </summary>
    /// <param name="cacheability">缓存可用性</param>
    public abstract void SetCacheability( HttpCacheability cacheability );



    /// <summary>
    /// 设置缓存绝对过期时间
    /// </summary>
    /// <param name="expiresDate">缓存绝对过期时间</param>
    public abstract void SetExpires( DateTimeOffset expiresDate );



    /// <summary>
    /// 设置请求内容最后一次被修改的时间
    /// </summary>
    /// <param name="lastModified">请求内容最后一次被修改的时间</param>
    public abstract void SetLastModified( DateTimeOffset lastModified );



    /// <summary>
    /// 设置内容标识
    /// </summary>
    /// <param name="etag">用于标识内容的哈希值</param>
    public abstract void SetETag( string etag );


    /// <summary>
    /// 设置 Vary 标头，指定哪些 HTTP 头可能引起缓存结果失效
    /// </summary>
    /// <param name="headers">将要引起缓存失效的 HTTP 头</param>
    public abstract void SetVary( string[] headers );



  }


  /// <summary>
  /// 封装 HttpCachePolicyBase 对象，使其支持客户端缓存策略。
  /// </summary>
  public sealed class ClientCachePolicyWrapper : ClientCachePolicyBase
  {


    private HttpCachePolicyBase _cachePolicy;

    /// <summary>
    /// 创建 ClientCachePolicyWrapper 对象
    /// </summary>
    /// <param name="cachePolicy">要被封装的 HttpCachePolicyBase 对象</param>
    public ClientCachePolicyWrapper( HttpCachePolicyBase cachePolicy )
    {
      _cachePolicy = cachePolicy;
    }


    /// <summary>
    /// 设置缓存过期时间
    /// </summary>
    /// <param name="delta">从当前开始最大的过期时间</param>
    public override void SetMaxAge( TimeSpan delta )
    {
      _cachePolicy.SetMaxAge( delta );
    }

    /// <summary>
    /// 设置代理服务器缓存过期时间
    /// </summary>
    /// <param name="delta">从当前开始最大的过期时间</param>
    public override void SetProxyMaxAge( TimeSpan delta )
    {
      _cachePolicy.SetProxyMaxAge( delta );
    }

    /// <summary>
    /// 设置 cache-control 标头为 HttpCacheability 值之一
    /// </summary>
    /// <param name="cacheability">缓存可用性</param>
    public override void SetCacheability( HttpCacheability cacheability )
    {

      switch ( cacheability )
      {
        case HttpCacheability.NoCache:
        case HttpCacheability.Private:
        case HttpCacheability.Public:
          _cachePolicy.SetCacheability( cacheability );
          break;


        default:
          throw new InvalidOperationException();
      }

    }

    /// <summary>
    /// 设置缓存绝对过期时间
    /// </summary>
    /// <param name="expiresDate">缓存绝对过期时间</param>
    public override void SetExpires( DateTimeOffset expiresDate )
    {
      _cachePolicy.SetExpires( expiresDate.UtcDateTime );
    }

    /// <summary>
    /// 设置请求内容最后一次被修改的时间
    /// </summary>
    /// <param name="lastModified">请求内容最后一次被修改的时间</param>
    public override void SetLastModified( DateTimeOffset lastModified )
    {
      _cachePolicy.SetLastModified( lastModified.UtcDateTime );
    }

    /// <summary>
    /// 设置内容标识
    /// </summary>
    /// <param name="etag">用于标识内容的哈希值</param>
    public override void SetETag( string etag )
    {
      _cachePolicy.SetETag( etag );
    }

    /// <summary>
    /// 设置 Vary 标头，指定哪些 HTTP 头可能引起缓存结果失效
    /// </summary>
    /// <param name="headers">将要引起缓存失效的 HTTP 头</param>
    public override void SetVary( string[] headers )
    {
      _cachePolicy.SetVaryByCustom( string.Join( ",", headers ) );
    }
  }




  /// <summary>
  /// 定义和协助应用 HTTP 客户端缓存策略
  /// </summary>
  public sealed class ClientCachePolicy : ClientCachePolicyBase
  {


    internal ClientCachePolicy()
    {
    }


    /// <summary>
    /// 应用客户端缓存策略
    /// </summary>
    public void ApplyClientCachePolicy( HttpResponseBase response )
    {

      string cacheControl;

      switch ( _cacheability )
      {
        case HttpCacheability.NoCache:
          cacheControl = "no-cache";
          break;

        case HttpCacheability.Public:
          cacheControl = "public";
          break;

        default:
        case HttpCacheability.Private:
          cacheControl = "private";
          break;
      }


      if ( cacheControl != "no-cache" )
      {
        if ( _maxAge != null )
          cacheControl += ",max-age=" + (int) _maxAge.Value.TotalSeconds;

        if ( _sMaxAge != null )
          cacheControl += ",s-maxage=" + (int) _sMaxAge.Value.TotalSeconds;
      }

      response.AppendHeader( "Cache-Control", cacheControl );


      if ( _expires != null )
        response.AppendHeader( "Expires", _expires.Value.ToString( "R" ) );


      if ( _lastModified != null )
        response.AppendHeader( "Last-Modified", _lastModified.Value.ToString( "R" ) );


      if ( _etag != null )
        response.AppendHeader( "ETag", _etag );

      if ( _varyHeaders != null && _varyHeaders.Any() )
        response.AppendHeader( "Vary", string.Join( ",", _varyHeaders ) );

    }


    private TimeSpan? _maxAge;

    /// <summary>
    /// 设置缓存过期时间
    /// </summary>
    /// <param name="delta">从当前开始最大的过期时间</param>
    public override void SetMaxAge( TimeSpan delta )
    {
      if ( delta > TimeSpan.FromDays( 300 ) || delta < TimeSpan.Zero )
        throw new ArgumentOutOfRangeException( "delta" );

      _maxAge = delta;
    }


    private TimeSpan? _sMaxAge;

    /// <summary>
    /// 设置代理服务器缓存过期时间
    /// </summary>
    /// <param name="delta">从当前开始最大的过期时间</param>
    public override void SetProxyMaxAge( TimeSpan delta )
    {

      if ( delta > TimeSpan.FromDays( 300 ) || delta < TimeSpan.Zero )
        throw new ArgumentOutOfRangeException( "delta" );

      _sMaxAge = delta;
    }



    private HttpCacheability _cacheability;

    /// <summary>
    /// 设置 cache-control 标头为 HttpCacheability 值之一
    /// </summary>
    /// <param name="cacheability">缓存可用性</param>
    public override void SetCacheability( HttpCacheability cacheability )
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
    public override void SetExpires( DateTimeOffset expiresDate )
    {
      _expires = expiresDate;
    }



    private DateTimeOffset? _lastModified = null;

    /// <summary>
    /// 设置请求内容最后一次被修改的时间
    /// </summary>
    /// <param name="lastModified">请求内容最后一次被修改的时间</param>
    public override void SetLastModified( DateTimeOffset lastModified )
    {
      _lastModified = lastModified;
    }



    private string _etag;

    /// <summary>
    /// 设置内容标识
    /// </summary>
    /// <param name="etag">用于标识内容的哈希值</param>
    public override void SetETag( string etag )
    {
      if ( etag == null )
        throw new ArgumentNullException( "etag" );

      _etag = etag;
    }





    private string[] _varyHeaders;

    internal static string Token = "Jumony_ClientCachePolicy";

    /// <summary>
    /// 设置 Vary 标头，指定哪些 HTTP 头可能引起缓存结果失效
    /// </summary>
    /// <param name="headers">将要引起缓存失效的 HTTP 头</param>
    public override void SetVary( string[] headers )
    {
      _varyHeaders = headers;
    }



    /// <summary>
    /// 重置 cache-control 设置
    /// </summary>
    public void ResetCacheControl()
    {

    }


    /// <summary>
    /// 重置 Last-Modified、ETag 等设置
    /// </summary>
    public void ResetEntitySettings()
    {

    }


    /// <summary>
    /// 指定是否禁用绝对过期
    /// </summary>
    public bool DisableAbsoluteExpiration
    {
      get;
      set;
    }
  }
}
