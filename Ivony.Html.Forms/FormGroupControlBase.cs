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
      set { SetValue( value ); }
    }



    protected virtual void SetValue( string valuesExpression )
    {

      valuesExpression = valuesExpression ?? "";

      var values = valuesExpression.Split( ',' );


      var invalidValue = values.Except( CandidateValues );

      if ( invalidValue.Any() )//如果有一个设置的值不在候选值列表
      {
        if ( Form.Configuration.ExceptionOnInvailidValues )
          throw new InvalidOperationException( string.Format( "不能对控件设置值 \"{0}\"", invalidValue.First() ) );
      }

      SetValues( new HashSet<string>( values.Except( invalidValue ) ) );
    }

    protected abstract void SetValues( HashSet<string> values );
  }
}
