using System;
namespace Ivony.Html.Forms
{
  public interface IHtmlGroupControl : IHtmlInputControl
  {

    bool AllowMultipleSelections
    {
      get;
    }


    IHtmlInputGroupItem[] Items
    {
      get;
    }
  }

  public interface IHtmlInputGroupItem : IHtmlFormElement
  {

    IHtmlElement Element
    {
      get;
    }

    IHtmlGroupControl Group
    {
      get;
    }

    bool Selected
    {
      get;
      set;
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
