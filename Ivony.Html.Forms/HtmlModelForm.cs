using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ivony.Html.Forms
{

  /// <summary>
  /// 代表一个强类型模型的表单
  /// </summary>
  /// <typeparam name="T">表单模型类型</typeparam>
  public class HtmlModelForm<T> : HtmlForm
  {


    public HtmlModelForm( T model, IHtmlElement element, FormConfiguration configuration = null, IFormProvider provider = null )
      : base( element, configuration, provider )
    {
      Model = model;

      ValidateModel();

    }

    private void ValidateModel()
    {
      foreach ( var name in Controls.ControlNames )
      {
        ValidateModel( name );
      }
    }

    private void ValidateModel( string name )
    {
      throw new NotImplementedException();
    }


    /// <summary>
    /// 表单映射的模型对象
    /// </summary>
    public T Model
    {
      get;
      private set;
    }

    /// <summary>
    /// 当模型对象的属性发生变更时调用此方法更新表单
    /// </summary>
    public void ModelChanged()
    {

    }

  }
}
