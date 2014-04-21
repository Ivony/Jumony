using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ivony.Html.Binding
{

  /// <summary>
  /// 定义绑定模式
  /// </summary>
  internal enum ListBindingMode
  {
    /// <summary>静态内容，无论数据项的多寡，绑定时仅绑定数据，而不对目标元素进行增删。</summary>
    StaticContent,

    /// <summary>动态内容，根据数据项的多寡，对目标元素进行增删以匹配数据项</summary>
    DynamicContent,

    /// <summary>动态内容，但是禁止增长，根据数据项的多寡，对目标元素进行删除以匹配数据项，但不增加目标元素</summary>
    DisableGrowth
  }
}
