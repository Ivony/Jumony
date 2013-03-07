using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ivony.Fluent
{
  /// <summary>
  /// 提供字符串相关的扩展方法
  /// </summary>
  public static class StringExtensions
  {
    /// <summary>
    /// 对字符串执行不区分大小写的比较
    /// </summary>
    /// <param name="str1">要比较的第一个字符串</param>
    /// <param name="str2">要比较的第二个字符串</param>
    /// <returns>两个字符串除了大小写是否存在其他区别</returns>
    public static bool EqualsIgnoreCase( this string str1, string str2 )
    {
      return string.Equals( str1, str2, StringComparison.OrdinalIgnoreCase );
    }
  }
}
