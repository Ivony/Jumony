using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ivony.Html.Web.Binding
{
  public interface IBindingProvider
  {
    IEnumerable<IBinding> CreateBindings( BindingManager manager, IHtmlElement element );
  }

  public class DataContextAttributeBindingProvider : IBindingProvider
  {

    public IEnumerable<IBinding> CreateBindings( BindingManager manager, IHtmlElement element )
    {
      /*
      var bindingArgs = BindingManager.ParseExpression( element.Attribute( "datacontext" ) );

      var binding = CreateBinding( manager, element, bindingArgs );

      if ( binding != null )
        yield return binding;

      yield break;
      */

      throw new NotImplementedException();
    }

    protected IBinding CreateBinding( BindingManager manager, IHtmlElement element, IDictionary<string, string> bindingArgs )
    {
      if ( bindingArgs == null )
        return null;

      return new DataContextBinding( manager, element, bindingArgs );
    }

    public class DataContextBinding : IBinding
    {
      private BindingManager _manager;
      private IHtmlElement _element;
      private IDictionary<string, string> _bindingArgs;

      public DataContextBinding( BindingManager manager, IHtmlElement element, IDictionary<string, string> bindingArgs )
      {
        _manager = manager;
        _element = element;
        _bindingArgs = bindingArgs;
      }


      public void DataBind( object dataContext )
      {
        _manager.GetValue( dataContext, _bindingArgs );
      }

      void IBinding.DataBind( object dataContext )
      {
        throw new NotImplementedException();
      }

      int IBinding.Priority
      {
        get { throw new NotImplementedException(); }
      }
    }


  }


}
