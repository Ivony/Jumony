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
using System.IO;
using System.Diagnostics;

namespace Ivony.Web
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
    /// 是否打开路由调试模式
    /// </summary>
    public static bool DebugMode
    {
      get;
      set;
    }

    /// <summary>
    /// 获取请求的路由数据
    /// </summary>
    /// <param name="httpContext">HTTP 请求</param>
    /// <returns>路由数据</returns>
    public override RouteData GetRouteData( HttpContextBase httpContext )
    {

      if ( DebugMode )
        Trace( "Begin GetRouteData" );

      var virtualPath = httpContext.Request.GetVirtualPath();


      if ( IsIgnoredPath( virtualPath ) )
        return null;


      var cacheKey = RouteUrlCacheKeyPrefix + httpContext.Request.Url.AbsoluteUri;

      var routeData = httpContext.Cache.Get( cacheKey ) as RouteData;


      if ( routeData != null )
      {

        if ( routeData.Route != this )
          return null;


        if ( DebugMode )
        {
          Trace( "Hit cache" );
          var routeValues = string.Join( ",", routeData.Values.Select( pair => string.Format( "\"{0}\" : \"{1}\"", pair.Key, pair.Value ) ).ToArray() );
          Trace( string.Format( "RouteData: {{{0}}}", routeValues ) );
        }

        return CloneRouteData( routeData );
      }


      var data = _rules
        .OrderBy( r => r.DynamicRouteKeys.Length )
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

      if ( DebugMode )
      {
        var values = string.Join( ",", routeData.Values.Select( pair => string.Format( "\"{0}\" : \"{1}\"", pair.Key, pair.Value ) ).ToArray() );
        Trace( string.Format( "RouteData: {{{0}}}", values ) );
      }

      return CloneRouteData( routeData );

    }


    /// <summary>
    /// 写入追踪消息
    /// </summary>
    /// <param name="message">消息内容</param>
    /// <param name="level">消息级别</param>
    protected void Trace( string message, TraceLevel level = TraceLevel.Info )
    {
      WebServiceLocator.GetTraceService().Trace( level, "Simple Route Table", message );
    }


    /// <summary>
    /// 确定指定的虚拟路径是否应当是被忽略的
    /// </summary>
    /// <param name="virtualPath">要检查的虚拟路径</param>
    /// <returns>该虚拟路径是否应当被忽略</returns>
    protected virtual bool IsIgnoredPath( string virtualPath )
    {
      if ( VirtualPathUtility.GetExtension( virtualPath ).EqualsIgnoreCase( ".axd" ) )
        return true;

      else
        return false;
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


      if ( IsIgnoredPath( virtualPath ) )//如果产生的虚拟路径是被忽略的，则返回 null
      {
        if ( DebugMode )
          Trace( string.Format( "名为 \"{0}\" 路由表的 \"{1}\" 路由规则产生的虚拟路径 {2} 被该路由表忽略", Name, bestRule.Name, virtualPath ), TraceLevel.Warning );

        else
          return null;
      }


      if ( MvcCompatible )
        virtualPath = virtualPath.Substring( 2 );



      cache.Insert( cacheKey, virtualPath, CacheItemPriority.AboveNormal );

      return CreateVirtualPathData( virtualPath, bestRule );
    }

    /// <summary>
    /// 创建 VirtualPathData 对象
    /// </summary>
    /// <param name="virtualPath">虚拟路径</param>
    /// <param name="rule">产生该虚拟路径的路由规则</param>
    /// <returns>VirtualPathData 对象</returns>
    protected VirtualPathData CreateVirtualPathData( string virtualPath, SimpleRouteRule rule )
    {
      var data = new VirtualPathData( this, virtualPath );


      foreach ( var pair in rule.DataTokens )
        data.DataTokens.Add( pair.Key, pair.Value );

      data.DataTokens["RoutingRuleName"] = rule.Name;

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

      SimpleRouteRule conflictRule;
      if ( !AddRuleAndCheckConflict( rule, out conflictRule ) )
        throw new InvalidOperationException( string.Format( "添加规则\"{0}\"失败，路由表 \"{1}\" 中已经存在一条可能冲突的规则：\"{2}\"", rule.Name, conflictRule.SimpleRouteTable.Name, conflictRule.Name ) );

      _rules.Add( rule );

      rule.SimpleRouteTable = this;

      return rule;
    }


    private static ConflictCheckList conflictCheckList = new ConflictCheckList();
    private static object _sync = new object();


    /// <summary>
    /// 在冲突检测表中添加一条规则并检查冲突
    /// </summary>
    /// <param name="rule">要添加的规则</param>
    /// <param name="conflictRule">与之相冲突的规则</param>
    /// <returns>是否添加成功</returns>
    public static bool AddRuleAndCheckConflict( SimpleRouteRule rule, out SimpleRouteRule conflictRule )
    {
      return conflictCheckList.AddRuleAndCheckConflict( rule, out conflictRule );
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
      candidateRules = candidateRules.GroupBy( p => p.DynamicRouteKeys.Length ).OrderBy( group => group.Key ).First().ToArray();

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
}
