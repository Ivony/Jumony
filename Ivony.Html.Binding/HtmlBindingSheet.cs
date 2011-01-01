using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using Ivony.Fluent;
using System.Collections;
using Ivony.Data;

namespace Ivony.Html.Binding
{
  public class HtmlBindingSheet : IHtmlBindingSheet
  {


    private static readonly Regex styleSheetRegex = new Regex( Regulars.styleSheetPattern, RegexOptions.Compiled );



    /// <summary>
    /// 加载样式表
    /// </summary>
    /// <param name="filepath">样式表文件路径</param>
    /// <returns>加载完毕的样式表文档</returns>
    public static HtmlBindingSheet Load( string filepath )
    {
      using ( var stream = File.OpenRead( filepath ) )
      {
        return Load( stream );
      }
    }

    /// <summary>
    /// 加载样式表
    /// </summary>
    /// <param name="stream">要从中加载样式表的流</param>
    /// <returns>加载的样式表</returns>
    public static HtmlBindingSheet Load( Stream stream )
    {
      using ( var reader = new StreamReader( stream ) )
      {
        return Load( reader );
      }
    }

    /// <summary>
    /// 加载样式表
    /// </summary>
    /// <param name="reader">用于读取样式表的 TextReader</param>
    /// <returns></returns>
    public static HtmlBindingSheet Load( TextReader reader )
    {
      string content = reader.ReadToEnd();

      var match = styleSheetRegex.Match( content );

      if ( !match.Success )
        throw new FormatException();

      var rules = match.Groups["styleRule"].Captures.Cast<Capture>().Select( c => new HtmlBindingRule( c.Value ) ).ToArray();

      return new HtmlBindingSheet( rules );

    }

    private HtmlBindingRule[] _rules;

    private HtmlBindingSheet( HtmlBindingRule[] rules )
    {
      _rules = rules;
    }

    public override string ToString()
    {
      return string.Join( "\n", Array.ConvertAll( _rules, r => r.ToString() ) );
    }


    public void Apply( BindingContext context )
    {
      _rules.ForAll( r => r.Apply( context ) );
    }


  }

  public class HtmlBindingRule
  {
    private Dictionary<string, string> settings = new Dictionary<string, string>( StringComparer.InvariantCultureIgnoreCase );


    private static readonly Regex styleRulesRegex = new Regex( @"^\s*" + Regulars.styleRulePattern + @"\s*$", RegexOptions.Compiled );
    private static readonly Regex styleSettingRegex = new Regex( "^" + Regulars.styleSettingPattern + "$", RegexOptions.Compiled );



    public HtmlBindingRule( string rule )
    {
      var ruleMatch = styleRulesRegex.Match( rule );
      if ( !ruleMatch.Success )
        throw new FormatException();

      Selector = CssSelector.Create( ruleMatch.Groups["selector"].Value );

      foreach ( Capture settingCapture in ruleMatch.Groups["styleSetting"].Captures )
      {
        var settingMatch = styleSettingRegex.Match( settingCapture.Value );

        if ( !settingMatch.Success )
          throw new FormatException();

        var name = settingMatch.Groups["name"].Value;
        var value = settingMatch.Groups["value"].Value;

        settings.Add( name, value );
      }


      Analyze();

    }

    protected ICssSelector Selector
    {
      get;
      private set;
    }



