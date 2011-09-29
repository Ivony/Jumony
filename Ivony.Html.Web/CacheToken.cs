using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Routing;
using Ivony.Fluent;
using System.Web;
using System.Collections.Specialized;

namespace Ivony.Html.Web
{

  /// <summary>
  /// 表示一个缓存标记，缓存标记可以从当前请求的特征中产生，并变换为唯一的 CacheKey
  /// </summary>
  [Serializable]
  public sealed class CacheToken
  {

    private CacheTokenItem[] _tokens;

    private CacheToken( params CacheTokenItem[] tokens )
    {
      if ( tokens.GroupBy( t => t.TypeName ).Any( g => g.Count() > 1 ) )
        throw new Exception( "不能合并包含相同类型的 CacheToken" );

      _tokens = tokens;
    }



    private string _tokenString;

    private void EnsureTokenString()
    {
      if ( _tokenString == null )
      {
        _tokenString = string.Join( "+", _tokens.Select( t => t.ToString() ) );
      }
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
    /// 比较两个缓存标记项
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public override bool Equals( object obj )
    {
      var _token = obj as CacheToken;
      if ( _token == null )
        return false;

      return ToString().Equals( _token.ToString(), StringComparison.Ordinal );
    }


    /// <summary>
    /// 计算哈希值
    /// </summary>
    /// <returns></returns>
    public override int GetHashCode()
    {
      return ToString().GetHashCode();
    }




    [Serializable]
    private sealed class CacheTokenItem
    {

      private string[] _tokens;


      public CacheTokenItem( string type, params string[] tokens )
      {
        TypeName = type;
        _tokens = tokens;
      }

      private string _tokenString;

      private void EnsureTokenString()
      {
        if ( _tokenString == null )
        {
          _tokenString = TypeName.Replace( ":", "@:" ) + ":" + string.Join( ";", _tokens.Select( t => t.Replace( "@", "@@" ).Replace( ";", "@;" ) ) );
          _tokenString = _tokenString.Replace( "+", "@+" );

        }
      }


      public string TypeName
      {
        get;
        private set;
      }


      /// <summary>
      /// 获取缓存标记项的字符串表达形式
      /// </summary>
      /// <returns></returns>
      public override string ToString()
      {
        EnsureTokenString();
        return _tokenString;
      }


      /// <summary>
      /// 计算哈希值
      /// </summary>
      /// <returns></returns>
      public override int GetHashCode()
      {
        return ToString().GetHashCode();
      }


      /// <summary>
      /// 比较两个缓存标记项
      /// </summary>
      /// <param name="obj"></param>
      /// <returns></returns>
      public override bool Equals( object obj )
      {
        var _token = obj as CacheTokenItem;
        if ( _token == null )
          return false;

        return ToString().Equals( _token.ToString(), StringComparison.Ordinal );
      }




      public static implicit operator CacheToken( CacheTokenItem item )
      {
        return new CacheToken( item );
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
    /// 合并两个缓存标记
    /// </summary>
    /// <param name="token1"></param>
    /// <param name="token2"></param>
    /// <returns></returns>
    public static CacheToken operator +( CacheToken token1, CacheToken token2 )
    {
      return Combine( token1, token2 );
    }





    public static CacheToken From( string typeName, NameValueCollection values )
    {
      if ( typeName == null )
        throw new ArgumentNullException( "typeName" );

      if ( values == null )
        throw new ArgumentNullException( "values" );


      return From( typeName, values, null );
    }



    public static CacheToken From( string typeName, NameValueCollection values, string[] names )
    {
      if ( typeName == null )
        throw new ArgumentNullException( "typeName" );

      if ( values == null )
        throw new ArgumentNullException( "values" );



      List<string> list = new List<string>();

      string[] keys;

      if ( names.IsNullOrEmpty() )
        keys = values.AllKeys;

      else
        keys = values.AllKeys.Intersect( names, StringComparer.OrdinalIgnoreCase ).ToArray();


      return CreateToken( typeName, keys.Select( k => string.Format( "{0}={1}", k.Replace( "=", "@=" ), values.Get( k ) ) ).ToArray() );

    }


    /// <summary>
    /// 创建 CacheToken
    /// </summary>
    /// <param name="typeName"></param>
    /// <param name="tokens"></param>
    /// <returns></returns>
    public static CacheToken CreateToken( string typeName, params string[] tokens )
    {
      if ( typeName == null )
        throw new ArgumentNullException( "typeName" );

      if ( tokens == null )
        throw new ArgumentNullException( "tokens" );

      return new CacheTokenItem( typeName, tokens );
    }



    /// <summary>
    /// 从路由值中产生缓存标记
    /// </summary>
    /// <param name="routeValues">路由值</param>
    /// <returns></returns>
    public static CacheToken From( RouteValueDictionary routeValues )
    {
      return From( routeValues, null );
    }


    /// <summary>
    /// 从路由值中产生缓存标记
    /// </summary>
    /// <param name="routeValues">路由值</param>
    /// <param name="keys">需要产生缓存标记的路由键</param>
    /// <returns></returns>
    public static CacheToken From( RouteValueDictionary routeValues, params string[] keys )
    {

      if ( routeValues == null )
        throw new ArgumentNullException( "routeValues" );

      IEnumerable<KeyValuePair<string,object>> values = routeValues;

      if ( !keys.IsNullOrEmpty() )
        values = routeValues.Where( pair => keys.Contains( pair.Key, StringComparer.OrdinalIgnoreCase ) );

      else
        values = routeValues;

      return CreateToken( "RouteValue", values.Select( pair => string.Format( "{0}={1}", pair.Key.Replace( "=", "@=" ), pair.Value ) ).ToArray() );
    }



    /// <summary>
    /// 从 Cookies 中产生缓存标记
    /// </summary>
    /// <param name="cookies"></param>
    /// <param name="names"></param>
    /// <returns></returns>
    public static CacheToken From( HttpCookieCollection cookies )
    {
      return From( cookies, null );
    }


    /// <summary>
    /// 从 Cookies 中产生缓存标记
    /// </summary>
    /// <param name="cookies"></param>
    /// <param name="names"></param>
    /// <returns></returns>
    public static CacheToken From( HttpCookieCollection cookies, params string[] names )
    {

      if ( cookies == null )
        throw new ArgumentNullException( "cookies" );

      IEnumerable<HttpCookie> _cookies = cookies.Cast<HttpCookie>();

      if ( !names.IsNullOrEmpty() )
        _cookies = _cookies.Where( c => names.Contains( c.Name, StringComparer.OrdinalIgnoreCase ) );

      return CreateToken( "Cookies", _cookies.Select( c => string.Format( "{0}={1}", c.Name.Replace( "=", "@=" ), c.Value ) ).ToArray() );
    }



    /// <summary>
    /// 合并多个缓存标记
    /// </summary>
    /// <param name="tokens"></param>
    /// <returns></returns>
    public static CacheToken Combine( params CacheToken[] tokens )
    {
      return Combine( tokens.AsEnumerable() );
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

      return new CacheToken( tokens.SelectMany( t => t._tokens ).ToArray() );

    }


    public static CacheToken FromSessionID( HttpContextBase context )
    {
      if ( context == null )
        throw new ArgumentNullException( "context" );

      return new CacheTokenItem( "SessionID", context.Session.SessionID );
    }

    public static CacheToken FromVirtualPath( HttpContextBase context )
    {
      if ( context == null )
        throw new ArgumentNullException( "context" );

      return new CacheTokenItem( "VirtualPath", context.Request.AppRelativeCurrentExecutionFilePath );
    }

    public static CacheToken FromQueryString( HttpContextBase context )
    {
      if ( context == null )
        throw new ArgumentNullException( "context" );

      return FromQueryString( context, null );
    }

    public static CacheToken FromQueryString( HttpContextBase context, string[] keys )
    {
      if ( context == null )
        throw new ArgumentNullException( "context" );

      return From( "QueryString", context.Request.QueryString, keys );
    }



  }
}
