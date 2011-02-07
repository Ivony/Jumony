using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace Ivony.Html.Binding
{
  public static partial class HtmlBinder
  {

    /// <summary>
    /// 格式化值
    /// </summary>
    /// <param name="format">格式化表达式</param>
    /// <param name="value">要被格式化的值</param>
    /// <returns>值格式化后的形式</returns>
    public static string FormatValue( object value, string format )
    {

      if ( value == null )
        return null;

      if ( format == null )
        return string.Format( format, value );
      else
        return value.ToString();
    }






    private static ExpressionProviderCollection _expressionProviders;

    /// <summary>
    /// 计算环境表达式值
    /// </summary>
    /// <param name="providerName">表达式提供程序名</param>
    /// <param name="expression">表达式</param>
    /// <returns>表达式值</returns>
    public static object EvaluateExpression( string providerName, string expression )
    {
      lock ( _expressionProviders.SyncRoot )
      {

        if ( !_expressionProviders.Contains( providerName.ToLowerInvariant() ) )
          throw new FormatException( string.Format( "不存在名为 \"{0}\" 的表达式提供程序。", providerName ) );

        var provider = _expressionProviders[providerName.ToLowerInvariant()];

        return provider.EvaluateExpression( expression );
      }
    }


    /// <summary>
    /// 注册一个表达式提供程序
    /// </summary>
    /// <param name="provider">表达式提供程序</param>
    public static void RegisterExpressionProvider( IExpressionProvider provider )
    {
      if ( provider == null )
        throw new ArgumentNullException( "provider" );

      _expressionProviders.Add( provider );
    }

    /// <summary>
    /// 注册一个表达式提供程序
    /// </summary>
    /// <param name="name">名称</param>
    /// <param name="evaluator">用于计算表达式的函数</param>
    public static void RegisterExpressionProvider( string name, Func<string, object> evaluator )
    {

      if ( name == null )
        throw new ArgumentNullException( "name" );

      if ( evaluator == null )
        throw new ArgumentNullException( "evaluator" );



      var provider = new GenericProvider( name, evaluator );
      RegisterExpressionProvider( provider );
    }



    static HtmlBinder()
    {
      _expressionProviders = new ExpressionProviderCollection();

      RegisterExpressionProvider( "AppSetting", name => ConfigurationManager.AppSettings[name] );
      RegisterExpressionProvider( "ConnectionString", name => ConfigurationManager.ConnectionStrings[name] == null ? null : ConfigurationManager.ConnectionStrings[name].ConnectionString );
    }






    private class ExpressionProviderCollection : SynchronizedKeyedCollection<string, IExpressionProvider>
    {
      protected override string GetKeyForItem( IExpressionProvider item )
      {
        return item.Name.ToLowerInvariant();
      }
    }


    private class GenericProvider : IExpressionProvider
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


      /// <summary>
      /// 表达式提供程序名称
      /// </summary>
      public string Name
      {
        get { return _name; }
      }


      /// <summary>
      /// 计算表达式的值
      /// </summary>
      /// <param name="expression">表达式</param>
      /// <returns>值</returns>
      public object EvaluateExpression( string expression )
      {
        return _evaluator( expression );
      }

      #endregion
    }
  }



  /// <summary>
  /// 
  /// </summary>
  public interface IExpressionProvider
  {

    string Name
    {
      get;
    }

    object EvaluateExpression( string expression );

  }

}
