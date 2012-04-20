using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Routing;
using System.Text.RegularExpressions;
using System.Web;
using Ivony.Fluent;
using System.Collections.Specialized;
using System.Web.Caching;
using System.Web.Hosting;
using System.Web.Mvc;

namespace Ivony.Html.Web.Mvc
{

  /// <summary>
  /// 简单路由表，提供简单的路由服务
  /// </summary>
  public class SimpleRouteTable : RouteBase
  {


    /// <summary>
    /// 定义通过路由值获取虚拟路径的缓存键前缀。
    /// </summary>
    protected const string RouteValuesCacheKeyPrefix = "RouteValues_";
    /// <summary>
    /// 定义通过虚拟路径获取路由值的缓存键前缀。
    /// </summary>
    protected const string RouteUrlCacheKeyPrefix = "RouteVirtualPath_";


    /// <summary>
    /// 获取请求的路由数据
    /// </summary>
    /// <param name="httpContext">HTTP 请求</param>
    /// <returns>路由数据</returns>
    public override RouteData GetRouteData( HttpContextBase httpContext )
    {

      var virtualPath = httpContext.Request.AppRelativeCurrentExecutionFilePath + httpContext.Request.PathInfo;

      var cacheKey = RouteUrlCacheKeyPrefix + httpContext.Request.Url.AbsoluteUri;

      var routeData = httpContext.Cache.Get( cacheKey ) as RouteData;


      if ( routeData != null )
        return CloneRouteData( routeData );


      var data = _rules
        .OrderBy( r => r.DynamicRouteKyes.Length )
        .Select( r => new
          {
            Rule = r,
            Values = r.GetRouteValues( virtualPath, httpContext.Request.QueryString ),
          } )
        .Where( i => i.Values != null )
        .FirstOrDefault();

      if ( data == null )
        return null;


      routeData = new RouteData( this, Handler );

      foreach ( var pair in data.Values )
        routeData.Values.Add( pair.Key, pair.Value );

      foreach ( var pair in data.Rule.DataTokens )
        routeData.DataTokens.Add( pair.Key, pair.Value );

      routeData.DataTokens["RoutingRuleName"] = data.Rule.Name;

      httpContext.Cache.Insert( cacheKey, routeData );

      return CloneRouteData( routeData );

    }


    /// <summary>
    /// 创建 RouteData 的副本
    /// </summary>
    /// <param name="routeData">要创建副本的 RouteData</param>
    /// <returns>创建的副本</returns>
    public static RouteData CloneRouteData( RouteData routeData )
    {
      var clone = new RouteData( routeData.Route, routeData.RouteHandler );

      foreach ( var key in routeData.Values.Keys )
      {
        clone.Values.Add( key, routeData.Values[key] );
      }

      foreach ( var key in routeData.DataTokens.Keys )
      {
        clone.DataTokens.Add( key, routeData.DataTokens[key] );
      }

      return clone;
    }




    /// <summary>
    /// 尝试从路由值创建虚拟路径
    /// </summary>
    /// <param name="requestContext">当前请求上下文</param>
    /// <param name="values">路由值</param>
    /// <returns>虚拟路径信息</returns>
    public override VirtualPathData GetVirtualPath( RequestContext requestContext, RouteValueDictionary values )
    {

      var cache = requestContext.HttpContext.Cache;


      var _values = values.ToDictionary( pair => pair.Key, pair => pair.Value == null ? null : pair.Value.ToString(), StringComparer.OrdinalIgnoreCase );

      var cacheKey = CreateCacheKey( _values );

      var virtualPath = cache.Get( cacheKey ) as string;

      if ( virtualPath != null )
        return new VirtualPathData( this, virtualPath );


      var keySet = new HashSet<string>( _values.Keys, StringComparer.OrdinalIgnoreCase );


      var candidateRules = _rules
        .Where( r => !r.Oneway )                                               //不是单向路由规则
        .Where( r => keySet.IsSupersetOf( r.RouteKeys ) )                      //所有路由键都必须匹配
        .Where( r => keySet.IsSubsetOf( r.AllKeys ) || !r.LimitedQueries )     //所有路由键和查询字符串键必须能涵盖要设置的键。
        .Where( r => r.IsMatch( _values ) )                                    //必须满足路由规则所定义的路由数据。
        .ToArray();

      if ( !candidateRules.Any() )
        return null;


      var bestRule = BestRule( candidateRules );

      virtualPath = bestRule.CreateVirtualPath( _values );

      if ( MvcCompatible )
        virtualPath = virtualPath.Substring( 2 );



      cache.Insert( cacheKey, virtualPath, CacheItemPriority.AboveNormal );


      var data = new VirtualPathData( this, virtualPath );


      foreach ( var pair in bestRule.DataTokens )
        data.DataTokens.Add( pair.Key, pair.Value );

      data.DataTokens["RoutingRuleName"] = bestRule.Name;

      return data;
    }

