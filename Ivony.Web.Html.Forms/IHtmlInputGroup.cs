using System;
namespace Ivony.Web.Html.Forms
{
  public interface IHtmlInputGroup : IHtmlInput
  {

    bool AllowMultipleSelections
    {
      get;
    }


    IHtmlInputGroupItem[] Items
    {
      get;
    }

    string[] Values
    {
      get;
    }
  }

  public interface IHtmlInputGroupItem
  {

    IHtmlElement Element
    {
      get;
    }

    IHtmlInputGroup Group
    {
      get;
    }

    bool Selected
    {
      get;
    }

    string Value
    {
      get;
    }

    string Text
    {
      get;
    }
  }
}
