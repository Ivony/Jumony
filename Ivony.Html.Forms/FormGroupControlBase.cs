using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ivony.Html.Forms
{
  public abstract class FormGroupControlBase : FormControl
  {

    protected FormGroupControlBase( HtmlForm form )
      : base( form )
    {

    }

    public abstract string[] CandidateValues
    {
      get;
    }


    public abstract bool AllowMultiple
    {
      get;
    }


    public abstract string[] Values
    {
      get;
    }



    public override bool CanSetValue( string value )
    {
      var values = value.Split( ',' );
      return values.All( v => CandidateValues.Contains( v ) );
    }

    public override string Value
    {
      get { return string.Join( ",", Values ); }
    }

  }
}
