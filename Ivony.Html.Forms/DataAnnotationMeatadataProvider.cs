using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace Ivony.Html.Forms
{

  /// <summary>
  /// 用于提取 DataAnnotaion 元数据并包装成表单元数据的表单元数据提供程序
  /// </summary>
  public class DataAnnotationMeatadataProvider : IFormMetadataProvider
  {



    /// <summary>
    /// 构建 DataAnnotationMeatadataProvider 对象
    /// </summary>
    /// <param name="modelType">要提取元数据的数据模型类型</param>
    public DataAnnotationMeatadataProvider( Type modelType )
    {
      ModelType = modelType;
    }


    /// <summary>
    /// 获取要提取元数据的模型类型
    /// </summary>
    protected Type ModelType
    {
      get;
      private set;
    }


    private ICustomTypeDescriptor _typeDescriptor;

    /// <summary>
    /// 获取 DataAnnotation 的 TypeDescriptor 对象
    /// </summary>
    /// <returns></returns>
    protected virtual ICustomTypeDescriptor GetTypeDescriptor()
    {
      return new AssociatedMetadataTypeTypeDescriptionProvider( ModelType ).GetTypeDescriptor( ModelType );
    }


    /// <summary>
    /// 用于获取 DataAnnotation 元数据的 TypeDescriptor 对象
    /// </summary>
    protected ICustomTypeDescriptor TypeDescrptor
    {
      get
      {
        if ( _typeDescriptor == null )
          _typeDescriptor = GetTypeDescriptor();

        return _typeDescriptor;
      }
    }


    /// <summary>
    /// 获取字段的元数据
    /// </summary>
    /// <param name="name">字段名</param>
    /// <returns>字段元数据</returns>
    public FormFieldMetadata GetFieldMetadata( string name )
    {
      throw new NotImplementedException();
    }


    /// <summary>
    /// 从属性获取字段元数据
    /// </summary>
    /// <param name="property">属性信息</param>
    /// <returns>字段元数据</returns>
    protected FormFieldMetadata GetFieldMetadata( PropertyDescriptor property )
    {
      throw new NotImplementedException();
    }



  }
}
