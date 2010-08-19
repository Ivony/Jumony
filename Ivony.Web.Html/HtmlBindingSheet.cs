using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using Ivony.Fluent;
using System.Collections;

namespace Ivony.Web.Html
{
  public class HtmlBindingSheet
  {


    public static readonly string styleSheetPattern = string.Format( @"^((\s*(?<styleRule>{0})\s*)+|\s*)$", HtmlBindingRule.styleRulePattern );
    private static readonly Regex styleSheetRegex = new Regex( styleSheetPattern, RegexOptions.Compiled );



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
    /// <param name="reader">要从中加载样式表的文本读取器</param>
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


    public void Apply()
    {
      _rules.ForAll( r => r.Apply( HtmlBindingContext.Current ) );
    }


  }

  public class HtmlBindingRule
  {
    private Dictionary<string, string> settings = new Dictionary<string, string>( StringComparer.InvariantCultureIgnoreCase );

    public static readonly string styleSettingPattern = string.Format( @"\s*(?<name>[\w-]+)\s*:(?<value>({0}|[^'"";])+);\s*", Regulars.quoteTextPattern );
    public static readonly string styleRulePattern = string.Format( @"(?<selector>{0})\s*{{(?<styleSetting>{1})*}}", Regulars.cssSelectorPatternNoGroup, styleSettingPattern );

    private Regex styleRulesRegex = new Regex( styleRulePattern, RegexOptions.Compiled );
    private Regex styleSettingRegex = new Regex( styleSettingPattern, RegexOptions.Compiled );


    private static readonly Regex quoteTextRegex = new Regex( Regulars.quoteTextPattern, RegexOptions.Compiled );


    public HtmlBindingRule( string rule )
    {
      var ruleMatch = styleRulesRegex.Match( rule );
      if ( !ruleMatch.Success )
        throw new FormatException();

      Selector = new HtmlCssSelector( ruleMatch.Groups["selector"].Value );

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

    protected HtmlCssSelector Selector
    {
      get;
      private set;
    }



    protected void Analyze()
    {

      //binding-source
      string dataSourceExpression = null;
      if ( settings.TryGetValue( "binding-source", out dataSourceExpression ) )
        DataSource = ParseDataSource( dataSourceExpression );


      //binding-source-type
      if ( DataSource is IEnumerable )
        SourceType = DataSourceType.Enumerable;
      else
        SourceType = DataSourceType.Object;

      //binding-source-default
      string defaultValueExpression;
      if ( settings.TryGetValue( "binding-source-default", out defaultValueExpression ) )
      {

      }


      string path = null;
      if ( settings.TryGetValue( "binding-path", out path ) )
        TargetPath = path.Trim();


      string format = null;
      if ( settings.TryGetValue( "binding-format", out format ) )
      {
        format = format.Trim();
        var quoteMatch = quoteTextRegex.Match( format );

        if ( quoteMatch.Success )
          format = quoteMatch.Groups["quoteText"].Value;

        FormatString = format;
      }

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


    protected enum DataSourceType
    {
      Object,
      Enumerable
    }

    private static readonly string dataSourceListPattern = string.Format( @"\[((?<item>{0}|[^\\'"",]*)(,(?<item>{0}|[^\\'"",]*))*)?\]", Regulars.quoteTextPattern );
    private static readonly Regex dataSourceListRegex = new Regex( dataSourceListPattern, RegexOptions.Compiled );

    private object ParseDataSource( string dataSourceExpression )
    {

      var listMatch = dataSourceListRegex.Match( dataSourceExpression );
      if ( listMatch.Success )
        return listMatch.Groups["item"].Captures.Cast<Capture>().Select( c => c.ToString() );

      throw new NotSupportedException();
    }



    public void Apply( HtmlBindingContext context )
    {

      var elements = Selector.Search( context.Scope, true );

      switch ( SourceType )
      {
        case DataSourceType.Object:
          BindAsObject( elements );
          break;
        case DataSourceType.Enumerable:
          BindAsEnumerable( elements );
          break;
        default:
          break;
      }
    }

    private void BindAsObject( IEnumerable<IHtmlElement> elements )
    {
      throw new NotImplementedException();
    }

    private void BindAsEnumerable( IEnumerable<IHtmlElement> elements )
    {
      var list = DataSource as IEnumerable;

      list.OfType<object>().BindTo( elements, ( item, e ) =>
      {
        e.Bind( TargetPath, item, FormatString, BindingNullBehavior.Ignore );
      } );
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
