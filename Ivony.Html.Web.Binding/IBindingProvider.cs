using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ivony.Html.Web.Binding
{
  public interface IBindingProvider
  {
    IBinding CreateBinding( BindingManager manager, IHtmlDomObject targetObject, IDictionary<string, string> args );
  }

  public interface IBindingTargetProvider
  {

    

    IBindingTarget CreateTarget( BindingManager manager, IHtmlDomObject bindingHost, object value );
  }

  public enum BindingHostType
  { 
    Attribute,
    Element,
    Both
  }



}
