using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ivony.Html.Forms
{



  /// <summary>
  /// 代表绑定在某个特定模型上的表单
  /// </summary>
  public class HtmlModelForm : HtmlForm
  {


    public HtmlModelForm( object model, IHtmlElement element, FormConfiguration configuration = null, IFormProvider provider = null )
      : base( element, configuration, provider )
    {

      Model = model;

      Initialize();
    }


    protected virtual void Initialize()
    {
      GetMetadata( Model.GetType() );
    }


    /// <summary>
    /// 表单绑定的模型对象
    /// </summary>
    public object Model
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


    protected FormMetadata GetMetadata( Type modelType )
    {
      return new FormMetadata( this, GetMetadataProvider( modelType ) );
    }

    protected virtual IFormMetadataProvider GetMetadataProvider( Type modelType )
    {
      throw new NotImplementedException();
    }


  }



  /// <summary>
  /// 代表一个强类型模型的表单
  /// </summary>
  /// <typeparam name="T">表单模型类型</typeparam>
  public class HtmlModelForm<T> : HtmlModelForm
  {


    public HtmlModelForm( T model, IHtmlElement element, FormConfiguration configuration = null, IFormProvider provider = null )
      : base( model, element, configuration, provider )
    {
      Model = model;
    }


    protected override void Initialize()
    {
      GetMetadata( typeof( T ) );
    }


    public new T Model
    {
      get;
      private set;
    }

  }
}
