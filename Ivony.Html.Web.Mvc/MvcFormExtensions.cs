using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Ivony.Fluent;
using Ivony.Html.Forms;

namespace Ivony.Html.Web
{

  /// <summary>
  /// 有关 MVC 的一些表单方法扩展
  /// </summary>
  public static class MvcFormExtensions
  {

    /// <summary>
    /// 在表单上应用指定模型的值
    /// </summary>
    /// <param name="form">要应用的表单</param>
    /// <param name="dataModel">要应用的模型值</param>
    /// <returns>返回表单，便于链式调用</returns>
    public static HtmlForm ApplyValues( this HtmlForm form, object dataModel )
    {

      var data = dataModel.ToPropertiesMap();

      foreach ( var key in form.InputControls.Select( c => c.Name ) )
      {

        if ( data.ContainsKey( key ) )
          form.Values[key] = data[key];

      }

      return form;

    }



    /// <summary>
    /// 在表单上应用指定值提供程序的值
    /// </summary>
    /// <param name="form">要应用的表单</param>
    /// <param name="valueProvider">提供值的 ValueProvider 实例</param>
    /// <returns>返回表单，便于链式调用</returns>
    public static HtmlForm ApplyValues( this HtmlForm form, IValueProvider valueProvider )
    {

      foreach ( var key in form.InputControls.Select( c => c.Name ) )
      {
        if ( valueProvider.ContainsPrefix( key ) )
        {
          form.Values[key] = valueProvider.GetValue( key ).AttemptedValue;
        }
      }

      return form;

    }


    /// <summary>
    /// 显示表单验证失败信息，如果有的话
    /// </summary>
    /// <param name="form">要显示验证失败信息的表单</param>
    /// <param name="modelStates">Model 验证信息</param>
    /// <param name="inputControlFinder">提示容器查找器，指定错误信息显示的位置</param>
    /// <returns>是否存在错误信息</returns>
    public static bool ShowErrorMessage( this HtmlForm form, ModelStateDictionary modelStates, Func<IHtmlInputControl, IHtmlElement> inputControlFinder )
    {
      return new GenericMvcFormValidator( form, modelStates )
      {
        InputControlFinder = inputControlFinder
      }.ShowErrorMessage();
    }

    private class GenericMvcFormValidator : MvcFormValidator
    {
      public GenericMvcFormValidator( HtmlForm form, ModelStateDictionary modelStates )
        : base( form, modelStates )
      {

      }


      public Func<IHtmlInputControl, IHtmlElement> InputControlFinder
      {
        get;
        set;
      }

      public Func<IHtmlElement, IHtmlElement> InputElementFinder
      {
        get;
        set;
      }

      public IHtmlElement SummaryContainer
      {
        get;
        set;
      }

      protected override IHtmlElement FailedMessageContainer( IHtmlInputControl input )
      {

        if ( InputControlFinder != null )
          return InputControlFinder( input );

        if ( InputElementFinder != null )
        {
          var element = GetInputElement( input );

          if ( element == null )
            return null;

          return InputElementFinder( element );
        }

        return base.FailedMessageContainer( input );
      }

      protected override IHtmlElement FailedSummaryContainer()
      {

        if ( SummaryContainer != null )
          return SummaryContainer;
        return base.FailedSummaryContainer();
      }

      private IHtmlElement GetInputElement( IHtmlInputControl input )
      {
        var inputText = input as HtmlInputText;
        if ( inputText != null )
          return inputText.Element;

        var select = input as HtmlSelect;
        if ( select != null )
          return select.Element;

        var textaera = input as HtmlTextArea;
        return textaera.Element;

      }

    }
  }
}
