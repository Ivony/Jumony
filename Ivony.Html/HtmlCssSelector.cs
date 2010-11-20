using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Collections;
using Ivony.Fluent;
using System.Diagnostics;

namespace Ivony.Html
{
  public class HtmlCssSelector
  {


    public static readonly Regex cssSelectorRegex = new Regex( "^" + Regulars.cssSelectorPattern + "$", RegexOptions.Compiled );

    public static readonly Regex extraRegex = new Regex( "^" + Regulars.extraExpressionPattern + "$", RegexOptions.Compiled );

    private readonly string[] _expressions;





    /// <summary>
    /// 创建CSS选择器实例
    /// </summary>
    /// <param name="expression">选择器表达式</param>
    /// <returns>CSS选择器</returns>
    public static HtmlCssSelector Create( string expression )
    {
      return Create( new[] { expression } );
    }

    /// <summary>
    /// 创建CSS选择器实例
    /// </summary>
    /// <param name="expressions">多个选择器表达式，结果会自动合并</param>
    /// <returns>CSS选择器</returns>
    public static HtmlCssSelector Create( params string[] expressions )
    {
      var selector = new HtmlCssSelector( expressions );

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
    private HtmlCssSelector( string[] expressions )
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

          Trace.Write( "Selector", string.Format( "Begin Enumerate Search \"{0}\"", coreSelector.ExpressionString ) );
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
          Trace.Write( "Selector", string.Format( "End Enumerate Search \"{0}\"", coreSelector.ExpressionString ) );
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
          Trace.Write( "Selector", string.Format( "Begin Enumerate Search \"{0}\"", coreSelector.ExpressionString ) );
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


    public static readonly Regex elementSelectorRegex = new Regex( Regulars.elementExpressionPattern, RegexOptions.Compiled );

    internal class ElementSelector
    {

      public ElementSelector( string expression )
      {

        if ( string.IsNullOrEmpty( expression ) )
          throw new FormatException();

        var match = elementSelectorRegex.Match( expression );
        if ( !match.Success )
          throw new FormatException();


        if ( match.Groups["name"].Success )
          _tagName = match.Groups["name"].Value;
        else
          _tagName = "*";

        var _attributeSelectors = match.Groups["attributeSelector"].Captures.Cast<Capture>().Select( c => new AttributeSelector( c.Value ) ).ToList();

        if ( match.Groups["identity"].Success )
          _attributeSelectors.Add( new AttributeSelector( string.Format( "[id={0}]", match.Groups["identity"].Value ) ) );

        if ( match.Groups["class"].Success )
          _attributeSelectors.Add( new AttributeSelector( string.Format( "[class~={0}]", match.Groups["class"].Value ) ) );

        attributeSelectors = _attributeSelectors.ToArray();

        pseudoClassSelectors = match.Groups["pseudoClassSelector"].Captures.Cast<Capture>().Select( c => PseudoClassFactory.Create( c.Value ) ).ToArray();

      }


      private string _tagName;

      private readonly AttributeSelector[] attributeSelectors;

      private readonly IPseudoClassSelector[] pseudoClassSelectors;


      public IEnumerable<IHtmlElement> Filter( IEnumerable<IHtmlElement> source )
      {
        return source.Where( item => Allows( item ) );
      }

      public bool Allows( IHtmlElement element )
      {
        if ( element == null )
          return false;


        if ( _tagName != "*" && !element.Name.EqualsIgnoreCase( _tagName ) )
          return false;

        foreach ( var selector in attributeSelectors )
        {
          if ( !selector.Allows( element ) )
            return false;
        }

        foreach ( var selector in pseudoClassSelectors )
        {
          if ( !selector.Allows( this, element ) )
            return false;
        }

        return true;
      }


      public IEnumerable<IHtmlElement> Search( IEnumerable<IHtmlElement> elements )
      {
        return elements.Where( e => Allows( e ) );
      }


      public override string ToString()
      {
        return string.Format( "{0}{1}{2}", _tagName.ToUpper(), string.Join( "", attributeSelectors.Select( a => a.ToString() ).ToArray() ), string.Join( "", pseudoClassSelectors.Select( p => p.ToString() ).ToArray() ) );
      }


      public string TagName { get { return _tagName; } }
    }





    public static readonly Regex attributeSelectorRegex = new Regex( Regulars.attributeExpressionPattern, RegexOptions.Compiled );

    private class AttributeSelector
    {

      private readonly string name;
      private readonly string separator;
      private readonly string value;

      private readonly string exp;


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

        var match = attributeSelectorRegex.Match( expression );

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
          _value = attribute.AttributeValue;

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


    private interface IPseudoClassSelector
    {
      bool Allows( ElementSelector elementSelector, IHtmlElement element );
    }


    private class PseudoClassFactory
    {
      private static readonly Regex pseudoClassRegex = new Regex( "^" + Regulars.pseudoClassPattern + "$", RegexOptions.Compiled );

      public static IPseudoClassSelector Create( string expression )
      {
        var match = pseudoClassRegex.Match( expression );

        if ( !match.Success )
          throw new FormatException();

        string name = match.Groups["name"].Value;

        string args = null;
        if ( match.Groups["args"].Success )
          args = match.Groups["args"].Value;

        return Create( name, args, expression );

      }

      public static IPseudoClassSelector Create( string name, string args, string expression )
      {
        switch ( name.ToLowerInvariant() )
        {
          case "nth-child":
          case "nth-last-child":
          case "nth-of-type":
          case "nth-last-of-type":
          case "first-child":
          case "last-child":
          case "first-of-type":
          case "last-of-type":
            return new NthPseudoClass( name, args, expression );

          case "only-child":
          case "only-of-type":
          case "empty":
            if ( args != null )
              throw new FormatException( string.Format( "{0} 伪类不能有参数", name ) );
            return new CountPseudoClass( name, args, expression );

          default:
            throw new NotSupportedException();
        }
      }

      private class NthPseudoClass : IPseudoClassSelector
      {

        private static readonly string expressionPattern = @"(?<augend>#interger)|((?<multiplier>((\+|\-)?#interger)|\-)n\p{Zs}*(?<augend>(\+|\-)\p{Zs}*#interger)?)".Replace( "#interger", Regulars.integerPattern );
        private static readonly Regex expressionRegex = new Regex( "^(" + expressionPattern + ")$", RegexOptions.Compiled );

        private string _name;
        private string _args;
        private string exp;

        private int multiplier;
        private int augend;


        private bool ofType;
        private bool last;
        private bool nth;


        public NthPseudoClass( string name, string args, string expression )
        {

          _name = name.ToLowerInvariant();

          var correctNames = new[] { "nth-child", "nth-last-child", "nth-of-type", "nth-last-of-type", "first-child", "last-child", "first-of-type", "last-of-type" };

          if ( !correctNames.Contains( _name ) )
            throw new InvalidOperationException();


          nth = _name.StartsWith( "nth-" );
          last = _name.Contains( "last-" );
          ofType = _name.Contains( "-of-type" );



          _args = null;

          if ( args != null )
            _args = args.Trim().ToLowerInvariant();

          exp = expression;

          if ( !nth )//没有 nth 前缀说明获取第一个
          {
            if ( !string.IsNullOrEmpty( args ) )//没有nth前缀的不能有参数
              throw new FormatException();

            _args = "1";
          }



          if ( _args == "odd" )
            _args = "2n+1";

          if ( args == "even" )
            _args = "2n";


          var match = expressionRegex.Match( _args );

          if ( !match.Success )
            throw new FormatException();


          multiplier = 0;//默认值是0，表示没有倍数选择
          augend = 0;

          if ( match.Groups["multiplier"].Success )
          {
            string _multiplier = match.Groups["multiplier"].Value;
            if ( _multiplier == "-" )//如果只有一个负号
              multiplier = -1;//那意味着负1
            else
              multiplier = int.Parse( match.Groups["multiplier"].Value );
          }

          if ( match.Groups["augend"].Success )
            augend = int.Parse( Regex.Replace( match.Groups["augend"].Value, @"\p{Zs}", "" ) );//这里的正则用于去掉符号与数字之间的空白
        }

        public bool Allows( ElementSelector elementSelector, IHtmlElement element )
        {

          List<IHtmlElement> siblings;

          if ( ofType )
            siblings = element.Siblings( element.Name ).ToList();
          else
            siblings = element.Siblings().ToList();

          if ( last )
            siblings.Reverse();

          return Check( siblings.IndexOf( element ) );
        }


        /// <summary>
        /// 检查元素所处的索引位置是否符合参数表达式要求。
        /// </summary>
        /// <param name="index">所处的索引位置</param>
        /// <returns></returns>
        public bool Check( int index )
        {
          index += 1;
          index = index - augend;//计算从 augend 开始的偏移量

          if ( multiplier == 0 )//如果没有倍数选择，那么判断元素是否恰好在 augend 的位置。
            return index == 0;

          if ( multiplier > 0 )//如果倍数大于0
          {
            if ( index < 0 )//在 augend 之前的元素被忽略
              return false;

            if ( index % multiplier != 0 )//看位置是否符合倍数
              return false;

            return true;
          }


          if ( multiplier < 0 )//如果倍数小于0
          {
            index = -index;//反转索引位置，换算成从 augend 往前的偏移量

            if ( index < 0 )//在 augend 之后的元素忽略
              return false;

            if ( index % Math.Abs( multiplier ) != 0 )//看位置是否符合倍数
              return false;

            return true;
          }


          throw new Exception( "分析nth伪类时出现了一个其他未知情况" );

        }


        public override string ToString()
        {

          string argsExp = null;
          if ( multiplier == 0 )
            argsExp = multiplier + "n";

          if ( argsExp != null )
          {
            if ( augend < 0 )
              argsExp += "-";
            else
              argsExp += "+";

            argsExp += Math.Abs( augend ).ToString();
          }
          else
            argsExp = augend.ToString();

          return string.Format( ":nth-child({1})", argsExp );
        }

      }



      private class CountPseudoClass : IPseudoClassSelector
      {


        private readonly string _name;
        private readonly string _args;
        private readonly string _expression;


        private static readonly string[] correctNames = new[] { "only-child", "only-of-type", "empty" };



        public CountPseudoClass( string name, string args, string expression )
        {
          _name = name.ToLowerInvariant();


          if ( !correctNames.Contains( _name ) )
            throw new InvalidOperationException();


          if ( args != null )
            throw new InvalidOperationException( string.Format( "{0} 伪类不能有参数", name ) );



          _args = args;
          _expression = expression;


        }

        #region IPseudoClassSelector 成员

        public bool Allows( ElementSelector elementSelector, IHtmlElement element )
        {
          switch ( _name )
          {
            case "only-child":
              return element.Siblings().Count() == 1;
            case "only-of-type":
              return element.Siblings( element.Name ).Count() == 1;
            case "empty":
              return element.Elements().Count() == 0;

            default:
              throw new InvalidOperationException();
          }
        }

        #endregion
      }



    }

  }
}