    /// <summary>
    /// 创建缓存键
    /// </summary>
    /// <param name="values">要创建缓存键的字典</param>
    /// <returns>创建的缓存键</returns>
    protected virtual string CreateCacheKey( Dictionary<string, string> values )
    {

      StringBuilder builder = new StringBuilder( RouteValuesCacheKeyPrefix );

      foreach ( var key in values.Keys )
      {
        var val = values[key];

        builder.Append( key.Replace( "\\", "\\\\" ).Replace( ":", "\\:" ).Replace( ";", "\\;" ) );
        builder.Append( ":" );

        if ( val == null )
          builder.Append( "@" );

        else
          builder.Append( val.Replace( "\\", "\\\\" ).Replace( ":", "\\:" ).Replace( ";", "\\;" ).Replace( "@", "\\@" ) );

        builder.Append( ";" );
      }

      return builder.ToString();
    }


    /// <summary>
    /// 添加一个路由规则
    /// </summary>
    /// <param name="name">规则名称</param>
    /// <param name="urlPattern">URL 模式</param>
    /// <param name="routeValues">静态/默认路由值</param>
    /// <param name="queryKeys">可用于 QueryString 的参数，若为null则表示无限制</param>
    public virtual SimpleRouteRule AddRule( string name, string urlPattern, IDictionary<string, string> routeValues, string[] queryKeys )
    {

      if ( urlPattern == null )
        throw new ArgumentNullException( "urlPattern" );

      if ( routeValues == null )
        throw new ArgumentNullException( "routeValues" );

      var rule = new SimpleRouteRule( name, urlPattern, routeValues, queryKeys );

      return AddRule( rule );
    }

    /// <summary>
    /// 添加一个路由规则
    /// </summary>
    /// <param name="rule">路由规则</param>
    protected virtual SimpleRouteRule AddRule( SimpleRouteRule rule )
    {

      lock ( Routes )
      {
        if ( Routes != null && !Routes.Contains( this ) )
          Routes = null;

        if ( Routes == null )
          throw new InvalidOperationException( "简单路由表实例并未被正确的注册在一个路由集合中，无法检测与路由集合中其他简单路由表是否存在冲突，所以不能添加简单路由规则" );

        var conflictRule = Routes.CheckConflict( rule );

        if ( conflictRule != null )
          throw new InvalidOperationException( string.Format( "添加规则\"{0}\"失败，路由表 \"{1}\" 中已经存在一条可能冲突的规则：\"{2}\"", rule.Name, conflictRule.SimpleRouteTable.Name, conflictRule.Name ) );
      }


      _rules.Add( rule );

      rule.SimpleRouteTable = this;

      return rule;
    }

