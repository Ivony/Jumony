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
    /// <summary>自动，如果被绑定元素是列表容器，如ul、ul，则尝试绑定其子元素。</summary>
    Auto,

    /// <summary>复制，不论被绑定元素是否为列表容器，均复制列表数据项份数，针对每一份进行绑定</summary>
    Repeat

  }
}
