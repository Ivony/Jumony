using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ivony.Html.Binding
{



  /// <summary>
  /// 为 HTML 绑定工作提供默认的元素绑定器，以及辅助创建数据绑定上下文和进行数据绑定
  /// </summary>
  public static class HtmlBinding
  {


    /// <summary>
    /// 获取样式绑定器
    /// </summary>
    public static StyleBinder StyleBinder { get; private set; }

    /// <summary>
    /// 获取文本绑定器
    /// </summary>
    public static LiteralBinder LiteralBinder { get; private set; }

    /// <summary>
    /// 获取脚本绑定器
    /// </summary>
    public static ScriptBinder ScriptBinder { get; private set; }

    /// <summary>
    /// 获取默认的绑定表达式绑定器
    /// </summary>
    public static EvalExpressionBinder EvalExpressionBinder { get; private set; }

    /// <summary>
    /// 获取默认的列表绑定表达式绑定器
    /// </summary>
    public static EvalListExpressionBinder EvalListExpressionBinder { get; private set; }



    static HtmlBinding()
    {

      Providers = new HtmlBindingContextProviderCollection();
      ExpressionBinders = new ExpressionBinderCollection();
      HtmlBinders = new List<IHtmlBinder>();

      Providers.Add( new HtmlListBindingContextProvider() );




      StyleBinder = new StyleBinder();
      ScriptBinder = new ScriptBinder();
      LiteralBinder = new LiteralBinder();



      EvalExpressionBinder = new EvalExpressionBinder();
      EvalListExpressionBinder = new EvalListExpressionBinder();




      HtmlBinders.Add( StyleBinder );
      HtmlBinders.Add( ScriptBinder );
      HtmlBinders.Add( LiteralBinder );

      ExpressionBinders.Add( EvalExpressionBinder );
      ExpressionBinders.Add( EvalListExpressionBinder );
    }



    /// <summary>
    /// 获取或注册表达式绑定器
    /// </summary>
    public static ICollection<IExpressionBinder> ExpressionBinders
    {
      get;
      private set;
    }


    /// <summary>
    /// 获取或注册元素绑定器
    /// </summary>
    public static ICollection<IHtmlBinder> HtmlBinders
    {
      get;
      private set;
    }



    internal static HtmlBindingContextProviderCollection Providers
    {
      get;
      private set;
    }




    /// <summary>
    /// 注册一个绑定上下文提供程序
    /// </summary>
    /// <param name="provider">要注册的绑定上下文提供程序</param>
    public static void RegisterBindingContextProvider( IHtmlBindingContextProvider provider )
    {
      lock ( Providers.SyncRoot )
      {
        if ( Providers.Contains( provider.ModelType ) )
          throw new InvalidOperationException();

        Providers.Add( provider );
      }
    }


    /// <summary>
    /// 解除绑定上下文提供程序注册
    /// </summary>
    /// <param name="modelType">模型类型</param>
    public static void UnregisterBindingContextProvider( Type modelType )
    {
      lock ( Providers.SyncRoot )
      {
        if ( Providers.Contains( modelType ) )
          Providers.Remove( modelType );
      }
    }



    /// <summary>
    /// 使用默认的绑定器设置创建 HtmlBindingContext 实例
    /// </summary>
    /// <param name="scope">要进行数据绑定的范畴</param>
    /// <param name="dataModel">数据模型</param>
    public static HtmlBindingContext Create( IHtmlContainer scope, object dataModel )
    {
      return HtmlBindingContext.Create( HtmlBinders.ToArray(), ExpressionBinders.ToArray(), scope, dataModel );
    }


    /// <summary>
    /// 创建绑定上下文
    /// </summary>
    /// <param name="scope">要进行数据绑定的范围</param>
    /// <param name="dataModel">数据模型</param>
    /// <param name="binders">在这一次数据绑定中要用到的绑定器列表</param>
    /// <returns>绑定上下文</returns>
    public static HtmlBindingContext Create( IHtmlContainer scope, object dataModel, params object[] binders )
    {
      var elementBinders = new List<IHtmlBinder>();
      var expressionBinders = new List<IExpressionBinder>();


      foreach ( var item in binders )
      {
        {
          var binder = item as IHtmlBinder;
          if ( binder != null )
          {
            elementBinders.Add( binder );
            continue;
          }
        }

        {
          var binder = item as IExpressionBinder;
          if ( binder != null )
          {
            expressionBinders.Add( binder );
            continue;
          }
        }

        {
          var list = item as IEnumerable<IHtmlBinder>;
          if ( list != null )
          {
            elementBinders.AddRange( list );
            continue;
          }
        }

        {
          var list = item as IEnumerable<IExpressionBinder>;
          if ( list != null )
          {
            expressionBinders.AddRange( list );
            continue;
          }
        }


        throw new ArgumentException( string.Format( "不支持 \"{0}\" 类型的绑定器", item.GetType().ToString() ), "binders" );
      }


      var context = HtmlBindingContext.Create( elementBinders.ToArray(), expressionBinders.ToArray(), scope, dataModel );
      return context;
    }



    /// <summary>
    /// 使用默认的绑定器设置进行数据绑定
    /// </summary>
    /// <param name="scope">要进行数据绑定的范畴</param>
    /// <param name="dataModel">数据模型</param>
    public static void DataBind( this IHtmlContainer scope, object dataModel )
    {
      Create( scope, dataModel ).DataBind();
    }


    /// <summary>
    /// 使用指定的绑定器设置进行数据绑定
    /// </summary>
    /// <param name="scope">要进行数据绑定的范畴</param>
    /// <param name="dataModel">数据模型</param>
    /// <param name="binders">在这一次数据绑定中要用到的绑定器列表</param>
    public static void DataBind( this IHtmlContainer scope, object dataModel, params object[] binders )
    {

      Create( scope, dataModel, binders ).DataBind();

    }

  }
}
