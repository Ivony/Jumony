using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ivony.Web.Html.Forms
{
  public interface IHtmlInput
  {

    string Name { get; }

    string Value { get; set; }

  }

  public interface IHtmlInputGroup : IHtmlInput
  {
    IHtmlInputControl InputControl( string vale );

    string[] Values { get; }

  }

  public interface IHtmlInputControl : IHtmlElement
  {
    string Name { get; }

    string Value { get; }

    bool Checked { get; }
  }
}
