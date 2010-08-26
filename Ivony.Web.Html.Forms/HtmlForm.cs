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


    private HtmlInputText[] inputTexts;
    private IHtmlInputGroup[] inputGroups;
    private HtmlLabel[] labels;




    private void Initialize()
    {
      inputTexts = Element.Find( "input[type=text]", "input[type=password]", "input[type=hidden]", "textarea" )
          .Select( element => new HtmlInputText( element ) ).ToArray(); ;


      inputGroups = Element.Find( "select" ).Select( select => new HtmlSelect( this, select ) ).Cast<IHtmlInputGroup>()
        .Union( HtmlButtonGroup.CaptureInputGroups( this ).Cast<IHtmlInputGroup>() ).ToArray();

      labels = Element.Find( "label" ).Select( element => new HtmlLabel( this, element ) ).ToArray();
    }
  }
}
