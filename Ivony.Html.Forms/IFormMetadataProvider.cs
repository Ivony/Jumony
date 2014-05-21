using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ivony.Html.Forms
{
  /// <summary>
  /// 定义表单字段元数据提供程序
  /// </summary>
  public interface IFormMetadataProvider
  {

    /// <summary>
    /// 获取指定字段元数据
    /// </summary>
    /// <param name="name">字段名</param>
    /// <returns>字段元数据</returns>
    FormFieldMetadata GetFieldMetadata( string name );
  }
}
