using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Routing;
using Ivony.Fluent;

namespace Ivony.Web
{
  /// <summary>
  /// 简单路由规则，定义简单路由表的路由规则
  /// </summary>
  public sealed class SimpleRouteRule
  {

    /// <summary>定义匹配静态路径段的正则表达式</summary>
    public const string staticParagraphPattern = @"(?<paragraph>[\p{Lu}\p{Ll}\p{Nd}-.]+)";
    /// <summary>定义匹配动态路径段的正则表达式</summary>
    public const string dynamicParagraphPattern = @"(?<paragraph>\{[\p{Lu}\p{Ll}\p{Nd}]+\})";
    /// <summary>定义匹配 URL 模式的正则表达式</summary>
    public static readonly string urlPattern = @"(^~/$)|(^~((/{static}(/{static})*(/{dynamic})*)|((/{dynamic})+))$)".Replace( "{static}", staticParagraphPattern ).Replace( "{dynamic}", dynamicParagraphPattern );

    private static readonly Regex urlPatternRegex = new Regex( urlPattern, RegexOptions.Compiled );


    /// <summary>
    /// 创建一个简单路由规则
    /// </summary>
    /// <param name="name">规则名称</param>
    /// <param name="urlPattern">URL 模式</param>
    /// <param name="routeValues">静态/默认路由值</param>
    /// <param name="queryKeys">可用于 QueryString 的参数</param>
    internal SimpleRouteRule( string name, string urlPattern, IDictionary<string, string> routeValues, string[] queryKeys )
    {
      Name = name;

      if ( queryKeys == null )
      {
        LimitedQueries = false;
        queryKeys = new string[0];
      }
      else
        LimitedQueries = true;

      DataTokens = new RouteValueDictionary();


      var match = urlPatternRegex.Match( urlPattern );

      if ( !match.Success )
        throw new FormatException( "URL模式格式不正确" );

      _paragraphes = match.Groups["paragraph"].Captures.Cast<Capture>().Select( c => c.Value ).ToArray();

      _urlPattern = urlPattern;
      _staticValues = new Dictionary<string, string>( routeValues, StringComparer.OrdinalIgnoreCase );


      _routeKeys = new HashSet<string>( _staticValues.Keys, StringComparer.OrdinalIgnoreCase );

      _dynamics = new HashSet<string>( _paragraphes.Where( p => p.StartsWith( "{" ) && p.EndsWith( "}" ) ).Select( p => p.Substring( 1, p.Length - 2 ) ), StringComparer.OrdinalIgnoreCase );



      foreach ( var key in _dynamics )
      {
        if ( _routeKeys.Contains( key ) )
          throw new FormatException( "URL模式格式不正确，包含重复的动态参数名或动态参数名与预设路由键重复" );


        _routeKeys.Add( key );
      }

      if ( _routeKeys.Intersect( queryKeys, StringComparer.OrdinalIgnoreCase ).Any() )
        throw new FormatException( "URL模式格式不正确，动态参数或预设路由键与可选查询字符串名重复" );

      _queryKeys = new HashSet<string>( queryKeys, StringComparer.OrdinalIgnoreCase );

      _allKeys = new HashSet<string>( _routeKeys, StringComparer.OrdinalIgnoreCase );
      _allKeys.UnionWith( _queryKeys );

    }


    /// <summary>
    /// 路由规则的名称
    /// </summary>
    public string Name
    {
      get;
      private set;
    }


    /// <summary>
    /// 是否限制产生的 QueryString 不超过指定范围（查询键）
    /// </summary>
    public bool LimitedQueries
    {
      get;
      private set;
    }


    /// <summary>
    /// 指示路由规则是否为单向的，单向路由只路由请求，不产生虚拟路径。
    /// </summary>
    public bool Oneway
    {
      get;
      private set;
    }


    private string[] _paragraphes;

    /// <summary>
    /// 获取所有路径段
    /// </summary>
    public string[] Paragraphes
    {
      get { return _paragraphes.Copy(); }
    }


    private HashSet<string> _routeKeys;

    /// <summary>
    /// 获取所有路由键（包括静态和动态的）
    /// </summary>
    /// <remarks>
    /// 路由键的值会作为虚拟路径的一部分
    /// </remarks>
    public string[] RouteKeys
    {
      get { return _routeKeys.ToArray(); }
    }



    private HashSet<string> _queryKeys;

    /// <summary>
    /// 获取所有查询键
    /// </summary>
    /// <remarks>
    /// 构造虚拟路径时，查询键都是可选的。
    /// 查询键的值会被产生为查询字符串。
    /// </remarks>
    public string[] QueryKeys
    {
      get { return _queryKeys.ToArray(); }
    }


    private HashSet<string> _allKeys;

    /// <summary>
    /// 获取所有键（包括路由键和查询键）
    /// </summary>
    public string[] AllKeys
    {
      get
      {
        return _allKeys.ToArray();
      }
    }


    private HashSet<string> _dynamics;

