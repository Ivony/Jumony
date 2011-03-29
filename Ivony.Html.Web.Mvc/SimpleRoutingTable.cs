using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Routing;
using System.Text.RegularExpressions;
using System.Web;
using Ivony.Fluent;
using System.Collections.Specialized;

namespace Ivony.Html.Web.Mvc
{
  public class SimpleRoutingTable : RouteBase
  {
    public override RouteData GetRouteData( HttpContextBase httpContext )
    {
      var virtualPath = httpContext.Request.AppRelativeCurrentExecutionFilePath;

      var data = _rules
        .Select( r => new
          {
            Rule = r,
            Values = r.GetRouteValues( virtualPath, httpContext.Request.QueryString ),
          } )
        .Where( i => i.Values != null )
        .OrderBy( i => i.Rule.DynamicRouteKyes.Length )
        .FirstOrDefault();

      if ( data == null )
        return null;


      var routeData = new RouteData( this, Handler );

      foreach ( var pair in data.Values )
        routeData.Values.Add( pair.Key, pair.Value );


      routeData.DataTokens["RoutingRuleName"] = data.Rule.Name;

      return routeData;

    }

    public override VirtualPathData GetVirtualPath( RequestContext requestContext, RouteValueDictionary values )
    {


      var _values = values.ToDictionary( pair => pair.Key, pair => pair.Value == null ? null : pair.Value.ToString() );
      var keySet = new HashSet<string>( _values.Keys );


      var candidateRules = _rules
        .Where( r => keySet.IsSupersetOf( r.RouteKeys ) )  //所有路由键都必须匹配
        .Where( r => keySet.IsSubsetOf( r.AllKeys ) )      //所有路由键和查询字符串键必须能涵盖要设置的键。
        .Where( r => r.IsMatch( _values ) );               //必须满足路由规则所定义的路由数据。

      if ( !candidateRules.Any() )
        return null;


      var bestRule = BestRule( candidateRules );

      var virtualPath = bestRule.CreateVirtualPath( _values );

      var data = new VirtualPathData( this, virtualPath );

      data.DataTokens["RoutingRuleName"] = bestRule.Name;

      return data;
    }


    public void AddRule( string name, string urlPattern, IDictionary<string, string> routeValues, string[] queryKeys )
    {
      var rule = new SimpleRoutingRule( name, urlPattern, routeValues, queryKeys );

      AddRule( rule );
    }

    protected void AddRule( SimpleRoutingRule rule )
    {

      {
        var conflictRule = _rules
          .Where( r => r.AllKeys.Length == rule.AllKeys.Length )                        //若通过所有键多寡无法区分
          .Where( r => r.RouteKeys.Length == rule.RouteKeys.Length )                    //若通过RouteKey多寡无法区分
          .Where( r => r.DynamicRouteKyes.Length == rule.DynamicRouteKyes.Length )      //若通过动态路径段多寡也无法区分
          .Where( r => r.EqualsConstraints( rule ) )                                    //若约束集也一致
          .FirstOrDefault();

        if ( conflictRule != null )
          throw new InvalidOperationException( string.Format( "添加规则失败，路由表中已经存在一条可能冲突的规则：{0}", conflictRule.Name ) );
      }


      {
        var conflictRule = _rules
          .Where( r => r.Paragraphes.Length == rule.Paragraphes.Length )                //若路径段长度一致
          .Where( r => r.StaticPrefix.EqualsIgnoreCase( rule.StaticPrefix ) )           //若静态段也一致
          .FirstOrDefault();


        if ( conflictRule != null )
          throw new InvalidOperationException( string.Format( "添加规则失败，路由表中已经存在一条可能冲突的规则：{0}", conflictRule.Name ) );
      }

      _rules.Add( rule );
    }