    protected void Analyze()
    {

      //binding-source
      string dataSourceExpression = null;
      if ( settings.TryGetValue( "binding-source", out dataSourceExpression ) )
        DataSource = GetDataSource( dataSourceExpression );


      //binding-source-type
      string dataSourceTypeExpression = null;
      if ( settings.TryGetValue( "binding-source-type", out dataSourceTypeExpression ) )
        SourceType = (DataSourceType) Enum.Parse( typeof( DataSourceType ), dataSourceTypeExpression, true );
      else
        SourceType = DataSourceType.Auto;


      //binding-source-default
      string defaultValueExpression;
      if ( settings.TryGetValue( "binding-source-default", out defaultValueExpression ) )
        DataSourceDefault = ExpressionParser.Evaluate( defaultValueExpression );
      else
        DataSourceDefault = null;


      //binding-path
      string path;
      if ( settings.TryGetValue( "binding-path", out path ) )
        TargetPath = path.Trim();
      else
        TargetPath = "@:text";


      //binding-format
      string formatExpression;
      if ( settings.TryGetValue( "binding-format", out formatExpression ) )
      {

        var format = ExpressionParser.Evaluate( formatExpression );
        if ( !(format is string) )
          throw new FormatException();

        FormatString = (string) format;
      }
      else
        FormatString = "{0}";


      //binding-null-behavior
      string nullBehaviorExpression = null;
      if ( settings.TryGetValue( "binding-null-behavior", out nullBehaviorExpression ) )
        NullBehavior = Enum.Parse( typeof( BindingNullBehavior ), nullBehaviorExpression, true ).CastTo<BindingNullBehavior>();
      else
        NullBehavior = BindingNullBehavior.Ignore;

    }


    protected object DataSource
    {
      get;
      private set;
    }

    protected string TargetPath
    {
      get;
      private set;
    }

    protected DataSourceType SourceType
    {
      get;
      private set;
    }

    protected object DataSourceDefault
    {
      get;
      private set;
    }

    protected string FormatString
    {
      get;
      private set;
    }

    public BindingNullBehavior NullBehavior
    {
      get;
      private set;
    }





    private class ValueNotSet
    {
      public static readonly ValueNotSet Instance = new ValueNotSet();
    }



    private static readonly string dataSourceListPattern = string.Format( @"\[((?<item>{0}|[^\\'"",]*)(,(?<item>{0}|[^\\'"",]*))*)?\]", Regulars.quoteTextPattern );
    private static readonly Regex dataSourceListRegex = new Regex( dataSourceListPattern, RegexOptions.Compiled );


    private object GetDataSource( string expression )
    {
      var dataSource = ExpressionParser.Evaluate( expression );
      IDataSource dataSourceObject = dataSource as IDataSource;


      if ( dataSourceObject != null )
        return dataSourceObject.GetDataSource();

      return dataSource;
    }



    public void Apply( BindingContext context )
    {

      var elements = Selector.Filter( context.Scope.Descendants() );

      switch ( SourceType )
      {
        case DataSourceType.Auto:
          if ( DataSource is IEnumerable )
            BindAsEnumerable( elements );
          else
            BindAsObject( elements );
          break;

        case DataSourceType.AsObject:
          BindAsObject( elements );
          break;
        default:
          break;
      }
    }


    /// <summary>
    /// 将数据源当作对象来绑定
    /// </summary>
    /// <param name="elements"></param>
    private void BindAsObject( IEnumerable<IHtmlElement> elements )
    {
      elements.Bind( TargetPath, DataSource, FormatString, NullBehavior );
    }

    /// <summary>
    /// 将数据源当作列表来绑定
    /// </summary>
    /// <param name="elements"></param>
    private void BindAsEnumerable( IEnumerable<IHtmlElement> elements )
    {
      var list = DataSource as IEnumerable;

      Action<object, IHtmlElement> binder = ( item, e ) =>
      {
        e.Bind( TargetPath, item, FormatString, NullBehavior );
      };

      if ( DataSourceDefault == ValueNotSet.Instance )
        list.Cast<object>().BindTo( elements, binder );
      else
        list.Cast<object>().BindTo( elements, DataSourceDefault, binder );
    }


    public override string ToString()
    {
      using ( StringWriter writer = new StringWriter() )
      {
        writer.WriteLine( Selector );
        writer.WriteLine( "{" );

        foreach ( var setting in settings )
          writer.WriteLine( "  {0}: {1};", setting.Key, setting.Value.Trim() );

        writer.WriteLine( "}" );

        return writer.ToString();
      }
    }

  }
}
