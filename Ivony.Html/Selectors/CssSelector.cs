using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Collections;
using Ivony.Fluent;
using System.Diagnostics;
using System.Globalization;

namespace Ivony.Html
{

  /// <summary>
  /// 代表一个CSS选择器
  /// </summary>
  public sealed class CssSelector
  {


    public static readonly Regex cssSelectorRegex = new Regex( "^" + Regulars.cssSelectorPattern + "$", RegexOptions.Compiled | RegexOptions.CultureInvariant );

    public static readonly Regex extraRegex = new Regex( "^" + Regulars.extraExpressionPattern + "$", RegexOptions.Compiled | RegexOptions.CultureInvariant );

    private readonly string[] _expressions;





    /// <summary>
    /// 创建CSS选择器实例
    /// </summary>
    /// <param name="expression">选择器表达式</param>
    /// <returns>CSS选择器</returns>
    public static CssSelector Create( string expression )
    {
      return Create( new[] { expression } );
    }

    /// <summary>
    /// 创建CSS选择器实例
    /// </summary>
    /// <param name="expressions">多个选择器表达式，结果会自动合并</param>
    /// <returns>CSS选择器</returns>
    public static CssSelector Create( params string[] expressions )
    {
      var selector = new CssSelector( expressions );

      return selector;
    }

    /// <summary>
    /// 创建一个元素选择器实例
    /// </summary>
    /// <param name="expression">元素选择器表达式</param>
    /// <returns>CSS元素选择器</returns>
    internal static CssElementSelector CreateElementSelector( string expression )
    {
      return new CssElementSelector( expression );
    }


    /// <summary>
    /// 创建一个CSS选择器实例
    /// </summary>
    /// <param name="expressions"></param>
    private CssSelector( string[] expressions )
    {
      _expressions = expressions;


      _selectors = expressions.Select( e => CreateSelector( e ) ).ToArray();

    }


    /// <summary>
    /// 创建选择器，这是核心函数
    /// </summary>
    /// <param name="expression">选择器表达式</param>
    /// <returns></returns>
    private CssCasecadingSelector CreateSelector( string expression )
    {

      var match = cssSelectorRegex.Match( expression );
      if ( !match.Success )
        throw new FormatException();


      var selector = new CssCasecadingSelector( new CssElementSelector( match.Groups["elementSelector"].Value ) );

      foreach ( var extraExpression in match.Groups["extra"].Captures.Cast<Capture>().Select( c => c.Value ) )
      {
        var extraMatch = extraRegex.Match( extraExpression );

        if ( !extraMatch.Success )
          throw new FormatException();

        var relative = extraMatch.Groups["relative"].Value.Trim();
        var elementSelector = extraMatch.Groups["elementSelector"].Value.Trim();

        var newPartSelector = new CssCasecadingSelector( new CssElementSelector( elementSelector ), relative, selector );
        selector = newPartSelector;

      }


      return selector;
    }


    internal string ExpressionString
    {
      get { return string.Join( " , ", _expressions ); }
    }



    /// <summary>
    /// 获取选择器的表达式
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
      return string.Join( " , ", _selectors.Select( s => s.ToString() ).ToArray() );
    }


    private readonly CssCasecadingSelector[] _selectors;




    /// <summary>
    /// 检查元素是否符合选择器要求
    /// </summary>
    /// <param name="element">元素</param>
    /// <param name="scope">上溯范畴</param>
    /// <returns>是否符合选择器要求</returns>
    private bool IsEligible( IHtmlElement element, IHtmlContainer scope )
    {
      return _selectors.Any( s => s.Allows( element, scope ) );
    }



    /// <summary>
    /// 在指定容器子代元素和指定范畴下搜索满足选择器的所有元素
    /// </summary>
    /// <param name="container">容器，其所有子代元素被列入搜索范围</param>
    /// <param name="asScope">指定选择器在计算父元素时，是否不超出指定容器的范畴</param>
    /// <remarks>
    /// 选择器的工作原理是从最里层的元素选择器开始搜索，逐步验证其父元素是否满足父选择器的规则。如果asScope参数为true，则选择器在上溯验证父元素时，将不超出container的范畴，换言之只有container的子代才会被考虑。考虑下面的文档结构：
    /// <![CDATA[
    /// <html>
    ///   <body>
    ///     <ul id="outer">
    ///       <li id="item">
    ///         <ul "inner">
    ///           <li>123</li>
    ///           <li>456</li>
    ///         </ul>
    ///         <ol>
    ///           <li>abc</li>
    ///         </ol>
    ///       </li>
    ///     </ul>
    ///   </body>
    /// </html>
    /// ]]>
    /// 当使用选择器"#item ul li"来选择元素时，我们将得到正确的结果，即123和456两个节点。
    /// 但如果我们将#item元素当作上下文且asScope参数为false来选择"ul li"元素时，可能会不能得到预期的结果，会发现abc元素也被选择了。这是因为选择器在查找父级元素限定时，会查找到id为outter的ul元素。为了解决此问题，请将asScope参数设置为true。
    /// </remarks>
    /// <returns>搜索到的所有元素</returns>
    public IEnumerable<IHtmlElement> Search( IHtmlContainer container, bool asScope )
    {

      var elements = container.Descendants();

      elements = elements.Where( element => IsEligible( element, asScope ? container : null ) );


      elements = new TraceEnumerable<IHtmlElement>( this, elements );

      return elements;
    }


  }
}