    private SimpleRoutingRule BestRule( IEnumerable<SimpleRoutingRule> candidateRules )
    {

      //满足最多约束的被优先考虑
      var maxConstraints = candidateRules.Max( r => r.RouteValueConstraints.Count );
      candidateRules = candidateRules.Where( r => r.RouteValueConstraints.Count == maxConstraints );

      if ( candidateRules.IsSingle() )
        return candidateRules.Single();


      //拥有最多路由键的被优先考虑
      var maxRouteKeys = candidateRules.Max( r => r.RouteKeys.Length );
      candidateRules = candidateRules.Where( r => r.RouteKeys.Length == maxRouteKeys );

      if ( candidateRules.IsSingle() )
        return candidateRules.Single();


      //拥有最少参数的被优先考虑。
      var minKeys = candidateRules.Min( r => r.AllKeys );
      candidateRules = candidateRules.Where( r => r.AllKeys == minKeys );

      if ( candidateRules.IsSingle() )
        return candidateRules.Single();


      //拥有最少动态参数的被优先考虑
      var minDynamics = candidateRules.Min( p => p.DynamicRouteKyes.Length );
      candidateRules = candidateRules.Where( r => r.DynamicRouteKyes.Length == minDynamics );

      if ( candidateRules.IsSingle() )
        return candidateRules.Single();

      else
        return null;

    }




    public SimpleRoutingTable( IRouteHandler handler )
    {
      Handler = handler;
      UrlEncoding = Encoding.UTF8;


    }



    private ICollection<SimpleRoutingRule> _rules = new List<SimpleRoutingRule>();

    public SimpleRoutingRule[] Rules
    {
      get
      {
        return _rules.ToArray();
      }
    }



    public IRouteHandler Handler { get; private set; }

    public Encoding UrlEncoding { get; private set; }
  }



  public class SimpleRoutingRule
  {

    public const string staticParagraphPattern = @"(?<paragraph>[\p{Lu}\p{Ll}\p{Nd}]+)";
    public const string dynamicParagraphPattern = @"(?<paragraph>\{[\p{Lu}\p{Ll}\p{Nd}]+\})";
    public static readonly string urlPattern = @"(^~/$)|(^~(/{static}(/{static})*(/{dynamic})*)|((/{dynamic})+)$)".Replace( "{static}", staticParagraphPattern ).Replace( "{dynamic}", dynamicParagraphPattern );

    private static readonly Regex urlPatternRegex = new Regex( urlPattern, RegexOptions.Compiled );

    public SimpleRoutingRule( string name, string urlPattern, IDictionary<string, string> routeValues, string[] queryKeys )
    {

      Name = name;

      var match = urlPatternRegex.Match( urlPattern );

      if ( !match.Success )
        throw new FormatException( "URL模式格式不正确" );

      _paragraphes = match.Groups["paragraph"].Captures.Cast<Capture>().Select( c => c.Value ).ToArray();

      _urlPattern = urlPattern;
      _routeValuesConstraints = routeValues.ToDictionary( pair => pair.Key, pair => pair.Value );

      var keys = new HashSet<string>( _routeValuesConstraints.Keys );

      _dynamics = _paragraphes.Where( p => p.StartsWith( "{" ) && p.EndsWith( "}" ) ).ToArray();


      foreach ( var p in _dynamics )
      {
        var key = p.Substring( 1, p.Length - 2 );

        if ( keys.Contains( key ) )
          throw new FormatException( "URL模式格式不正确，包含重复的动态参数名或动态参数名与预设路由键重复" );


        keys.Add( key );
      }

      _routeKeys = keys.ToArray();

      if ( _routeKeys.Intersect( queryKeys ).Any() )
        throw new FormatException( "URL模式格式不正确，动态参数或预设路由键与可选查询字符串名重复" );

      _queryKeys = queryKeys;

    }


    public string Name
    {
      get;
      private set;
    }


    private string[] _paragraphes;

    public string[] Paragraphes
    {
      get { return _paragraphes; }
    }


    private string[] _routeKeys;

    public string[] RouteKeys
    {
      get { return _routeKeys; }
    }



    private string[] _queryKeys;

    public string[] QueryKeys
    {
      get { return _queryKeys; }
    }

