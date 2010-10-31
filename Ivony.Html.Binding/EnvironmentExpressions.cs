using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Configuration;

namespace Ivony.Html.Binding
{

  /// <summary>
  /// 提供环境变量表达式的解析
  /// </summary>
  public static class EnvironmentExpressions
  {

    private static ExpressionProviderCollection _providers;

    internal static object EvaluateExpression( string providerName, string expression )
    {
      lock ( _providers.SyncRoot )
      {

        if ( !_providers.Contains( providerName.ToLowerInvariant() ) )
          throw new FormatException( string.Format( "不存在名为 \"{0}\" 的环境变量提供程序。", providerName ) );

        var provider = _providers[providerName.ToLowerInvariant()];

        return provider.Evaluate( expression );
      }
    }

    public static void RegisterProvider( IEnvironmentVariableProvider provider )
    {
      if ( provider == null )
        throw new ArgumentNullException( "provider" );

      _providers.Add( provider );
    }

    public static void RegisterProvider( string name, Func<string, object> evaluator )
    {

      if ( name == null )
        throw new ArgumentNullException( "name" );

      if ( evaluator == null )
        throw new ArgumentNullException( "evaluator" );



      var provider = new GenericProvider( name, evaluator );
      RegisterProvider( provider );
    }



    private static HttpContext Context
    {
      get { return HttpContext.Current; }
    }

    static EnvironmentExpressions()
    {
      _providers = new ExpressionProviderCollection();

      RegisterProvider( "Application", name => Context.Application[name] );
      RegisterProvider( "Session", name => Context.Session[name] );
      RegisterProvider( "Get", name => Context.Request.QueryString[name] );
      RegisterProvider( "Post", name => Context.Request.Form[name] );
      RegisterProvider( "Server", name => Context.Request.ServerVariables[name] );
      RegisterProvider( "AppSetting", name => ConfigurationManager.AppSettings[name] );
      RegisterProvider( "Context", name => Context.Items[name] );
      RegisterProvider( "ConnectionString", name => ConfigurationManager.ConnectionStrings[name] == null ? null : ConfigurationManager.ConnectionStrings[name].ConnectionString );
      RegisterProvider( new CookiesProvider() );
    }






    private class ExpressionProviderCollection : SynchronizedKeyedCollection<string, IEnvironmentVariableProvider>
    {
      protected override string GetKeyForItem( IEnvironmentVariableProvider item )
      {
        return item.Name.ToLowerInvariant();
      }
    }


    private class GenericProvider : IEnvironmentVariableProvider
    {
      private string _name;
      private Func<string, object> _evaluator;

      public GenericProvider( string name, Func<string, object> evaluator )
      {
        if ( name == null )
          throw new ArgumentNullException( "name" );

        if ( evaluator == null )
          throw new ArgumentNullException( "evaluator" );


        _name = name;
        _evaluator = evaluator;
      }

      #region IEnvironmentExpressionProvider 成员

      public string Name
      {
        get { return _name; }
      }

      public object Evaluate( string expression )
      {
        return _evaluator( expression );
      }

      #endregion
    }



    private class CookiesProvider : IEnvironmentVariableProvider
    {
      public string Name { get { return "Cookies"; } }
      public object Evaluate( string expression )
      {
        var cookie = HttpContext.Current.Request.Cookies[expression];
        if ( cookie != null )
          return cookie.Value;
        else
          return null;
      }
    }

  }

  public interface IEnvironmentVariableProvider
  {

    string Name
    {
      get;
    }

    object Evaluate( string expression );

  }
}
