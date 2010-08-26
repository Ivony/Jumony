using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ivony.Fluent;

namespace Ivony.Web.Html.Forms
{
  public class HtmlForm : IHtmlForm
  {
    private IHtmlElement _element;

    public IHtmlElement Element
    {
      get { return _element; }
    }

    public HtmlForm( IHtmlElement element )
    {
      _element = element;

      Initialize();

    }


    private HtmlInputText[] inputTexts;
    private IHtmlInputGroup[] inputGroups;

    public IEnumerable<IHtmlInput> InputControls
    {
      get { return inputTexts.Cast<IHtmlInput>().Union( inputGroups.Cast<IHtmlInput>() ); }
    }


    /// <summary>
    /// 所有文本输入控件
    /// </summary>
    public HtmlInputText[] TextInputs
    {
      get { return inputTexts; }
    }

    /// <summary>
    /// 所有输入控件组
    /// </summary>
    public IHtmlInputGroup[] GroupInputs
    {
      get { return inputGroups; }
    }


    private void Initialize()
    {
      inputTexts = Element.Find( "input[type=text]", "input[type=password]", "input[type=hidden]", "textarea" )
          .Select( element => new HtmlInputText( element ) ).ToArray(); ;


      inputGroups = Element.Find( "select" ).Select( select => new HtmlSelect( select ) ).Cast<IHtmlInputGroup>()
        .Union( HtmlInputGroup.CaptureInputGroups( this ).Cast<IHtmlInputGroup>() ).ToArray();
    }
  }
}