    private string[] _allKeys;

    public string[] AllKeys
    {
      get
      {
        if ( _allKeys == null )
          _allKeys = _routeKeys.Union( _queryKeys ).ToArray();


        return _allKeys;
      }
    }


    private string[] _dynamics;

    public string[] DynamicRouteKyes
    {
      get { return _dynamics; }
    }


    private string _prefix;

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

    public string UrlPattern
    {
      get { return _urlPattern; }
    }


    private IDictionary<string, string> _routeValuesConstraints;

    public IDictionary<string, string> RouteValueConstraints
    {
      get { return new Dictionary<string, string>( _routeValuesConstraints ); }
    }



    public string CreateVirtualPath( IDictionary<string, string> routeValues )
    {

      var builder = new StringBuilder( StaticPrefix );

      foreach ( var key in DynamicRouteKyes )
      {
        var value = routeValues[key];

        if ( string.IsNullOrEmpty( value ) )
          throw new ArgumentException( "作为动态路径的路由值不能包含空引用或空字符串", "routeValues" );


        if ( value.Contains( '/' ) )
          throw new ArgumentException( "作为动态路径的路由值不能包含路径分隔符 '/'", "routeValues" );

        value = HttpUtility.UrlEncode( value, RoutingTable.UrlEncoding );

        builder.Append( "/" + value );

      }


      bool isAppendQueryStartSymbol = false;

      foreach ( var key in QueryKeys )
      {
        var value = routeValues[key];

        if ( !isAppendQueryStartSymbol )
          builder.Append( '?' );
        else
          builder.Append( '&' );

        builder.Append( HttpUtility.UrlEncode( key ) );
        builder.Append( '=' );
        builder.Append( HttpUtility.UrlEncode( routeValues[key] ) );

      }


      return builder.ToString();
    }


    public SimpleRoutingTable RoutingTable
    {
      get;
      private set;
    }



    public bool IsMatch( IDictionary<string, string> values )
    {

      if ( values == null )
        throw new ArgumentNullException( "values" );


      foreach ( var key in _routeValuesConstraints.Keys )
      {
        string value;

        if ( values.TryGetValue( key, out value ) )
          return false;

        if ( _routeValuesConstraints[key] != value )
          return false;
      }

      return true;
    }

    public bool EqualsConstraints( SimpleRoutingRule rule )
    {

      if ( rule == null )
        throw new ArgumentNullException( "rule" );


      if ( rule.RouteValueConstraints.Count != RouteValueConstraints.Count )
        return false;

      return IsMatch( rule.RouteValueConstraints );

    }

    public virtual IDictionary<string, string> GetRouteValues( string virtualPath, NameValueCollection queryString )
    {

      if ( virtualPath == null )
        throw new ArgumentNullException( "virtualPath" );

      if ( !VirtualPathUtility.IsAppRelative( virtualPath ) )
        throw new ArgumentException( "必须是相对于应用程序根的路径" );

      var queryKeySet = new HashSet<string>( _queryKeys );

      if ( !queryKeySet.IsSupersetOf( queryString.AllKeys ) )
        return null;


      var pathParagraphs = virtualPath.Substring( 2 ).Split( '/' );

      if ( pathParagraphs.Length != Paragraphes.Length )
        return null;



      var values = new Dictionary<string, string>();



      foreach ( var pair in _routeValuesConstraints )
        values.Add( pair.Key, pair.Value );

      for ( int i = 0; i < pathParagraphs.Length; i++ )
      {

        var paragraph = Paragraphes[i];

        if ( !paragraph.StartsWith( "{" ) )
        {
          if ( !pathParagraphs[i].EqualsIgnoreCase( paragraph ) )
            return null;
        }
        else
        {
          var name = paragraph.Substring( 1, paragraph.Length - 2 );
          values.Add( name, pathParagraphs[i] );
        }
      }


      foreach ( var key in queryString.AllKeys )
        values.Add( key, queryString[key] );


      return values;
    }
  }
}
