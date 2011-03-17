using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Ivony.Fluent;
using System.Collections.Specialized;
using System.Web;

namespace Ivony.Html.Forms
{
  public class HtmlForm
  {
    private IHtmlElement _element;

    public IHtmlElement Element
    {
      get { return _element; }
    }



    private IHtmlTextControl[] textControls;
    private IHtmlGroupControl[] groupControls;

    private HtmlLabel[] labels;

    private Hashtable labelsTable = Hashtable.Synchronized( new Hashtable() );
    private Hashtable controlsTable = Hashtable.Synchronized( new Hashtable() );



    public HtmlForm( IHtmlElement element )
    {
      _element = element;


      textControls =
      Element.Find( "input[type=text] , input[type=password] , input[type=hidden]" )
        .Select( e => new HtmlInputText( this, e ) ).Cast<IHtmlTextControl>()
        .Union( Element.Find( "textarea" ).Select( e => new HtmlTextArea( this, e ) ).Cast<IHtmlTextControl>() )
        .ForAll( control => controlsTable.Add( control.Name, control ) )
        .ToArray();


      groupControls =
      Element.Find( "select" )
        .Select( select => new HtmlSelect( this, select ) ).Cast<IHtmlGroupControl>()
        .Union( HtmlButtonGroup.CaptureInputGroups( this ).Cast<IHtmlGroupControl>() )
        .ForAll( control => controlsTable.Add( control.Name, control ) )
        .ToArray();




      labels = Element.Find( "label[for]" ).Select( e => new HtmlLabel( this, e ) ).ToArray();

      labels.GroupBy( l => l.ForElementId ).ForAll( grouping =>
        labelsTable.Add( grouping.Key, grouping.ToArray() ) );

    }




    public void Submit( NameValueCollection data )
    {

      if ( SubmittedValues != null )
        throw new InvalidOperationException( "表单已经被提交过一次了" );

      if ( !data.Keys.Cast<string>().All( key => InputControls.Any( input => input.Name == key ) ) )
        throw new InvalidOperationException();

      SubmittedValues = data;
    }


    public NameValueCollection SubmittedValues
    {
      get;
      private set;
    }



    public IEnumerable<IHtmlInputControl> InputControls
    {
      get { return controlsTable.Values.Cast<IHtmlInputControl>(); }
    }

    public IHtmlInputControl this[string name]
    {
      get { return controlsTable[name].CastTo<IHtmlInputControl>(); }
    }


    /// <summary>
    /// 所有文本输入控件
    /// </summary>
    private IHtmlTextControl[] TextInputs
    {
      get { return textControls; }
    }

    /// <summary>
    /// 所有输入控件组
    /// </summary>
    private IHtmlGroupControl[] GroupInputs
    {
      get { return groupControls; }
    }



    /// <summary>
    /// 检索指定HTML元素绑定的 Label
    /// </summary>
    /// <param name="element">要检索 Label 的元素</param>
    /// <returns></returns>
    internal HtmlLabel[] FindLabels( string elementId )
    {
      return (HtmlLabel[]) labelsTable[elementId];
    }

  }
}