    /// <summary>
    /// 检查指定规则是否与简单路由表现存的任何规则冲突，若有冲突，返回与其冲突的规则。
    /// </summary>
    /// <param name="rule">要检查冲突的规则</param>
    /// <returns>如果现存规则与检查的规则存在一个冲突，则返回冲突的规则。</returns>
    public SimpleRouteRule CheckConflict( SimpleRouteRule rule )
    {
      //验证 GetVirtualPath 时可能的冲突
      {
        var conflictRule = _rules
          .Where( r => r.RouteKeys.Length == rule.RouteKeys.Length )                    //若通过RouteKey多寡无法区分
          .Where( r => r.DynamicRouteKyes.Length == rule.DynamicRouteKyes.Length )      //若通过动态路径段多寡也无法区分
          .Where( r => !SimpleRouteRule.Mutex( r, rule ) )                              //若与现存规则不互斥
          .FirstOrDefault();

        if ( conflictRule != null )
          return conflictRule;
      }

      //验证 GetRouteData 时可能的冲突
      {
        var conflictRule = _rules
          .Where( r => r.Paragraphes.Length == rule.Paragraphes.Length )                //若路径段长度一致
          .Where( r => r.StaticPrefix.EqualsIgnoreCase( rule.StaticPrefix ) )           //若静态段也一致
          .FirstOrDefault();


        if ( conflictRule != null )
          return conflictRule;
      }

      return null;
    }


    private SimpleRouteRule BestRule( IEnumerable<SimpleRouteRule> candidateRules )
    {

      SimpleRouteRule bestRule;


      //满足最多静态值的被优先考虑
      candidateRules = candidateRules.GroupBy( r => r.StaticRouteValues.Count ).OrderByDescending( group => group.Key ).First().ToArray();

      if ( candidateRules.IsSingle( out bestRule ) )
        return bestRule;


      //拥有最多路由键的被优先考虑
      candidateRules = candidateRules.GroupBy( r => r.RouteKeys.Length ).OrderByDescending( group => group.Key ).First().ToArray();

      if ( candidateRules.IsSingle( out bestRule ) )
        return bestRule;


      //拥有最少动态参数的被优先考虑
      candidateRules = candidateRules.GroupBy( p => p.DynamicRouteKyes.Length ).OrderBy( group => group.Key ).First().ToArray();

      if ( candidateRules.IsSingle( out bestRule ) )
        return bestRule;

      else
        return null;

    }


    /// <summary>
    /// 获取简单路由表实例名称
    /// </summary>
    public string Name
    {
      get;
      private set;
    }



    /// <summary>
    /// 获取简单路由表所属的路由集合
    /// </summary>
    public RouteCollection Routes
    {
      get;
      internal set;
    }


    /// <summary>
    /// 创建一个简单路由表实例
    /// </summary>
    /// <param name="name">简单路由表名称</param>
    /// <param name="handler">处理路由请求的对象</param>
    /// <param name="mvcCompatible">是否产生MVC兼容的虚拟路径（去除~/）</param>
    public SimpleRouteTable( string name, IRouteHandler handler, bool mvcCompatible )
    {
      Name = name;
      Handler = handler;
      MvcCompatible = mvcCompatible;
      UrlEncoding = Encoding.UTF8;
    }


    internal SimpleRouteTable()
      : this( "BuiltIn", new MvcRouteHandler(), true )
    {
      IsBuiltIn = true;
    }

    internal bool IsBuiltIn { get; set; }




    /// <summary>
    /// 是否产生MVC兼容的虚拟路径（去除~/）
    /// </summary>
    public bool MvcCompatible
    {
      get;
      private set;
    }




    private ICollection<SimpleRouteRule> _rules = new List<SimpleRouteRule>();

    /// <summary>
    /// 路由表中定义的路由规则
    /// </summary>
    public SimpleRouteRule[] Rules
    {
      get
      {
        return _rules.ToArray();
      }
    }


    /// <summary>
    /// 处理路由请求的对象
    /// </summary>
    public IRouteHandler Handler { get; private set; }

    /// <summary>
    /// 获取 URL 默认编码格式
    /// </summary>
    public Encoding UrlEncoding { get; private set; }


  }