    /// <summary>
    /// 获取所有动态路由键
    /// </summary>
    /// <remarks>
    /// 动态路由键的值不能包含特殊字符
    /// </remarks>
    public string[] DynamicRouteKeys
    {
      get { return _dynamics.ToArray(); }
    }


    private string _prefix;

    /// <summary>
    /// 获取URL模式的静态前缀
    /// </summary>
    public string StaticPrefix
    {
      get
      {
        if ( _prefix == null )
        {
          var dynamicStarts = _urlPattern.IndexOf( '{' );
          if ( dynamicStarts < 0 )
            _prefix = _urlPattern;
          else
            _prefix = _urlPattern.Substring( 0, dynamicStarts - 1 );
        }

        return _prefix;
      }
    }


    private string _urlPattern;

    /// <summary>
    /// 获取整个URL模式
    /// </summary>
    public string UrlPattern
    {
      get { return _urlPattern; }
    }


    private IDictionary<string, string> _staticValues;

    /// <summary>
    /// 获取所有的静态值
    /// </summary>
    public IDictionary<string, string> StaticRouteValues
    {
      get { return new Dictionary<string, string>( _staticValues ); }
    }


    /// <summary>
    /// 根据路由值创建虚拟路径
    /// </summary>
    /// <param name="routeValues">路由值</param>
    /// <returns>创建的虚拟路径</returns>
    public string CreateVirtualPath( IDictionary<string, string> routeValues )
    {

      var builder = new StringBuilder( StaticPrefix );

      foreach ( var key in DynamicRouteKeys )
      {
        var value = routeValues[key];

        if ( string.IsNullOrEmpty( value ) )
          throw new ArgumentException( "作为动态路径的路由值不能包含空引用或空字符串", "routeValues" );


        if ( value.Contains( '/' ) )
          throw new ArgumentException( "作为动态路径的路由值不能包含路径分隔符 '/'", "routeValues" );

        //value = HttpUtility.UrlEncode( value, RoutingTable.UrlEncoding );

        builder.Append( "/" + value );

      }


      bool isAppendedQueryStartSymbol = false;

      var unallocatedKeys = routeValues.Keys.Except( RouteKeys, StringComparer.OrdinalIgnoreCase );


      foreach ( var key in unallocatedKeys )
      {

        if ( QueryKeys.Contains( key, StringComparer.OrdinalIgnoreCase ) || !LimitedQueries )
        {

          var value = routeValues[key];

          if ( !isAppendedQueryStartSymbol )
            builder.Append( '?' );
          else
            builder.Append( '&' );

          isAppendedQueryStartSymbol = true;

          builder.Append( HttpUtility.UrlEncode( key ) );
          builder.Append( '=' );
          builder.Append( HttpUtility.UrlEncode( routeValues[key] ) );

        }
      }


      return builder.ToString();
    }


    /// <summary>
    /// 规则所属的简单路由表实例
    /// </summary>
    public SimpleRouteTable SimpleRouteTable
    {
      get;
      internal set;
    }


    /// <summary>
    /// 检查指定的路由值是否满足约束
    /// </summary>
    /// <param name="values">路由值</param>
    /// <returns>是否满足路由规则的约束</returns>
    public bool IsMatch( IDictionary<string, string> values )
    {

      if ( values == null )
        throw new ArgumentNullException( "values" );


      foreach ( var key in _staticValues.Keys )
      {
        string val;

        if ( !values.TryGetValue( key, out val ) )
          return false;

        if ( !_staticValues[key].EqualsIgnoreCase( val ) )
          return false;
      }

      return true;
    }



    private object _sync = new object();


    private string _virtualPathDescriptor;

    internal string GetVirtualPathDescriptor()
    {
      lock ( _sync )
      {
        if ( _virtualPathDescriptor != null )
          return _virtualPathDescriptor;

        return _virtualPathDescriptor = StaticPrefix + string.Join( "", Enumerable.Repeat( "/{dynamic}", DynamicRouteKeys.Length ).ToArray() );
      }
    }


    private string _routeValuesDescriptor;

    internal string GetRouteValuesDescriptor()
    {
      lock ( _sync )
      {
        if ( _routeValuesDescriptor != null )
          return _routeValuesDescriptor;

        List<string> list = new List<string>();

        foreach ( var key in RouteKeys.OrderBy( k => k, StringComparer.OrdinalIgnoreCase ) )
        {
          string value;

          if ( StaticRouteValues.TryGetValue( key, out value ) )
            list.Add( string.Format( "<\"{0}\",\"{1}\">", key.Replace( "\"", "\\\"" ), value.Replace( "\"", "\\\"" ) ) );

          else
            list.Add( string.Format( "<\"{0}\",dynamic>", key.Replace( "\"", "\\\"" ) ) );
        }

        return _routeValuesDescriptor = string.Join( ",", list.ToArray() );
      }
    }



