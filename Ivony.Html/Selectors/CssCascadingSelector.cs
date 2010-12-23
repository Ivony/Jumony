using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;

namespace Ivony.Html
{

  /// <summary>
  /// CSS层叠选择器
  /// </summary>
  internal sealed class CssCasecadingSelector : ICssSelectorWithScope
  {

    private readonly string _relative;
    /// <summary>
    /// 关系描述符
    /// </summary>
    public string Relative
    {
      get { return _relative; }
    }

    private readonly CssElementSelector _selector;
    /// <summary>
    /// 元素选择器
    /// </summary>
    public CssElementSelector ElementSelector
    {
      get { return _selector; }
    }

    private readonly CssCasecadingSelector _parent;
    /// <summary>
    /// 父级选择器
    /// </summary>
    public CssCasecadingSelector ParentSelector
    {
      get { return _parent; }
    }


    internal CssCasecadingSelector( CssElementSelector selector ) : this( selector, null, null ) { }

    internal CssCasecadingSelector( CssElementSelector selector, string relative, CssCasecadingSelector parent )
    {
      _selector = selector;
      _relative = relative.Trim();
      _parent = parent;
    }


    /// <summary>
    /// 检查元素是否符合选择条件
    /// </summary>
    /// <param name="element">要检查的元素</param>
    /// <param name="scope">范围限定，追溯父级到此为止</param>
    /// <returns>是否符合选择条件</returns>
    public bool IsEligible( IHtmlElement element, IHtmlContainer scope )
    {

      if ( !ElementSelector.IsEligible( element ) )
        return false;

      if ( Relative == null )
        return true;

      else if ( Relative == ">" )
        return element.Parent().Equals( scope ) ? false : ParentSelector.IsEligible( element.Parent(), scope );

      else if ( Relative == "" )
        return element.Ancestors().TakeWhile( e => !e.Equals( scope ) ).Any( e => ParentSelector.IsEligible( e, scope ) );

      else if ( Relative == "+" )
        return ParentSelector.IsEligible( element.PreviousElement(), scope );

      else if ( Relative == "~" )
        return element.SiblingsBeforeSelf().Any( e => ParentSelector.IsEligible( e, scope ) );

      else
        throw new NotSupportedException( string.Format( CultureInfo.InvariantCulture, "不支持的关系选择符 \"{0}\"", Relative ) );
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
