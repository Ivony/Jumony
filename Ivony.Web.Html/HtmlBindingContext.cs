using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Collections;
using System.ComponentModel;
using System.Web;

namespace Ivony.Web.Html
{


  /// <summary>
  /// 保存数据绑定操作的上下文
  /// </summary>
  public class HtmlBindingContext : IDisposable
  {

    private HtmlBindingActionCollection _actionList = new HtmlBindingActionCollection();


    /// <summary>
    /// 保存的所有绑定操作
    /// </summary>
    public IEnumerable<IHtmlBindingAction> Actions
    {
      get { return _actionList.GetActions(); }
    }


    /// <summary>
    /// 添加一个绑定操作
    /// </summary>
    /// <typeparam name="T">绑定对象类型</typeparam>
    /// <param name="target">绑定对象</param>
    /// <param name="action">绑定操作方法</param>
    public void Action<T>( T target, Action<T> action ) where T : IHtmlElement
    {
      _actionList.Add( new HtmlBindingAction<T>()
      {
        Target = target,
        Action = action
      } );
    }

    /// <summary>
    /// 添加一个绑定操作
    /// </summary>
    /// <typeparam name="T">绑定对象类型</typeparam>
    /// <param name="action">一个 BindAction 类型的对象，他保存了执行绑定操作所需的数据</param>
    public void Action<T>( HtmlBindingAction<T> action ) where T : IHtmlElement
    {
      _actionList.Add( action );
    }



    private Hashtable _dataContexts = new Hashtable();

    /// <summary>
    /// 为一个容器对象设置数据上下文
    /// </summary>
    /// <param name="container">要设置数据上下文的容器对象</param>
    /// <param name="dataContext">数据上下文</param>
    internal void SetDataContext( IHtmlContainer container, object dataContext )
    {
      _dataContexts[container] = dataContext;
    }

    /// <summary>
    /// 获取指定节点或元素最近的数据上下文
    /// </summary>
    /// <returns>最近的数据上下文，将递归查找父级和父级上下文，如果都没找到则返回null</returns>
    internal object GetDataContext( IHtmlNode node )
    {

      object data = GetDataContextCore( node );
      if ( data != null )
        return data;

      IHtmlContainer container;
      while ( (container = node.Parent) != null )
      {
        data = GetDataContextCore( container );
        if ( data != null )
          return data;

        node = container;
      }

      return null;
    }


    /// <summary>
    /// 查找指定节点的数据上下文，将递归向父级BindingContext查询
    /// </summary>
    /// <param name="node">指定节点</param>
    /// <returns>查找到的数据上下文，如果没有找到则返回null</returns>
    private object GetDataContextCore( IHtmlNode node )
    {
      object data = _dataContexts[node];

      if ( data == null && ParentContext != null )
        return ParentContext.GetDataContextCore( node );

      return data;
    }



    /// <summary>
    /// 执行数据绑定操作，此操作将导致递归提交
    /// </summary>
    public void Commit()
    {
      var commitContexts = new List<HtmlBindingContext>();

      var context = Current;

      while ( context != this )//递归找出所有之下的上下文
      {
        commitContexts.Add( context );
        context = context.ParentContext;

        if ( context == null )
          throw new InvalidOperationException();
      }


      foreach ( var c in commitContexts )
        c.CommitCore();

      CommitCore();
    }


    private void CommitCore()
    {
      HttpContext.Current.Trace.Write( "Binding", string.Format( "Begin Commit BindingContext \"{0}\"", Name ) );

      foreach ( var element in PostOrderTraverse( Scope ) )
      {
        var actions = _actionList.GetActions( element );

        foreach ( var a in actions )
          a.Bind();
      }

      _actionList.Clear();

      HttpContext.Current.Trace.Write( "Binding", string.Format( "End Commit BindingContext \"{0}\"", Name ) );
    }

    /// <summary>
    /// 后序遍历所有元素用于绑定
    /// </summary>
    /// <param name="container">根元素</param>
    /// <returns></returns>
    private static IEnumerable<IHtmlElement> PostOrderTraverse( IHtmlContainer container )
    {
      var elementList = new List<IHtmlElement>();

      PostOrderTraverse( container, elementList );
      return elementList;
    }


    /// <summary>
    /// 后序遍历所有元素，并将找到的元素放入集合中
    /// </summary>
    /// <param name="container">根元素</param>
    /// <param name="elementList">存放元素的集合</param>
    /// <returns></returns>
    private static void PostOrderTraverse( IHtmlContainer container, IList<IHtmlElement> elementList )
    {
      foreach ( var node in container.Nodes().OfType<IHtmlContainer>() )
      {
        PostOrderTraverse( node, elementList );

        IHtmlElement element = node as IHtmlElement;
        if ( element != null )
          elementList.Add( element );
      }
    }



