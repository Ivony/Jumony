using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ivony.Fluent
{
  public static class StringExtensions
  {
    /// <summary>
    /// 对字符串执行不区分大小写的比较
    /// </summary>
    /// <param name="str1"></param>
    /// <param name="str2"></param>
    /// <returns></returns>
    public static bool EqualsIgnoreCase( this string str1, string str2 )
    {
      return string.Equals( str1, str2, StringComparison.OrdinalIgnoreCase );
    }
  }
}
