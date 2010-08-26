using System;
namespace Ivony.Web.Html.Forms
{
  public interface IHtmlInputGroup : IHtmlInput
  {
    string[] Values
    {
      get;
    }
  }

  public interface IHtmlInputGroupItem
  {
    IHtmlInputGroup Group
    {
      get;
    }

    bool Selected
    {
      get;
    }

  }
}
