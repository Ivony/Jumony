using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ivony.Html.Forms.Validation;
using Ivony.Html.Forms;
using System.Web.Mvc;

namespace Ivony.Html.Web.Mvc
{
  public class MvcFormValidator : HtmlFormValidator
  {

    public MvcFormValidator( HtmlForm form, ModelStateDictionary modelStates )
      : base( form )
    { 

    }


    protected override void ExecuteValidate()
    {



    }

  }
}
