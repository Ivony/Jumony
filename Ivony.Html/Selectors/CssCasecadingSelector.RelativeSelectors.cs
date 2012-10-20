using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ivony.Html
{
  partial class CssCasecadingSelector
  {
    /// <summary>
    /// 创建关系选择器
    /// </summary>
    /// <param name="leftSelector">左选择器</param>
    /// <param name="relative">关系运算符</param>
    /// <returns>关系选择器</returns>
    private static ICssSelector CreateRelativeSelector( ICssSelector leftSelector, string relative )
    {
      if ( relative == ">" )
        return new ParentRelativeSelector( leftSelector );

      else if ( relative == "" )
        return new AncetorRelativeSelector( leftSelector );

      else if ( relative == "+" )
        return new PreviousRelativeSelector( leftSelector );

      else if ( relative == "~" )
        return new SiblingsRelativeSelector( leftSelector );

      throw new NotSupportedException( "不支持的关系运算符" );
    }

    /// <summary>
    /// 父级关系选择器
    /// </summary>
    private class ParentRelativeSelector : ICssSelector
    {

      private ICssSelector _leftSelector;

      public ParentRelativeSelector( ICssSelector leftSelector )
      {
        _leftSelector = leftSelector;
      }

      public bool IsEligible( IHtmlElement element )
      {
        if ( element == null )
          return false;

        return _leftSelector.IsEligibleBuffered( element.Parent() );
      }

      public override string ToString()
      {
        return _leftSelector.ToString() + " > ";
      }

    }


    /// <summary>
    /// 父代关系选择器
    /// </summary>
    private class AncetorRelativeSelector : ICssSelector
    {
      private ICssSelector _leftSelector;

      public AncetorRelativeSelector( ICssSelector leftSelector )
      {
        _leftSelector = leftSelector;
      }

      public bool IsEligible( IHtmlElement element )
      {
        if ( element == null )
          return false;

        var parant = element.Parent();

        //如果父级元素符合左选择器，或者父级元素符合本选择器。
        return _leftSelector.IsEligibleBuffered( parant ) || this.IsEligibleBuffered( parant );
      }


      public override string ToString()
      {
        return _leftSelector.ToString() + " ";
      }
    }


    /// <summary>
    /// 毗邻关系选择器
    /// </summary>
    private class PreviousRelativeSelector : ICssSelector
    {
      private ICssSelector _leftSelector;

      public PreviousRelativeSelector( ICssSelector leftSelector )
      {
        _leftSelector = leftSelector;
      }

      public bool IsEligible( IHtmlElement element )
      {
        if ( element == null )
          return false;

        return _leftSelector.IsEligibleBuffered( element.PreviousElement() );
      }

      public override string ToString()
      {
        return _leftSelector.ToString() + " + ";
      }
    }


    /// <summary>
    /// 兄弟元素选择器
    /// </summary>
    private class SiblingsRelativeSelector : ICssSelector
    {
      private ICssSelector _leftSelector;

      public SiblingsRelativeSelector( ICssSelector leftSelector )
      {
        _leftSelector = new PreviousRelativeSelector( leftSelector );
      }

      public bool IsEligible( IHtmlElement element )
      {
        var previous = element.PreviousElement();

        return _leftSelector.IsEligibleBuffered( previous ) || this.IsEligibleBuffered( previous );
      }


      public override string ToString()
      {
        return _leftSelector.ToString() + " ~ ";
      }
    }

  }
}
