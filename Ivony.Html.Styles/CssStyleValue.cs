using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ivony.Html.Styles
{
  public interface ICssStyleValue
  {
    string ValueString { get; }
  }

  public abstract class CssStyleValue : ICssStyleValue
  {
    public static readonly CssStyleValue Inherit = InheritValue.Insatnce;

    public abstract string ValueString
    {
      get;
    }

    public override string ToString()
    {
      return ValueString;
    }

  }


  public class InheritValue : CssStyleValue
  {

    private InheritValue() { }

    public override string ValueString
    {
      get { return "inherit"; }
    }

    private static readonly InheritValue _instance = new InheritValue();

    public static InheritValue Insatnce { get { return _instance; } }
  }


  public class EnumStyleValue : CssStyleValue
  {
    private string _value;

    public EnumStyleValue( string value )
    {
      _value = value;
    }

    public override string ValueString
    {
      get { return _value; }
    }
  }


}
