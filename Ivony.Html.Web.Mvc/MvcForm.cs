using Ivony.Html.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace Ivony.Html.Web
{
  public class MvcForm : HtmlModelForm
  {

    public MvcForm( ViewDataDictionary viewData, IHtmlElement element, FormConfiguration configuration = null, IFormProvider provider = null )
      : base( viewData.Model, element, configuration, provider )
    {

      ModelState = viewData.ModelState;
      ModelMetadata = viewData.ModelMetadata;

    }


    protected ModelStateDictionary ModelState
    {
      get;
      private set;
    }

    protected ModelMetadata ModelMetadata
    {
      get;
      private set;
    }

  }
}
