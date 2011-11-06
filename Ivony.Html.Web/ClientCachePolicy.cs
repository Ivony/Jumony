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


    private HttpContextBase _context;


    internal ClientCachePolicy( HttpContextBase context )
    {
      _context = context;

      _context.ApplicationInstance.PreSendRequestHeaders += new EventHandler( PreSendRequestHeaders );
    }

    private void PreSendRequestHeaders( object sender, EventArgs e )
    {
      ApplyClientCachePolicy();
    }


    /// <summary>
    /// 应用客户端缓存策略
    /// </summary>
    public void ApplyClientCachePolicy()
    {
      var response = _context.Response;

    
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
    /// <param name="delta">从当前开始最大的过期时间</param>
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




    private string[] _varyHeaders;

    /// <summary>
    /// 设置 Vary 标头，指定哪些 HTTP 头可能引起缓存结果失效
    /// </summary>
    /// <param name="headers"></param>
    public void SetVary( string[] headers )
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
    public void DisableAbsoluteExpiration
    {
      get;
      set;
      ]


  }
}
