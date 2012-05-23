using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ivony.Html.Web.Binding
{

  public interface IBinding
  {
    void DataBind();
  }


  public abstract class ElementBinding : IBinding
  {
    public ElementBinding( IHtmlElement element )
    {
      Element = element;
    }

    public IHtmlElement Element
    {
      get;
      private set;
    }

    public void DataBind()
    {

      var dataItem = GetDataItem();


      DataBind( dataItem );
    }

    protected abstract void DataBind( object dataItem );

    private object GetDataItem()
    {
      throw new NotImplementedException();
    }
  }

}
