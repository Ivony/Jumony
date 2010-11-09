using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ivony.Fluent;


namespace Ivony.Html.Forms.Validation
{
  /// <summary>
  /// HTML 表单验证器，承担一个表单的验证。这是一个抽象类，具体的表单验证类型应从此类型继承。
  /// </summary>
  public abstract class HtmlFormValidator
  {

    /// <summary>
    /// 构造表单验证器
    /// </summary>
    /// <param name="form">要被验证的表单</param>
    protected HtmlFormValidator( HtmlForm form )
    {
      Form = form;
    }


    /// <summary>
    /// 获取验证器验证的表单
    /// </summary>
    public HtmlForm Form
    {
      get;
      private set;
    }


    /// <summary>
    /// 初始化验证器
    /// </summary>
    public virtual void Initialize()
    {
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

        if ( !validated )
          Validate();

        return (bool) valid;
      }
    }



    private List<GenericFieldValidator> _validators = new List<GenericFieldValidator>();


    protected virtual IHtmlElement FailedMessageContainer( IHtmlInputControl input )
    {
      return null;
    }

    protected virtual IHtmlElement FailedSummaryContainer()
    {
      return null;
    }

    protected virtual IHtmlElement FieldDescrptionContainer( IHtmlInputControl input )
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
      var failedDalidators = _validators.Where( v => !v.IsValid );

      foreach ( var v in failedDalidators )
      {
        var messageContainer = FailedMessageContainer( v.InputControl );
        if ( messageContainer != null )
        {
          var messages = v.FailedMessage();

          RenderMessages( messageContainer, messages );
        }
      }

      var summaryContainer = FailedSummaryContainer();
      if ( summaryContainer != null )
      {
        IHtmlElement list;


        IHtmlNodeFactory factory;

        list = EnsureList( summaryContainer, out factory );

        foreach ( var v in failedDalidators )
        {
          var messages = v.FailedMessage();

          var item = (IHtmlElement) factory.CreateElement( "li" ).AppendTo( summaryContainer );

          RenderMessages( item, messages );
        }
      }
    }


    protected virtual void RenderMessages( IHtmlElement container, string[] messages )
    {

      if ( messages.Length == 0 )
        return;

      else if ( messages.Length == 1 )
        container.InnerText( messages[0] );

      else
      {
        IHtmlNodeFactory factory;
        var list = EnsureList( container, out factory );
        foreach ( var m in messages )
        {
          var item = (IHtmlElement) factory.CreateElement( "li" ).AppendTo( list );
          item.InnerText( m );
        }
      }

      container.InnerText( string.Join( "\n", messages ) );
    }

    protected virtual void ShowFieldDescription()
    {
      foreach ( var field in _validators )
      {
        var container = FieldDescrptionContainer( field.InputControl );

        if ( container != null )
        {

          var rules = field.RuleDescription();


          IHtmlNodeFactory factory;
          var list = EnsureList( container, out factory );

          foreach ( var r in rules )
          {
            var item = (IHtmlElement) factory.CreateElement( "li" ).AppendTo( list );
            item.InnerText( r );
          }
        }

      }
    }




    protected IHtmlElement EnsureList( IHtmlElement container, out IHtmlNodeFactory factory )
    {
      factory = container.Document.GetNodeFactory();

      if ( HtmlSpecification.listElements.Contains( container.Name.ToLowerInvariant() ) )
        return container;


      return factory.CreateElement( "ul" ).InsertTo( container, 0 );
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

      var fieldValidator = new GenericFieldValidator( input, fieldName, validators );
      _validators.Add( fieldValidator );
    }



    protected class GenericFieldValidator : HtmlFieldValidator
    {

      private readonly IHtmlValueValidator[] _validators;


      public GenericFieldValidator( IHtmlInputControl inputControl, string fieldName, params IHtmlValueValidator[] validators )
        : base( inputControl, fieldName )
      {
        _validators = validators;
      }


      private IHtmlValueValidator failedValidator;

      protected override bool ExecuteValidate( string value )
      {

        failedValidator = _validators.FirstOrDefault( v => v.Validate( value ) == false );

        return failedValidator == null;
      }


      public override string[] FailedMessage()
      {
        if ( IsValid )
          return null;

        return new string[] { failedValidator.ErrorMessage.Replace( "<field>", FieldName ) };
      }

      public override string[] RuleDescription()
      {
        return _validators.Select( v => v.RuleDescription ).ToArray();
      }
    }


  }
}
