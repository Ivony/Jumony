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

    /// <summary>
    /// 表单元素
    /// </summary>
    public IHtmlElement Element
    {
      get { return _element; }
    }



    private IHtmlTextControl[] textControls;
    private IHtmlGroupControl[] groupControls;

    private HtmlLabel[] labels;

    private Hashtable labelsTable = Hashtable.Synchronized( new Hashtable() );
    private Hashtable controlsTable = Hashtable.Synchronized( new Hashtable() );



    /// <summary>
    /// 创建一个 HTML 表单对象
    /// </summary>
    /// <param name="element"></param>
    public HtmlForm( IHtmlElement element )
    {
      _element = element;


      RefreshForm();

    }


    /// <summary>
    /// 重新扫描表单中所有控件
    /// </summary>
    public void RefreshForm()
    {
      textControls =
      Element.Find( "input[type=text][name] , input[type=password][name] , input[type=hidden][name]" )
        .Select( e => new HtmlInputText( this, e ) ).Cast<IHtmlTextControl>()
        .Union( Element.Find( "textarea[name]" ).Select( e => new HtmlTextArea( this, e ) ).Cast<IHtmlTextControl>() )
        .ForAll( control => controlsTable.Add( control.Name, control ) )
        .ToArray();


      groupControls =
      Element.Find( "select[name]" )
        .Select( select => new HtmlSelect( this, select ) ).Cast<IHtmlGroupControl>()
        .Union( HtmlButtonGroup.CaptureInputGroups( this ).Cast<IHtmlGroupControl>() )
        .ForAll( control => controlsTable.Add( control.Name, control ) )
        .ToArray();




      labels = Element.Find( "label[for]" ).Select( e => new HtmlLabel( this, e ) ).ToArray();

      labels.GroupBy( l => l.ForElementId ).ForAll( grouping =>
        labelsTable.Add( grouping.Key, grouping.ToArray() ) );
    }




    /// <summary>
    /// 尝试提交表单
    /// </summary>
    /// <param name="data">提交的数据</param>
    /// <returns>被提交的表单</returns>
    public HtmlForm Submit( NameValueCollection data )
    {
      return Submit( data, true );
    }


    /// <summary>
    /// 尝试提交表单
    /// </summary>
    /// <param name="data">提交的数据</param>
    /// <param name="validateInputs">指示是否应当验证表单提交的数据是否与表单吻合</param>
    /// <returns>被提交的表单</returns>
    public HtmlForm Submit( NameValueCollection data, bool validateInputs )
    {

      if ( data == null )
        throw new ArgumentNullException( "data" );


      if ( SubmittedValues != null )
        throw new InvalidOperationException( "表单已经被提交过一次了" );

      var inputControlNames = InputControls.Select( input => input.Name ).ToArray();

      if ( validateInputs && data.AllKeys.Any( key => !inputControlNames.Contains( key ) ) )
        throw new InvalidOperationException();//如果表单尚有一些控件没有提交值，那么这是错误的。

      SubmittedValues = data;

      return this;
    }


    /// <summary>
    /// 获取表单提交的值，若表单尚未提交，则为 null
    /// </summary>
    public NameValueCollection SubmittedValues
    {
      get;
      private set;
    }



    /// <summary>
    /// 获取表单所有的输入控件
    /// </summary>
    public IEnumerable<IHtmlInputControl> InputControls
    {
      get { return controlsTable.Values.Cast<IHtmlInputControl>(); }
    }


    /// <summary>
    /// 获取指定名称的输入控件
    /// </summary>
    /// <param name="name">输入控件名</param>
    /// <returns>输入控件</returns>
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
    /// 所有组合输入控件
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
      if ( elementId == null )
        return new HtmlLabel[0];

      return labelsTable[elementId].CastTo<HtmlLabel[]>().IfNull( new HtmlLabel[0] );
    }

  }
}
