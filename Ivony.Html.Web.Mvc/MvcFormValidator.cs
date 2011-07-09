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


    private ModelStateDictionary _modelStates;

    public MvcFormValidator( HtmlForm form, ModelStateDictionary modelStates )
      : base( form )
    {
      _modelStates = modelStates;
    }


    protected override bool ExecuteValidate()
    {

      if ( _modelStates.IsValid )
        return true;


      foreach ( var key in _modelStates.Keys )
      {
        var state = _modelStates[key];

      }

      return false;


    }

  }
}
