using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ivony.Html
{

  /// <summary>
  /// CSS 关系选择器基类
  /// </summary>
  public abstract class CssRelativeSelector : ICssSelector
  {

    protected CssRelativeSelector( ICssSelector leftSelector )
    {
      LeftSelector = leftSelector;
    }

    public virtual ICssSelector LeftSelector
    {
      get;
      private set;
    }

    public virtual bool IsEligible( IHtmlElement element )
    {
      if ( element == null )
        return false;

      return IsEligible( LeftSelector, element );
    }

    protected abstract bool IsEligible( ICssSelector leftSelector, IHtmlElement element );



    public abstract char Combinator { get; }

    public override string ToString()
    {
      return LeftSelector.ToString() + Combinator;
    }
  }


  /// <summary>
  /// 父级关系选择器
  /// </summary>
  internal class CssParentRelativeSelector : CssRelativeSelector
  {

    public CssParentRelativeSelector( ICssSelector leftSelector ) : base( leftSelector ) { }


    protected override bool IsEligible( ICssSelector leftSelector, IHtmlElement element )
    {
      var restrict = leftSelector as ContainerRestrict;
      if ( restrict != null )
        return restrict.RestrictContainer.Nodes().Contains( element );

      return leftSelector.IsEligibleBuffered( element.Parent() );
    }

    public override char Combinator { get { return '>'; } }

  }


  /// <summary>
  /// 父代关系选择器
  /// </summary>
  internal class CssAncetorRelativeSelector : CssRelativeSelector
  {
    public CssAncetorRelativeSelector( ICssSelector leftSelector ) : base( leftSelector ) { }

    protected override bool IsEligible( ICssSelector leftSelector, IHtmlElement element )
    {

      var restrict = leftSelector as ContainerRestrict;
      if ( restrict != null )
        return element.IsDescendantOf( restrict.RestrictContainer );


      var parant = element.Parent();

      //如果父级元素符合左选择器，或者父级元素符合本选择器。
      return leftSelector.IsEligibleBuffered( parant ) || this.IsEligibleBuffered( parant );
    }

    public override char Combinator { get { return ' '; } }

  }


  /// <summary>
  /// 毗邻关系选择器
  /// </summary>
  internal class CssPreviousRelativeSelector : CssRelativeSelector
  {
    public CssPreviousRelativeSelector( ICssSelector leftSelector ) : base( leftSelector ) { }

    protected override bool IsEligible( ICssSelector leftSelector, IHtmlElement element )
    {
      return leftSelector.IsEligibleBuffered( element.PreviousElement() );
    }

    public override char Combinator { get { return '+'; } }

  }


  /// <summary>
  /// 兄弟元素选择器
  /// </summary>
  internal class CssSiblingsRelativeSelector : CssRelativeSelector
  {

    public CssSiblingsRelativeSelector( ICssSelector leftSelector ) : base( leftSelector ) { }

    protected override bool IsEligible( ICssSelector leftSelector, IHtmlElement element )
    {
      var previous = element.PreviousElement();

      return leftSelector.IsEligibleBuffered( previous ) || this.IsEligibleBuffered( previous );
    }

    public override char Combinator { get { return '~'; } }
  }



  internal class ContainerRestrict : ICssSelector
  {

    public ContainerRestrict( IHtmlContainer container )
    {
      RestrictContainer = container;
    }

    public IHtmlContainer RestrictContainer
    {
      get;
      private set;
    }

    bool ICssSelector.IsEligible( IHtmlElement element )
    {
      throw new NotSupportedException();
    }
  }
}
