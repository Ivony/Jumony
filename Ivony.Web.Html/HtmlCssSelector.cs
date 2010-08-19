using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Collections;

namespace Ivony.Web.Html
{
  public class HtmlCssSelector
  {


    public static readonly Regex cssSelectorRegex = new Regex( "^" + Regulars.cssSelectorPattern + "$", RegexOptions.Compiled );

    public static readonly Regex extraRegex = new Regex( "^" + Regulars.extraExpressionPattern + "$", RegexOptions.Compiled );



    private readonly string _expression;



    private static readonly Hashtable selectorCache = Hashtable.Synchronized( new Hashtable() );


    public static HtmlCssSelector Create( string expression )
    {
      var selector = (HtmlCssSelector) selectorCache[expression];
      if ( selector != null )
        return selector;

      selector = new HtmlCssSelector( expression );

      selectorCache[expression] = selector;

      return selector;
    }


    /// <summary>
    /// 创建一个CSS选择器实例
    /// </summary>
    /// <param name="expression"></param>
    private HtmlCssSelector( string expression )
    {
      _expression = expression;

      if ( HttpContext.Current.Trace.IsEnabled )
        HttpContext.Current.Trace.Write( "Selector", string.Format( "Begin Analyze Search \"{0}\"", expression ) );

      var match = cssSelectorRegex.Match( expression );
      if ( !match.Success )
        throw new FormatException();


      _selector = new PartSelector( new ElementSelector( match.Groups["elementSelector"].Value ) );

      foreach ( var extraExpression in match.Groups["extra"].Captures.Cast<Capture>().Select( c => c.Value ) )
      {
        var extraMatch = extraRegex.Match( extraExpression );

        if ( !extraMatch.Success )
          throw new FormatException();

        var relative = extraMatch.Groups["relative"].Value.Trim();
        var elementSelector = extraMatch.Groups["elementSelector"].Value.Trim();

        var newPartSelector = new PartSelector( new ElementSelector( elementSelector ), relative, _selector );
        _selector = newPartSelector;

      }


      if ( HttpContext.Current.Trace.IsEnabled )
        HttpContext.Current.Trace.Write( "Selector", string.Format( "End Analyze Search \"{0}\"", expression ) );

    }

    public override string ToString()
    {
      return _selector.ToString();
    }


    private readonly PartSelector _selector;


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
          return element.Parent.Equals( scope )? false : ParentSelector.Allows( element.Parent as IHtmlElement, scope );

        else if ( Relative == "" )
          return element.Ancestors().TakeWhile( e => !e.Equals( scope ) ).Any( e => ParentSelector.Allows( e, scope ) );

        else if ( Relative == "+" )
          return ParentSelector.Allows( element.PreviousElement(), scope );

        else if ( Relative == "~" )
          return element.ElementsBeforeSelf().Any( e => ParentSelector.Allows( e, scope ) );

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


    /// <summary>
    /// 在指定容器子代元素和指定范畴下搜索满足选择器的所有元素
    /// </summary>
    /// <param name="container">容器，其所有子代元素被列入搜索范围</param>
    /// <param name="asScope">指定选择器在计算父元素时，是否不超出指定容器的范畴</param>
    /// <remarks>
    /// 选择器的工作原理是从最里层的元素选择器开始搜索，逐步验证其父元素是否满足父选择器的规则。如果asScope参数为true，则选择器在验证父元素时，将不超出container的。考虑下面的文档结构：
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

      var elements = container.Descendant();

      elements =  elements.Where( element => _selector.Allows( element, asScope ? container : null ) );
      if ( HttpContext.Current.Trace.IsEnabled )
        elements = new TraceEnumerable<IHtmlElement>( this, elements );

      return elements;
    }


    private class TraceEnumerable<T> : IEnumerable<T>
    {

      private HtmlCssSelector _selector;
      private IEnumerable<T> _enumerable;

      public TraceEnumerable( HtmlCssSelector selector, IEnumerable<T> enumerable )
      {
        _selector = selector;
        _enumerable = enumerable;
      }

      private class Enumerator : IEnumerator<T>
      {

        private HtmlCssSelector coreSelector;
        private IEnumerator<T> coreEnumerator;