    /// <summary>
    /// 绑定的范围
    /// </summary>
    public IHtmlContainer Scope
    {
      get;
      private set;
    }


    /// <summary>
    /// 绑定上下文的友好名称
    /// </summary>
    public string Name
    {
      get;
      private set;
    }

    private HtmlBindingContext( IHtmlContainer scope, string name )
    {
      Scope = scope;
      Name = name;
    }

    /// <summary>
    /// 创建并进入一个绑定上下文
    /// </summary>
    /// <param name="scope">绑定范围，超出此范围的绑定都不会被提交</param>
    /// <param name="name">上下文友好的名称，用于Trace识别</param>
    /// <returns></returns>
    public static HtmlBindingContext EnterContext( IHtmlContainer scope, string name )
    {
      var context = new HtmlBindingContext( scope, name );
      context.Enter();
      return context;
    }

    /// <summary>
    /// 获取当前的绑定上下文
    /// </summary>
    public static HtmlBindingContext Current
    {
      get
      {
        return System.Runtime.Remoting.Messaging.CallContext.GetData( contextName ) as HtmlBindingContext;
      }
      private set
      {
        System.Runtime.Remoting.Messaging.CallContext.SetData( contextName, value );
      }
    }



    /// <summary>
    /// 父级绑定上下文
    /// </summary>
    public HtmlBindingContext ParentContext
    {
      get;
      private set;
    }


    private const string contextName = "HtmlBindingContexts";


    private void Enter()
    {
      ParentContext = Current;
      System.Runtime.Remoting.Messaging.CallContext.SetData( contextName, this );

      HttpContext.Current.Trace.Write( "Binding", string.Format( "Enter BindingContext \"{0}\"", Name ) );
    }


    /// <summary>
    /// 放弃所有的绑定
    /// </summary>
    public void Discard()
    {
      _actionList.Clear();
    }

    /// <summary>
    /// 退出绑定上下文，同时也会递归退出其上所有没有退出的绑定上下文
    /// </summary>
    public void Exit()
    {
      Exit( false );
    }

    /// <summary>
    /// 退出上下文
    /// </summary>
    /// <param name="discard">是否放弃上下文中存在的绑定操作</param>
    public void Exit( bool discard )
    {
      Exit( discard, this );

    }

    /// <summary>
    /// 退出上下文
    /// </summary>
    /// <param name="discard">是否放弃上下文中存在的绑定操作</param>
    /// <param name="target">递归退出的目标</param>
    private static void Exit( bool discard, HtmlBindingContext target )
    {

      var context = Current;

      if ( discard )
        context.Discard();
      else
        context.Commit();

      Current = context.ParentContext;

      HttpContext.Current.Trace.Write( "Binding", string.Format( "Exit BindingContext \"{0}\"", context.Name ) );

      if ( context ==  target )
        return;

      Exit( discard, target );
    }

    /// <summary>
    /// 退出当前上下文
    /// </summary>
    public static void ExitContext()
    {
      Current.Exit();
    }


    #region IDisposable 成员

    public void Dispose()
    {
      Exit( true );
    }

    #endregion




    private class HtmlBindingActionCollection
    {

      private Dictionary<object, List<IHtmlBindingAction>> _targetSet = new Dictionary<object, List<IHtmlBindingAction>>();

      public void Add( IHtmlBindingAction action )
      {
        if ( _targetSet.ContainsKey( action.Target ) )
          _targetSet[action.Target].Add( action );
        else
          _targetSet.Add( action.Target, new List<IHtmlBindingAction>() { action } );
      }


      public bool Contains( object target )
      {
        return _targetSet.ContainsKey( target );
      }


      public IEnumerable<IHtmlBindingAction> GetActions( object target )
      {
        List<IHtmlBindingAction> actions;
        if ( _targetSet.TryGetValue( target, out actions ) )
          return actions;
        else
          return Enumerable.Empty<IHtmlBindingAction>();
      }


      public IEnumerable<IHtmlBindingAction> GetActions()
      {
        return _targetSet.Values.SelectMany( value => value );
      }

      public void Clear()
      {
        _targetSet.Clear();
      }

      internal void Remove( IHtmlBindingAction a )
      {
        throw new NotImplementedException();
      }
    }

  }



  public interface IHtmlBindingAction
  {
    object Target { get; }

    void Bind();
  }

  public class HtmlBindingAction<T> : IHtmlBindingAction where T : IHtmlElement
  {

    public HtmlBindingAction()
    {
      DataBound = false;
    }

    public T Target
    {
      get;
      internal set;
    }

    public Action<T> Action
    {
      get;
      internal set;
    }

    object IHtmlBindingAction.Target { get { return Target; } }

    void IHtmlBindingAction.Bind() { Action( Target ); }

    public bool DataBound
    {
      get;
      internal set;
    }
  }
}
