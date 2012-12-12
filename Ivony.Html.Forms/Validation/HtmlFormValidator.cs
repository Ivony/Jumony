using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ivony.Fluent;
using System.Globalization;


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


    /// <summary>
    /// 进行验证
    /// </summary>
    public virtual void Validate()
    {

      if ( validated )
        throw new InvalidOperationException( "表单已执行验证，无法再次执行" );

      valid = ExecuteValidate();

      validated = true;

      if ( valid )
        OnSuccessful();
      else
        OnFailed();

    }


    /// <summary>
    /// 派生类重写此方法实现具体的验证过程
    /// </summary>
    /// <returns></returns>
    protected virtual bool ExecuteValidate()
    {

      _validators.ForAll( v => v.Validate() );


      return _validators.All( v => v.IsValid );

    }


    /// <summary>
    /// 表单是否能够通过验证
    /// </summary>
    public bool IsValid
    {
      get
      {

        if ( !validated )
          Validate();

        return valid;
      }
    }



    private List<IHtmlFieldValidator> _validators = new List<IHtmlFieldValidator>();


    /// <summary>
    /// 派生类重写此方法查找显示指定输入控件验证错误信息的容器
    /// </summary>
    /// <param name="input">要显示错误信息的输入控件</param>
    /// <returns>显示错误信息的容器</returns>
    protected virtual IHtmlElement FailedMessageContainer( IHtmlInputControl input )
    {
      return null;
    }

    /// <summary>
    /// 派生类重写此方法查找显示验证失败摘要信息的容器
    /// </summary>
    /// <returns>显示验证失败摘要信息的容器</returns>
    protected virtual IHtmlElement FailedSummaryContainer()
    {
      return null;
    }

    /// <summary>
    /// 派生类重写此方法查找显示指定输入控件验证描述信息的容器
    /// </summary>
    /// <param name="input">要显示描述信息的输入控件</param>
    /// <returns>显示描述信息的容器</returns>
    protected virtual IHtmlElement FieldDescrptionContainer( IHtmlInputControl input )
    {
      return null;
    }



    /// <summary>
    /// 派生类重写此方法自定义验证失败信息的显示逻辑
    /// </summary>
    protected virtual void ShowFailedMessage()
    {
      var failedValidators = _validators.Where( v => !v.IsValid );

      foreach ( var v in failedValidators )
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

        list = EnsureList( summaryContainer );

        foreach ( var v in failedValidators )
        {
          var messages = v.FailedMessage();

          var item = list.AddElement( "li" );

          RenderMessages( item, messages );
        }
      }
    }


    /// <summary>
    /// 派生类重写此方法在指定容器呈现指定信息
    /// </summary>
    /// <param name="container"></param>
    /// <param name="messages"></param>
    protected virtual void RenderMessages( IHtmlElement container, string[] messages )
    {

      if ( messages.Length == 0 )
        return;

      else if ( messages.Length == 1 )
        container.InnerText( messages[0] );

      else
      {
        var list = EnsureList( container );
        foreach ( var m in messages )
        {
          var item = list.AddElement( "li" );
          item.InnerText( m );
        }
      }

      container.InnerText( string.Join( "\n", messages ) );
    }


    /// <summary>
    /// 派生类重写此方法自定义验证描述信息展现过程
    /// </summary>
    protected virtual void ShowFieldDescription()
    {
      foreach ( var field in _validators )
      {
        var container = FieldDescrptionContainer( field.InputControl );

        if ( container != null )
        {

          var rules = field.RuleDescription();

          if ( rules == null )
            continue;

          var list = EnsureList( container );

          foreach ( var r in rules )
          {
            var item = list.AddElement( "li" );
            item.InnerText( r );
          }
        }

      }
    }



    /// <summary>
    /// 确保指定的 HTML 元素是一个列表元素，若不是则在末尾加上一个列表
    /// </summary>
    /// <param name="container"></param>
    /// <returns></returns>
    protected IHtmlElement EnsureList( IHtmlElement container )
    {
      if ( HtmlSpecification.listElements.Contains( container.Name.ToLowerInvariant() ) )
        return container;


      return container.AddElement( "ul" );
    }



    /// <summary>
    /// 当验证失败时，引发Failed事件
    /// </summary>
    protected virtual void OnFailed()
    {
      ShowFailedMessage();


      if ( Failed != null )
        Failed( this, EventArgs.Empty );
    }

    public event EventHandler Failed;




    /// <summary>
    /// 当验证成功时，引发Successful事件
    /// </summary>
    protected virtual void OnSuccessful()
    {
      if ( Successful != null )
        Successful( this, EventArgs.Empty );
    }

    public event EventHandler Successful;



    /// <summary>
    /// 尝试获取字段名
    /// </summary>
    /// <param name="input">输入控件</param>
    /// <returns></returns>
    protected virtual string GetFieldName( IHtmlInputControl input )
    {

      if ( input == null )
        throw new ArgumentNullException( "input" );

      var labels = input.Labels();

      if ( labels.Length == 1 )
        return labels[0].Text;
      else
        return input.Name;

    }



    /// <summary>
    /// 添加一个字段验证器
    /// </summary>
    /// <param name="validator"></param>
    protected void AddFieldValidator( IHtmlFieldValidator validator )
    {
      _validators.Add( validator );
    }


    /// <summary>
    /// 添加一个字段验证规则
    /// </summary>
    /// <param name="inputName">输入控件名</param>
    /// <param name="validators">值验证器</param>
    protected void AddFieldValidation( string inputName, params IHtmlValueValidator[] validators )
    {
      AddFieldValidation( inputName, null, validators );
    }


    /// <summary>
    /// 添加一个字段验证规则
    /// </summary>
    /// <param name="inputName">输入控件名</param>
    /// <param name="fieldName">字段名</param>
    /// <param name="validators">值验证器</param>
    protected void AddFieldValidation( string inputName, string fieldName, params IHtmlValueValidator[] validators )
    {

      if ( inputName == null )
        throw new ArgumentNullException( "inputName" );

      var input = Form[inputName];

      if ( input == null )
        throw new ArgumentException( string.Format( CultureInfo.InvariantCulture, "未能找到 \"name\" 属性为 \"{0}\" 的输入控件", inputName ), "inputName" );

      AddFieldValidation( input, fieldName, validators );
    }

    /// <summary>
    /// 添加一个字段验证规则
    /// </summary>
    /// <param name="input">输入控件</param>
    /// <param name="fieldName">字段名</param>
    /// <param name="validators">值验证器</param>
    protected void AddFieldValidation( IHtmlInputControl input, string fieldName, params IHtmlValueValidator[] validators )
    {
      if ( input == null )
        throw new ArgumentNullException( "input" );


      if ( !input.Form.Equals( Form ) )
        throw new ArgumentException( "输入控件不属于验证表单", "input" );


      if ( fieldName == null )
        fieldName = GetFieldName( input );

      var fieldValidator = new GenericFieldValidator( input, fieldName, validators );
      AddFieldValidator( fieldValidator );
    }


    /// <summary>
    /// 一般字段验证器
    /// </summary>
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