        public Enumerator( HtmlCssSelector selector, IEnumerator<T> enumerator )
        {
          coreSelector = selector;
          coreEnumerator = enumerator;

          HttpContext.Current.Trace.Write( "Selector", string.Format( "Begin Enumerate Search \"{0}\"", coreSelector._expression ) );
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
          HttpContext.Current.Trace.Write( "Selector", string.Format( "End Enumerate Search \"{0}\"", coreSelector._expression ) );
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
          HttpContext.Current.Trace.Write( "Selector", string.Format( "Begin Enumerate Search \"{0}\"", coreSelector._expression ) );
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




    public class ElementSelector
    {

      private static readonly Regex regex = new Regex( Regulars.elementExpressionPattern, RegexOptions.Compiled );

      public ElementSelector( string expression )
      {

        if ( string.IsNullOrEmpty( expression ) )
          throw new FormatException();

        var match = regex.Match( expression );
        if ( !match.Success )
          throw new FormatException();


        if ( match.Groups["name"].Success )
          tagName = match.Groups["name"].Value;
        else
          tagName = "*";

        attributeSelectors = match.Groups["attributeSelector"].Captures.Cast<Capture>().Select( c => new AttributeSelector( c.Value ) ).ToList();

        if ( match.Groups["identity"].Success )
          attributeSelectors.Add( new AttributeSelector( string.Format( "[id={0}]", match.Groups["identity"].Value ) ) );

        if ( match.Groups["class"].Success )
          attributeSelectors.Add( new AttributeSelector( string.Format( "[class*={0}]", match.Groups["class"].Value ) ) );
      }


      private string tagName;

      private readonly IList<AttributeSelector> attributeSelectors;


      public bool Allows( IHtmlElement element )
      {
        if ( element == null )
          return false;


        if ( tagName != "*" && !string.Equals( element.Name, tagName, StringComparison.InvariantCultureIgnoreCase ) )
          return false;

        foreach ( var selector in attributeSelectors )
        {
          if ( !selector.Allows( element ) )
            return false;
        }

        return true;
      }


      public override string ToString()
      {
        return string.Format( "{0}{1}", tagName.ToUpper(), string.Join( "", attributeSelectors.Select( a => a.ToString() ).ToArray() ) );
      }

    }



    public class AttributeSelector
    {


      private readonly string name;
      private readonly string separator;
      private readonly string value;

      private readonly string exp;


      private static readonly Regex regex = new Regex( Regulars.attributeExpressionPattern, RegexOptions.Compiled );


      private delegate bool ValueMatcher( string exp, string value );

      private static readonly Dictionary<string, ValueMatcher> matchers = new Dictionary<string, ValueMatcher>()
      {
        { "^=", ( exp, value ) => value != null && value.StartsWith( exp ) },
        { "$=", ( exp, value ) => value != null && value.EndsWith( exp ) },
        { "*=", ( exp, value ) => value != null && value.Contains( exp ) },
        { "~=", ( exp, value ) => value != null && value.Split( ' ' ).Contains( exp ) },
        { "!=", ( exp, value ) => value != exp },
        { "=",  ( exp, value ) => value == exp }
      };





      public AttributeSelector( string expression )
      {

        exp = expression;

        var match = regex.Match( expression );

        if ( !match.Success )
          throw new FormatException();


        name = match.Groups["name"].Value;
        if ( match.Groups["separator"].Success )
        {
          separator = match.Groups["separator"].Value;
          if ( match.Groups["quoteText"].Success )
            value = match.Groups["quoteText"].Value;
          else
            value = match.Groups["value"].Value;
        }

      }


      public bool Allows( IHtmlElement element )
      {

        string _value = null;

        var attribute = element.Attribute( name );

        if ( separator == null )//如果没有运算符，那么表示判断属性是否存在
          return attribute != null;

        if ( attribute != null )
          _value = attribute.Value;

        return matchers[separator]( value, _value );
      }


      public override string ToString()
      {
        if ( separator != null )
          return string.Format( "[{0}{1}'{2}']", name, separator, value.Replace( "'", "\\'" ).Replace( "\"", "\\\"" ) );
        else
          return string.Format( "[{0}]", name );
      }

    }



    public class PseudoClassSelector
    {

    }

  }
}
