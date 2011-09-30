using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Ivony.Html
{

  /// <summary>
  /// 提供一个简单的 IHtmlAdapter 的实现，方便重写对指定元素的渲染规则。
  /// </summary>
  public abstract class HtmlElementAdapter : IHtmlAdapter
  {

    bool IHtmlAdapter.Render( IHtmlNode node, TextWriter writer )
    {
      var element = node as IHtmlElement;

      if ( element == null )
        return false;

      if ( !IsEligible( element ) )
        return false;

      Render( element, writer );
      return true;
    }


    /// <summary>
    /// 派生类重写此方法确定指定元素是否需要重写渲染规则
    /// </summary>
    /// <param name="element">要检测的元素</param>
    /// <returns>是否需要使用自定义渲染规则</returns>
    protected virtual bool IsEligible( IHtmlElement element )
    {
      if ( CssSelector == null )
        return false;

      if ( _selectorExpression == CssSelector )
        return _selectorCache.IsEligible( element );

      _selectorExpression = CssSelector;
      _selectorCache = Ivony.Html.CssSelector.Create( CssSelector );


      return _selectorCache.IsEligible( element );
    }


    private string _selectorExpression;
    private ICssSelector _selectorCache;



    /// <summary>
    /// 派生类重写此属性使用一个选择器来确定元素是否需要重写渲染规则
    /// </summary>
    protected virtual string CssSelector
    {
      get { return null; }
    }

    /// <summary>
    /// 派生类实现此方法提供自定义的渲染规则。
    /// </summary>
    /// <param name="element">要渲染的元素</param>
    /// <param name="writer">渲染输出的 TextWriter</param>
    public abstract void Render( IHtmlElement element, TextWriter writer );
  }
}
