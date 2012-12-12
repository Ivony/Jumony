using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ivony.Html.Web.Binding
{
  public interface IValueBinder
  {

    void BindValue( IHtmlDomObject target, object value );

  }


  public interface IAttributeValueBinder<T>
  {

    void BindValue( IHtmlAttribute target, T value );

  }

  public interface IElementValueBinder<T>
  {

    void BindValue( IHtmlElement target, T value );

  }

}