  /// <summary>
  /// 用于 MVC 的简单区域路由表，提供某一区域的简单路由服务
  /// </summary>
  public sealed class SimpleAreaRouteTable : SimpleRouteTable, IRouteWithArea
  {


    /// <summary>
    /// 构建简单区域路由表对象
    /// </summary>
    /// <param name="areaName">区域名</param>
    /// <param name="namespaces">区域所要搜索的命名空间</param>
    /// <param name="useNamespaceFallback"></param>
    internal SimpleAreaRouteTable( string areaName, string[] namespaces, bool useNamespaceFallback )
      : base( "Area_" + areaName, new MvcRouteHandler(), true )
    {
      AreaName = areaName;
      Namespaces = namespaces;
      UseNamespaceFallback = useNamespaceFallback || true;
    }

    /// <summary>
    /// 获取路由表所适用的区域名
    /// </summary>
    public string AreaName
    {
      get;
      private set;
    }

    string IRouteWithArea.Area
    {
      get { return AreaName; }
    }



    /// <summary>
    /// 区域所要搜索的命名空间
    /// </summary>
    public string[] Namespaces
    {
      get;
      private set;
    }



    public bool UseNamespaceFallback
    {
      get;
      private set;
    }




    /// <summary>
    /// 添加一个路由规则
    /// </summary>
    /// <param name="name">规则名称</param>
    /// <param name="urlPattern">URL 模式</param>
    /// <param name="routeValues">静态/默认路由值</param>
    /// <param name="queryKeys">可用于 QueryString 的参数</param>
    /// <param name="limitedQueries">是否限制产生的 QueryString ，使其不产生在指定之外的路由参数</param>
    /// <returns>创建的简单路由规则</returns>
    /// <remarks>
    /// 简单区域路由表会自动为路由规则增加一个静态路由值 area 保存当前区域名。
    /// </remarks>
    public override SimpleRouteRule AddRule( string name, string urlPattern, IDictionary<string, string> routeValues, string[] queryKeys )
    {

      var _routeValues = new Dictionary<string, string>( routeValues, StringComparer.OrdinalIgnoreCase );

      if ( _routeValues.ContainsKey( "area" ) )
        throw new InvalidOperationException( "静态路由值不能包含 area" );

      _routeValues.Add( "area", AreaName );

      return base.AddRule( name, urlPattern, _routeValues, queryKeys );
    }


    /// <summary>
    /// 获取请求的路由数据
    /// </summary>
    /// <param name="httpContext">HTTP 请求</param>
    /// <returns>路由数据</returns>
    /// <remarks>
    /// 简单区域路由表获取路由数据后会自动设置区域所需的 DataTokens
    /// </remarks>
    public override RouteData GetRouteData( HttpContextBase httpContext )
    {
      var routeData = base.GetRouteData( httpContext );

      if ( routeData != null )
      {
        routeData.Values.Remove( "area" );
        routeData.DataTokens["area"] = AreaName;
        routeData.DataTokens["Namespaces"] = Namespaces;
        routeData.DataTokens["UseNamespaceFallback"] = UseNamespaceFallback;
      }

      return routeData;
    }



    /// <summary>
    /// 尝试从路由值创建虚拟路径
    /// </summary>
    /// <param name="requestContext">当前请求上下文</param>
    /// <param name="values">路由值</param>
    /// <remarks>
    /// 简单区域路由表获取路由数据后会自动设置区域所需的 DataTokens
    /// </remarks>
    public override VirtualPathData GetVirtualPath( RequestContext requestContext, RouteValueDictionary values )
    {
      values["area"] = AreaName;

      var data = base.GetVirtualPath( requestContext, values );

      if ( data != null )
      {
        data.DataTokens["area"] = AreaName;
        data.DataTokens["Namespaces"] = Namespaces;
        data.DataTokens["UseNamespaceFallback"] = UseNamespaceFallback;
      }

      return data;
    }
  }
}
