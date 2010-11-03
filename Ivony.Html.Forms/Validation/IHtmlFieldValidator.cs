using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ivony.Html.Forms.Validation
{
  public class HtmlFieldValidator
  {

    private IHtmlInputControl _input;
    private string _fieldName;
    private IHtmlElement _messageContainer;
    private List<IHtmlValueValidator> _validators = new List<IHtmlValueValidator>();



    public HtmlFieldValidator( IHtmlInputControl inputControl, params IHtmlValueValidator[] validators ) : this( inputControl, null, validators ) { }

    public HtmlFieldValidator( IHtmlInputControl inputControl, string fieldName, params IHtmlValueValidator[] validators ) : this( inputControl, fieldName, null, validators ) { }

    public HtmlFieldValidator( IHtmlInputControl inputControl, string fieldName, IHtmlElement messageContainer, params IHtmlValueValidator[] validators )
    {
      _input = inputControl;
      _fieldName = fieldName;
      _messageContainer = messageContainer;
      _validators.AddRange( validators );

      if ( _fieldName == null )
      {
        var inputElement =_input as IHtmlFocusableControl;
        _fieldName = inputElement.LabelText();

        if ( _fieldName == null )
          _fieldName = _input.Name;
      }
    }



    public IHtmlInputControl InputControl
    {
      get { return _input; }
    }

    public IList<IHtmlValueValidator> Validators
    {
      get { return _validators; }
    }


    public bool Validate()
    {
      var form = _input.Form;
      if ( form.SubmittedValues == null )
        throw new InvalidOperationException( "表单尚未提交，无法进行验证" );

      var value = _input.SubmittedValue();

      var faildValidator = _validators.FirstOrDefault( v => v.Validate( value ) == false );

      if ( faildValidator == null )
        return true;

      ShowFaildMessage( faildValidator );

      return false;
    }

    protected virtual void ShowFaildMessage( IHtmlValueValidator faildValidator )
    {
      var message = faildValidator.ErrorMessage.Replace( "<fieldname>", _fieldName );

      _messageContainer.InnerText( message );
    }
  }

}
