using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Routing;
using Ivony.Fluent;
using System.Web;

namespace Ivony.Html.Web
{

  /// <summary>
  /// 表示一个缓存标记，缓存标记可以从当前请求的特征中产生，并变换为唯一的 CacheKey 或是 ETag 标记
  /// </summary>
  public class CacheToken
  {

    private string _type;
    private string[] _tokens;



    private CacheToken()
    {

    }


    /// <summary>
    /// 创建 CacheToken 的实例
    /// </summary>
    /// <param name="type">CacheToken 的类别</param>
    /// <param name="tokens">一些特征字符串</param>
    public CacheToken( string type, params string[] tokens )
    {
      _type = type;
      _tokens = tokens;
    }

    private string _tokenString;

    private virtual void EnsureTokenString()
    {
      if ( _tokenString == null )
      {
        _tokenString = _type.Replace( ":", "@:" ) + ":" + string.Join( ";", _tokens.Select( t => t.Replace( "@", "@@" ).Replace( ";", "@;" ) ) );
        _tokenString = _tokenString.Replace( "+", "@+" );

      }
    }


    private sealed class MultipleCacheToken : CacheToken
    {

      private CacheToken[] _tokens;

      public MultipleCacheToken( params CacheToken[] tokens )
      {

        List<CacheToken> list = new List<CacheToken>();

        foreach ( var t in tokens )
        {
        }


        _tokens = list.ToArray();
      }



      private static void AddToken( IList<CacheToken> list, CacheToken token )
      {
        var multiple = token as MultipleCacheToken;
        
        if ( multiple != null )
          multiple._tokens.ForAll( t => AddToken( list, t ) );

        else

          list.Add( token );
      }



      private override void EnsureTokenString()
      {
        if ( _tokenString == null )
        {
          _tokenString = string.Join( "+", _tokens.Select( t => t.ToString() ) );
        }
      }

    }




    /// <summary>
    /// 从缓存标记产生缓存键
    /// </summary>
    /// <returns></returns>
    public string CacheKey()
    {
      return ToString();
    }

    /// <summary>
    /// 从缓存标记产生 ETag 标记
    /// </summary>
    /// <returns></returns>
    public string ETag()
    {
      return HttpServerUtility.UrlTokenEncode( CacheHelper.ComputeHash( ToString() ) );
    }


    /// <summary>
    /// 获取缓存标记的字符串表达形式
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
      EnsureTokenString();
      return _tokenString;
    }


    /// <summary>
    /// 合并两个缓存标记
    /// </summary>
    /// <param name="token1"></param>
    /// <param name="token2"></param>
    /// <returns></returns>
    public static CacheToken operator +( CacheToken token1, CacheToken token2 )
    {
      return Combine( token1, token2 );
    }


    /// <summary>
    /// 从当前路由值中产生缓存标记
    /// </summary>
    /// <param name="routeValues">路由值</param>
    /// <returns></returns>
    public static CacheToken From( RouteValueDictionary routeValues )
    {
      return From( routeValues, null );
    }


    /// <summary>
    /// 从当前路由值中产生缓存标记
    /// </summary>
    /// <param name="routeValues">路由值</param>
    /// <param name="keys">需要产生缓存标记的路由键</param>
    /// <returns></returns>
    public static CacheToken From( RouteValueDictionary routeValues, params string[] keys )
    {
      IEnumerable<KeyValuePair<string,object>> values = routeValues;

      if ( !keys.IsNullOrEmpty() )
        values = routeValues.Where( pair => keys.Contains( pair.Key, StringComparer.OrdinalIgnoreCase ) );

      else
        values = routeValues;

      return new CacheToken( "RouteValue", values.Select( pair => string.Format( "{0}={1}", pair.Key.Replace( "=", "@=" ), pair.Value ) ).ToArray() );
    }



    /// <summary>
    /// 从 Cookies 中产生缓存标记
    /// </summary>
    /// <param name="cookies"></param>
    /// <param name="names"></param>
    /// <returns></returns>
    public static CacheToken From( HttpCookieCollection cookies, params string[] names )
    {
      IEnumerable<HttpCookie> _cookies = cookies.Cast<HttpCookie>();

      if ( !names.IsNullOrEmpty() )
        _cookies = _cookies.Where( c => names.Contains( c.Name, StringComparer.OrdinalIgnoreCase ) );

      return new CacheToken( "Cookies", _cookies.Select( c => string.Format( "{0}={1}", c.Name.Replace( "=", "@=" ), c.Value ) ).ToArray() );
    }



    /// <summary>
    /// 合并多个缓存标记
    /// </summary>
    /// <param name="tokens"></param>
    /// <returns></returns>
    public static CacheToken Combine( params CacheToken[] tokens )
    {
      return Combine( tokens );
    }

    /// <summary>
    /// 合并多个缓存标记
    /// </summary>
    /// <param name="tokens"></param>
    /// <returns></returns>
    public static CacheToken Combine( IEnumerable<CacheToken> tokens )
    {

      if ( tokens.IsNullOrEmpty() )
        return null;

      tokens = tokens.NotNull();
      if ( tokens.IsSingle() )
        return tokens.Single();

      return new MultipleCacheToken( tokens.ToArray() );
    }


    public static CacheToken FromSessionID( HttpContextBase context )
    {
      return new CacheToken( "SessionID", context.Session.SessionID );
    }

    public static CacheToken FromVirtualPath( HttpContextBase context )
    {
      return new CacheToken( "VirtualPath", context.Request.AppRelativeCurrentExecutionFilePath );
    }
  }
}
