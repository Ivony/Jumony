using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ivony.Fluent;

namespace Ivony.Html.Forms
{

  /// <summary>
  /// 按钮组，是input type="checkbox"及input type="radio"组的抽象。
  /// </summary>
  public class HtmlButtonGroup : IHtmlGroupControl
  {


    private readonly HtmlInputButtonGroupItem[] items;
    private readonly string name;
    private readonly HtmlForm _form;



    private HtmlButtonGroup( HtmlForm form, IGrouping<string, IHtmlElement> inputGroup )
    {
      _form = form;

      name = inputGroup.Key;

      items = inputGroup.Select( e => new HtmlInputButtonGroupItem( this, e ) ).ToArray();
    }


    public static IEnumerable<HtmlButtonGroup> CaptureInputGroups( HtmlForm form )
    {

      var inputItems = form.Element.Find( "input[type=radio][name] , input[type=checkbox][name]" );

      var groups = inputItems.GroupBy( item => item.Attribute( "name" ).Value() )
          .Select( item => new HtmlButtonGroup( form, item ) );

      return groups;

    }


    public string Name
    {
      get { return name; }
    }

    public bool AllowMultipleSelections
    {
      get
      {
        //如果有任何一个复选框
        if ( items.Select( i => i.Element ).Any( e => e.Attribute( "type" ).Value().EqualsIgnoreCase( "checkbox" ) ) )
          return true;

        return false;
      }
    }


    public IHtmlInputGroupItem[] Items
    {
      get { return items; }
    }


    public HtmlForm Form
    {
      get { return _form; }
    }


    public IHtmlInputGroupItem this[string value]
    {
      get { return items.FirstOrDefault( i => i.Value == value ); }
    }






  }
}
