using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ivony.Fluent;


namespace Ivony.Html.Forms.Validation
{
  /// <summary>
  /// HTML 表单验证器，承担一个表单的验证。
  /// </summary>
  public abstract class HtmlFormValidator
  {

    protected HtmlFormValidator( HtmlForm form )
    {
      Form = form;
    }

    public HtmlForm Form
    {
      get;
      private set;
    }

    public void Init()
    {
      throw new NotImplementedException();
    }


    private bool valid;
    private bool validated = false;


    public virtual void Validate()
    {

      if ( Form.SubmittedValues == null )
        throw new InvalidOperationException( "表单尚未提交，无法进行验证" );

      if ( validated )
        throw new InvalidOperationException( "表单已执行验证，无法再次执行" );


      _validators.ForAll( v => v.Validate() );



      validated = true;



      valid = _validators.All( v => v.IsValid );

      if ( valid )
        OnSuccessful();
      else
        OnFailed();
    }

    public bool IsValid
    {
      get
      {

        if ( validated == null )
          Validate();

        return (bool) valid;
      }
    }



    private List<HtmlFieldValidator> _validators = new List<HtmlFieldValidator>();


    protected virtual IHtmlElement GetMessageContainer( IHtmlInputControl input )
    {
      return null;
    }

    protected virtual IHtmlElement GetSummaryContainer()
    {
      return null;
    }




    protected virtual void OnFailed()
    {

      ShowFailedMessage();



      if ( Failed != null )
        Failed( this, EventArgs.Empty );
    }



    protected virtual void ShowFailedMessage()
    {
      var validators = _validators.Where( v => !v.IsValid );

      foreach ( var v in validators )
      {
        var messageContainer = GetMessageContainer( v.InputControl );
        if ( messageContainer != null )
          v.ShowFailedMessage( messageContainer );
      }
    }

    public EventHandler Failed;




    protected virtual void OnSuccessful()
    {
      if ( Successful != null )
        Successful( this, EventArgs.Empty );
    }

    public EventHandler Successful;



    protected virtual string GetFieldName( IHtmlInputControl input )
    {

      var labels = input.Labels();

      if ( labels.Length == 1 )
        return labels[0].Text;
      else
        return input.Name;

    }


    protected void AddFieldValidation( string inputName, params IHtmlValueValidator[] validators )
    {
      AddFieldValidation( inputName, null, validators );
    }


    protected void AddFieldValidation( string inputName, string fieldName, params IHtmlValueValidator[] validators )
    {

      if ( inputName == null )
        throw new ArgumentNullException( "inputName" );

      var input = Form.InputElement( inputName );

      if ( input == null )
        throw new ArgumentException( string.Format( "未能找到 \"name\" 属性为 \"{0}\" 的输入控件", inputName ), "inputName" );

      AddFieldValidation( input, fieldName, validators );
    }

    protected void AddFieldValidation( IHtmlInputControl input, string fieldName, params IHtmlValueValidator[] validators )
    {
      if ( input == null )
        throw new ArgumentNullException( "input" );


      if ( !input.Form.Equals( Form ) )
        throw new ArgumentException( "输入控件不属于验证表单", "input" );


      if ( fieldName == null )
        fieldName = GetFieldName( input );

      var fieldValidator = new HtmlFieldValidator( input, fieldName, validators );
      _validators.Add( fieldValidator );
    }

  }
}
