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
  public class CssSelector
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
    internal static ElementSelector CreateElementSelector( string expression )
    {
      return new ElementSelector( expression );
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
    private PartSelector CreateSelector( string expression )
    {

      var match = cssSelectorRegex.Match( expression );
      if ( !match.Success )
        throw new FormatException();


      var selector = new PartSelector( new ElementSelector( match.Groups["elementSelector"].Value ) );

      foreach ( var extraExpression in match.Groups["extra"].Captures.Cast<Capture>().Select( c => c.Value ) )
      {
        var extraMatch = extraRegex.Match( extraExpression );

        if ( !extraMatch.Success )
          throw new FormatException();

        var relative = extraMatch.Groups["relative"].Value.Trim();
        var elementSelector = extraMatch.Groups["elementSelector"].Value.Trim();

        var newPartSelector = new PartSelector( new ElementSelector( elementSelector ), relative, selector );
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


    private readonly PartSelector[] _selectors;




    /// <summary>
    /// 检查元素是否符合选择器要求
    /// </summary>
    /// <param name="element">元素</param>
    /// <param name="scope">上溯范畴</param>
    /// <returns>是否符合选择器要求</returns>
    private bool Allows( IHtmlElement element, IHtmlContainer scope )
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

      elements = elements.Where( element => Allows( element, asScope ? container : null ) );


      elements = new TraceEnumerable<IHtmlElement>( this, elements );

      return elements;
    }


    private class TraceEnumerable<T> : IEnumerable<T>
    {

      private CssSelector _selector;
      private IEnumerable<T> _enumerable;

      public TraceEnumerable( CssSelector selector, IEnumerable<T> enumerable )
      {
        _selector = selector;
        _enumerable = enumerable;
      }

      private class Enumerator : IEnumerator<T>
      {

        private CssSelector coreSelector;
        private IEnumerator<T> coreEnumerator;

        public Enumerator( CssSelector selector, IEnumerator<T> enumerator )
        {
          coreSelector = selector;
          coreEnumerator = enumerator;

          Trace.Write( "Selector", string.Format( CultureInfo.InvariantCulture, "Begin Enumerate Search \"{0}\"", coreSelector.ExpressionString ) );
        }


        #region IEnumerator<T> 成员

        T IEnumerator<T>.Current
        {
          get { return coreEnumerator.Current; }
        }

        #endregion

        #region IDisposable 成员

        void IDisposable.Dispose()
        {
          coreEnumerator.Dispose();
          Trace.Write( "Selector", string.Format( CultureInfo.InvariantCulture, "End Enumerate Search \"{0}\"", coreSelector.ExpressionString ) );
        }

        #endregion

        #region IEnumerator 成员

        object System.Collections.IEnumerator.Current
        {
          get { return coreEnumerator.Current; }
        }

        bool System.Collections.IEnumerator.MoveNext()
        {
          return coreEnumerator.MoveNext();
        }

        void System.Collections.IEnumerator.Reset()
        {
          Trace.Write( "Selector", string.Format( CultureInfo.InvariantCulture, "Begin Enumerate Search \"{0}\"", coreSelector.ExpressionString ) );
          coreEnumerator.Reset();
        }

        #endregion
      }



      #region IEnumerable<T> 成员

      IEnumerator<T> IEnumerable<T>.GetEnumerator()
      {
        return new Enumerator( _selector, _enumerable.GetEnumerator() );
      }

      #endregion

      #region IEnumerable 成员

      System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
      {
        throw new NotImplementedException();
      }

      #endregion
    }









    private class PartSelector
    {

      private readonly string _relative;
      public string Relative
      {
        get { return _relative; }
      }

      private readonly ElementSelector _selector;
      public ElementSelector ElementSelector
      {
        get { return _selector; }
      }

      private readonly PartSelector _parent;
      public PartSelector ParentSelector
      {
        get { return _parent; }
      }


      public PartSelector( ElementSelector selector )
        : this( selector, null, null )
      {

      }

      public PartSelector( ElementSelector selector, string relative, PartSelector parent )
      {
        _selector = selector;
        _relative = relative;
        _parent = parent;
      }


      public bool Allows( IHtmlElement element, IHtmlContainer scope )
      {

        if ( !ElementSelector.Allows( element ) )
          return false;

        if ( Relative == null )
          return true;

        else if ( Relative == ">" )
          return element.Parent().Equals( scope ) ? false : ParentSelector.Allows( element.Parent(), scope );

        else if ( Relative == "" )
          return element.Ancestors().TakeWhile( e => !e.Equals( scope ) ).Any( e => ParentSelector.Allows( e, scope ) );

        else if ( Relative == "+" )
          return ParentSelector.Allows( element.PreviousElement(), scope );

        else if ( Relative == "~" )
          return element.SiblingsBeforeSelf().Any( e => ParentSelector.Allows( e, scope ) );

        else
          throw new FormatException();
      }

      public override string ToString()
      {
        if ( Relative == null )
          return ElementSelector.ToString();

        else if ( Relative == "" )
          return string.Format( "{0} {1}", ParentSelector, ElementSelector );

        else
          return string.Format( "{0} {1} {2}", ParentSelector, Relative, ElementSelector );
      }

    }

  }
}
