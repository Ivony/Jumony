using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ivony.Html
{

  /// <summary>
  /// CSS 关系选择器基类
  /// </summary>
  public abstract class CssRelativeSelector : ISelector
  {

    /// <summary>
    /// 创建 CssRelativeSelector 对象
    /// </summary>
    /// <param name="leftSelector"></param>
    protected CssRelativeSelector( ISelector leftSelector )
    {
      LeftSelector = leftSelector;
    }

    /// <summary>
    /// 获取左选择器
    /// </summary>
    public virtual ISelector LeftSelector
    {
      get;
      private set;
    }

    /// <summary>
    /// 检查元素是否符合选择器
    /// </summary>
    /// <param name="element">要检验的元素</param>
    /// <returns>是否符合选择器</returns>
    public virtual bool IsEligible( IHtmlElement element )
    {
      if ( element == null )
        return false;

      return IsEligible( LeftSelector, element );
    }

    /// <summary>
    /// 检查元素是否符合选择器
    /// </summary>
    /// <param name="leftSelector">左选择器</param>
    /// <param name="element">要检验的元素</param>
    /// <returns>是否符合选择器</returns>
    protected abstract bool IsEligible( ISelector leftSelector, IHtmlElement element );


    /// <summary>
    /// 结合符
    /// </summary>
    public abstract char Combinator { get; }

    /// <summary>
    /// 重写 ToString 方法输出选择器表达式
    /// </summary>
    /// <returns>选择器表达式形式</returns>
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

    public CssParentRelativeSelector( ISelector leftSelector ) : base( leftSelector ) { }


    protected override bool IsEligible( ISelector leftSelector, IHtmlElement element )
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
    public CssAncetorRelativeSelector( ISelector leftSelector ) : base( leftSelector ) { }

    protected override bool IsEligible( ISelector leftSelector, IHtmlElement element )
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
    public CssPreviousRelativeSelector( ISelector leftSelector ) : base( leftSelector ) { }

    protected override bool IsEligible( ISelector leftSelector, IHtmlElement element )
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

    public CssSiblingsRelativeSelector( ISelector leftSelector ) : base( leftSelector ) { }

    protected override bool IsEligible( ISelector leftSelector, IHtmlElement element )
    {
      var previous = element.PreviousElement();

      return leftSelector.IsEligibleBuffered( previous ) || this.IsEligibleBuffered( previous );
    }

    public override char Combinator { get { return '~'; } }
  }



  internal class ContainerRestrict : ISelector
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

    bool ISelector.IsEligible( IHtmlElement element )
    {
      throw new NotSupportedException();
    }
  }
}
