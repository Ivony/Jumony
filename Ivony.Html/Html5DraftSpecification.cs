using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ivony.Fluent;

#pragma warning disable 1591

namespace Ivony.Html
{


  /// <summary>
  /// 定义 HTML5 草案规范
  /// </summary>
  /// <remarks>此类型实例是只读且线程安全的</remarks>
  public sealed class Html5DraftSpecification : HtmlSpecificationBase
  {
    public override bool IsCDataElement( string elementName )
    {
      throw new NotImplementedException();
    }

    public override bool IsOptionalEndTag( string elementName )
    {
      throw new NotImplementedException();
    }

    public override bool IsForbiddenEndTag( string elementName )
    {
      throw new NotImplementedException();
    }

    public override bool ImmediatelyClose( string openTag, string nextTag )
    {
      throw new NotImplementedException();
    }

    public override bool IsBlockElement( IHtmlElement element )
    {
      throw new NotImplementedException();
    }

    public override bool IsInlineElement( IHtmlElement element )
    {
      throw new NotImplementedException();
    }

    public override bool IsSpecialElement( IHtmlElement element )
    {
      throw new NotImplementedException();
    }

    public override bool IsFormInputElement( IHtmlElement element )
    {
      throw new NotImplementedException();
    }

    public override bool IsStylingElement( IHtmlElement element )
    {
      throw new NotImplementedException();
    }

    public override bool IsListElement( IHtmlElement element )
    {
      throw new NotImplementedException();
    }

    public override bool IsPhraseElement( IHtmlElement element )
    {
      throw new NotImplementedException();
    }



    public override bool IsUriValue( IHtmlAttribute attribute )
    {
      throw new NotImplementedException();
    }

    public override bool IsScriptValue( IHtmlAttribute attribute )
    {
      throw new NotImplementedException();
    }

    public override bool IsMarkupAttribute( IHtmlAttribute attribute )
    {
      throw new NotImplementedException();
    }


    
    public override TextMode ElementTextMode( IHtmlElement element )
    {
      throw new NotImplementedException();
    }

    public override CssStyleSpecificationBase GetCssStyleSpecification()
    {
      return new Css21StyleSpecification();
    }

  }
}