    /// <summary>
    /// 检查两个路由规则是否互斥。
    /// </summary>
    /// <param name="rule1">路由规则1</param>
    /// <param name="rule2">路由规则2</param>
    /// <returns></returns>
    public static bool Mutex( SimpleRouteRule rule1, SimpleRouteRule rule2 )
    {

      var KeySet = new HashSet<string>( rule1.StaticRouteValues.Keys, StringComparer.OrdinalIgnoreCase );
      var KeySet2 = new HashSet<string>( rule2.StaticRouteValues.Keys, StringComparer.OrdinalIgnoreCase );

      KeySet.IntersectWith( KeySet2 );

      return KeySet.Any( key => !rule1.StaticRouteValues[key].EqualsIgnoreCase( rule2.StaticRouteValues[key] ) );
    }


    /// <summary>
    /// 比较两个路由规则约束是否一致
    /// </summary>
    /// <param name="rule">要比较的路由规则</param>
    /// <returns>两个规则的约束是否一致</returns>
    public bool EqualsConstraints( SimpleRouteRule rule )
    {

      if ( rule == null )
        throw new ArgumentNullException( "rule" );


      if ( rule.StaticRouteValues.Count != StaticRouteValues.Count )
        return false;

      return IsMatch( rule.StaticRouteValues );

    }



    private static Regex multipleSlashRegex = new Regex( "/+", RegexOptions.Compiled );


    /// <summary>
    /// 获取路由值
    /// </summary>
    /// <param name="virtualPath">当前请求的虚拟路径</param>
    /// <param name="queryString">当前请求的查询数据</param>
    /// <returns></returns>
    public IDictionary<string, string> GetRouteValues( string virtualPath, NameValueCollection queryString )
    {

      if ( virtualPath == null )
        throw new ArgumentNullException( "virtualPath" );

      if ( !VirtualPathUtility.IsAppRelative( virtualPath ) )
        throw new ArgumentException( "virtualPath 只能使用应用程序根相对路径，即以 \"~/\" 开头的路径，调用 VirtualPathUtility.ToAppRelative 方法或使用 HttpRequest.AppRelativeCurrentExecutionFilePath 属性获取", "virtualPath" );



      var queryKeySet = new HashSet<string>( _queryKeys, StringComparer.OrdinalIgnoreCase );
      var requestQueryKeySet = new HashSet<string>( queryString.AllKeys, StringComparer.OrdinalIgnoreCase );

      if ( LimitedQueries && !queryKeySet.IsSupersetOf( requestQueryKeySet ) )//如果限制了查询键并且查询键集合没有完全涵盖所有传进来的QueryString键的话，即存在有一个QueryString键不在查询键集合中，则这条规则不适用。
        return null;



      virtualPath = multipleSlashRegex.Replace( virtualPath, "/" );//将连续的/替换成单独的/
      var extensionLength = VirtualPathUtility.GetExtension( virtualPath ).Length;
      if ( extensionLength > 0 )
        virtualPath = virtualPath.Remove( virtualPath.Length - extensionLength );//去除扩展名


      virtualPath = virtualPath.Substring( 2 );
      virtualPath = VirtualPathUtility.RemoveTrailingSlash( virtualPath ) ?? "";//在虚拟路径最后移除 / ，使得 xxx/ 与 xxx 被视为同一路径。


      var pathParagraphs = virtualPath.Split( '/' );

      if ( virtualPath == "" )
        pathParagraphs = new string[0];


      if ( pathParagraphs.Length != Paragraphes.Length )//路径段长度不一致，规则不适用
        return null;


      var values = new Dictionary<string, string>( StringComparer.OrdinalIgnoreCase );



      foreach ( var pair in _staticValues )
        values.Add( pair.Key, pair.Value );

      for ( int i = 0; i < pathParagraphs.Length; i++ )
      {

        var paragraph = Paragraphes[i];

        if ( !paragraph.StartsWith( "{" ) )
        {
          if ( !pathParagraphs[i].EqualsIgnoreCase( paragraph ) )//静态路径段不符，规则不适用
            return null;
        }
        else
        {
          var name = paragraph.Substring( 1, paragraph.Length - 2 );
          values.Add( name, pathParagraphs[i] );
        }
      }



      if ( !LimitedQueries )//如果没有限制查询键，但传进来的查询键与现有路由键有任何冲突，则这条规则不适用。
      {                     //因为如果限制了查询键，则上面会确保查询键不超出限制的范围，且查询键的范围与路由键范围不可能重合（构造函数限定），也就不可能存在冲突。
        requestQueryKeySet.IntersectWith( _routeKeys );
        if ( requestQueryKeySet.Any() )
          return null;
      }


      foreach ( var key in queryString.AllKeys )
      {
        if ( key == null )
          continue;//因某些未知原因会导致 AllKeys 包含空键值。

        var v = queryString[key];
        if ( v != null )
          values.Add( key, v );
      }


      return values;
    }


    /// <summary>
    /// 获取一个字符串，其描述了这个简单路由规则。
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
      return UrlPattern + " : " + "{" + GetRouteValuesDescriptor() + "}";
    }


    /// <summary>
    /// 获取简单路由规则的扩展数据标记
    /// </summary>
    public RouteValueDictionary DataTokens
    {
      get;
      private set;
    }



  }
}
