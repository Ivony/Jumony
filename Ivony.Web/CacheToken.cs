using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Routing;
using Ivony.Fluent;
using System.Web;
using System.Collections.Specialized;
using System.Web.Caching;

namespace Ivony.Web
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

      VaryHeaders = tokens.Select( t => t.VaryHeader ).NotNull().Distinct( StringComparer.OrdinalIgnoreCase ).ToArray();

    }



    /// <summary>
    /// 获取缓存依赖项
    /// </summary>
    public ICacheDependency CacheDependency
    {
      get { return null; }
    }


    /// <summary>
    /// 获取客户端缓存依赖头
    /// </summary>
    public string[] VaryHeaders
    {
      get;
      private set;
    }




    private string _tokenString;

    private void EnsureTokenString()
    {
      if ( _tokenString == null )
      {
        _tokenString = string.Join( "+", _tokens.Select( t => t.ToString() ).ToArray() );
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

      /// <summary>
      /// 类型名
      /// </summary>
      public string TypeName
      {
        get;
        private set;
      }

      /// <summary>
      /// 缓存依赖项
      /// </summary>
      public ICacheDependency CacheDependency
      {
        get { return null; }
      }

      /// <summary>
      /// 客户端缓存依赖头
      /// </summary>
      public string VaryHeader
      {
        get;
        private set;
      }



      private string[] _tokens;

      public CacheTokenItem( string type, string[] tokens, ICacheDependency cacheDependency, string varyHeader )
      {
        TypeName = type;
        _tokens = tokens;

        VaryHeader = varyHeader;

      }

      private string _tokenString;

      private void EnsureTokenString()
      {
        if ( _tokenString == null )
        {
          _tokenString = TypeName.Replace( ":", "@:" ) + ":" + string.Join( ";", _tokens.Select( t => t.Replace( "@", "@@" ).Replace( ";", "@;" ) ).ToArray() );
          _tokenString = _tokenString.Replace( "+", "@+" );

        }
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
    /// <param name="token1">要合并的缓存标记</param>
    /// <param name="token2">要合并的另一个缓存标记</param>
    /// <returns>合并后的缓存标记</returns>
    public static CacheToken operator +( CacheToken token1, CacheToken token2 )
    {
      return Combine( token1, token2 );
    }


    /// <summary>
    /// 确定两个缓存标记是否相等
    /// </summary>
    /// <param name="token1">要比较的缓存标记</param>
    /// <param name="token2">要比较的另一个缓存标记</param>
    /// <returns></returns>
    public static bool operator ==( CacheToken token1, CacheToken token2 )
    {

      return Equals( token1, token2 );
    }

    /// <summary>
    /// 确定两个缓存标记是否不相等
    /// </summary>
    /// <param name="token1">要比较的缓存标记</param>
    /// <param name="token2">要比较的另一个缓存标记</param>
    /// <returns></returns>
    public static bool operator !=( CacheToken token1, CacheToken token2 )
    {
      return !Equals( token1, token2 );
    }






    internal static CacheToken From( string typeName, NameValueCollection values )
    {
      if ( typeName == null )
        throw new ArgumentNullException( "typeName" );

      if ( values == null )
        throw new ArgumentNullException( "values" );


      return From( typeName, values, null );
    }



    internal static CacheToken From( string typeName, NameValueCollection values, string[] names )
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
    /// <param name="typeName">缓存标记类别名称</param>
    /// <param name="tokens">用于标识的字符串</param>
    /// <returns>创建的 CacheToken</returns>
    public static CacheToken CreateToken( string typeName, params string[] tokens )
    {
      return CreateToken( typeName, null, null, tokens );
    }

    /// <summary>
    /// 创建 CacheToken
    /// </summary>
    /// <param name="cacheDependency">缓存依赖项</param>
    /// <param name="varyHeader">客户端缓存依赖头</param>
    /// <param name="typeName">缓存标记类别名称</param>
    /// <param name="tokens">用于标识的字符串</param>
    /// <returns>创建的 CacheToken</returns>
    public static CacheToken CreateToken( string typeName, ICacheDependency cacheDependency, string varyHeader, string[] tokens )
    {
      if ( typeName == null )
        throw new ArgumentNullException( "typeName" );

      if ( tokens == null )
        throw new ArgumentNullException( "tokens" );

      return new CacheTokenItem( typeName, tokens, cacheDependency, varyHeader );
    }







    /// <summary>
    /// 合并多个缓存标记
    /// </summary>
    /// <param name="tokens">要合并的缓存标记列表</param>
    /// <returns>合并后的缓存标记</returns>
    public static CacheToken Combine( params CacheToken[] tokens )
    {
      return Combine( tokens.AsEnumerable() );
    }

    /// <summary>
    /// 合并多个缓存标记
    /// </summary>
    /// <param name="tokens">要合并的缓存标记列表</param>
    /// <returns>合并后的缓存标记</returns>
    public static CacheToken Combine( IEnumerable<CacheToken> tokens )
    {

      if ( tokens.IsNullOrEmpty() )
        return null;

      tokens = tokens.NotNull();

      CacheToken result;
      if ( tokens.IsSingle( out result ) )
        return result;

      return new CacheToken( tokens.SelectMany( t => t._tokens ).ToArray() );

    }

                                                      


    /// <summary>
    /// 从路由值中产生缓存标记
    /// </summary>
    /// <param name="context">当前 HTTP 请求上下文</param>
    /// <returns>产生的缓存标记</returns>
    public static CacheToken FromRouteValues( RequestContext context )
    {
      if ( context == null )
        throw new ArgumentNullException( "context" );

      return FromRouteValues( context, null );
    }


    /// <summary>
    /// 从路由值中产生缓存标记
    /// </summary>
    /// <param name="context">当前 HTTP 请求上下文</param>
    /// <param name="keys">要产生缓存标记的路由键</param>
    /// <returns></returns>
    public static CacheToken FromRouteValues( RequestContext context, params string[] keys )
    {
      if ( context == null )
        throw new ArgumentNullException( "context" );

      return FromRouteValues( context.RouteData.Values, keys );
    }


    /// <summary>
    /// 从路由值中产生缓存标记
    /// </summary>
    /// <param name="routeValues">路由值</param>
    /// <returns></returns>
    public static CacheToken FromRouteValues( RouteValueDictionary routeValues )
    {
      if ( routeValues == null )
        throw new ArgumentNullException( "routeValues" );

      return FromRouteValues( routeValues, null );
    }


    /// <summary>
    /// 从路由值中产生缓存标记
    /// </summary>
    /// <param name="routeValues">路由值</param>
    /// <param name="keys">需要产生缓存标记的路由键</param>
    /// <returns></returns>
    public static CacheToken FromRouteValues( RouteValueDictionary routeValues, params string[] keys )
    {

      if ( routeValues == null )
        throw new ArgumentNullException( "routeValues" );

      IEnumerable<KeyValuePair<string, object>> values = routeValues;

      if ( !keys.IsNullOrEmpty() )
        values = routeValues.Where( pair => keys.Contains( pair.Key, StringComparer.OrdinalIgnoreCase ) );

      else
        values = routeValues;

      return CreateToken( "RouteValue", values.Select( pair => string.Format( "{0}={1}", pair.Key.Replace( "=", "@=" ), pair.Value ) ).ToArray() );
    }



    /// <summary>
    /// 从 Cookies 中产生缓存标记
    /// </summary>
    /// <param name="context">当前 HTTP 请求上下文</param>
    /// <returns>产生的缓存标记</returns>
    public static CacheToken FromCookies( HttpContextBase context )
    {
      return FromCookies( context, null );
    }


    /// <summary>
    /// 从 Cookies 中产生缓存标记
    /// </summary>
    /// <param name="context">当前 HTTP 请求上下文</param>
    /// <param name="names">要产生缓存标记的 Cookie 名</param>
    /// <returns>产生的缓存标记</returns>
    public static CacheToken FromCookies( HttpContextBase context, params string[] names )
    {

      if ( context == null )
        throw new ArgumentNullException( "context" );

      var cookies = context.Request.Cookies;


      var keys = cookies.AllKeys;

      if ( !names.IsNullOrEmpty() )
        keys = keys.Intersect( names, StringComparer.OrdinalIgnoreCase ).ToArray();

      var values = keys
        .Select( key => string.Format( "{0}={1}", key.Replace( "=", "@=" ), cookies[key].Value ) )
        .ToArray();

      return CreateToken( "Cookies", null, "Set-Cookie", values );
    }




    /// <summary>
    /// 从 SessionID 中创建缓存标记
    /// </summary>
    /// <param name="context">当前 HTTP 请求上下文</param>
    /// <returns>创建的缓存标记</returns>
    public static CacheToken FromSessionID( HttpContextBase context )
    {
      if ( context == null )
        throw new ArgumentNullException( "context" );

      return CreateToken( "SessionID", null, "Set-Cookie", new[] { context.Session.SessionID } );
    }


    /// <summary>
    /// 从虚拟路径中创建缓存标记
    /// </summary>
    /// <param name="context">当前 HTTP 请求上下文</param>
    /// <returns>创建的缓存标记</returns>
    public static CacheToken FromVirtualPath( HttpContextBase context )
    {
      if ( context == null )
        throw new ArgumentNullException( "context" );

      return CreateToken( "VirtualPath", context.Request.AppRelativeCurrentExecutionFilePath );
    }


    /// <summary>
    /// 从当前请求的查询字符串中创建缓存标记
    /// </summary>
    /// <param name="context">当前 HTTP 请求上下文</param>
    /// <returns>创建的缓存标记</returns>
    public static CacheToken FromQueryString( HttpContextBase context )
    {
      if ( context == null )
        throw new ArgumentNullException( "context" );

      return FromQueryString( context, null );
    }


    /// <summary>
    /// 从当前请求的查询字符串中创建缓存标记
    /// </summary>
    /// <param name="context">当前 HTTP 请求上下文</param>
    /// <param name="keys">要创建缓存标记的查询数据键</param>
    /// <returns>创建的缓存标记</returns>
    public static CacheToken FromQueryString( HttpContextBase context, params string[] keys )
    {
      if ( context == null )
        throw new ArgumentNullException( "context" );

      return From( "QueryString", context.Request.QueryString, keys );
    }



  }
}
