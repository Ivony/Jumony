using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ivony.Fluent;

namespace Ivony.Html.Web.Binding
{

  /// <summary>
  /// 默认绑定对象提供程序
  /// </summary>
  public class DefaultBindingProvider : IBindingProvider
  {
    public IEnumerable<IBinding> CreateBindings( BindingManager manager, IHtmlElement element )
    {
      if ( element.Name.EqualsIgnoreCase( "binding" ) )
      {
        return new[] { CreateElementBinding( element ) };
      }
      else
      {
        return element.Attributes().Select( a => CreateAttributeBinding( a ) ).NotNull();
      }
    }

    private IBinding CreateElementBinding( IHtmlElement element )
    {
      return new Binding( element, element.Attributes().ToDictionary( a => a.Name, a => a.AttributeValue ) );
    }

    private IBinding CreateAttributeBinding( IHtmlAttribute attribute )
    {
      var args = BindingExpression.ParseExpression( attribute );
      if ( args == null )
        return null;

      return new Binding( attribute, args );

    }

    public IEnumerable<IBinding> CreateBindings( BindingManager manager, IHtmlDocument document )
    {
      return Enumerable.Empty<IBinding>();
    }
  }
}
