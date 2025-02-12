using System.Diagnostics.CodeAnalysis;

namespace zms9110750.TreeCollection.Abstract;

public abstract partial class TreeNodeBase<T>
{
	#region Level
	[field: AllowNull]
	[AllowNull]
	public TreeNodeBase<T> Root
	{
		get => field ?? Parent ?? this;
		private set;
	}
	public int Level
	{
		get
		{
			if (field < 0)
			{
				field = Parent?.Level is int i and >= 0 ? i + 1 : 0;
			}
			return field;
		}
		private set;
	}
	public TreeNodeBase<T>? Parent
	{
		get;
		protected set
		{
			if (value == field)
			{
				return;
			}
			field = value;
			ResertLevelAndRoot();
		}
	}
	/// <summary>
	/// 父节点
	/// </summary>
	/// <remarks>如果父节点为null会抛出异常</remarks>
	/// <exception cref="InvalidOperationException"></exception>
	public TreeNodeBase<T> RequiredParent => Parent ?? throw new InvalidOperationException("根节点不能执行需要父节点的操作");
	/// <summary>
	/// 递归地将自己的所有子节点的层级和根节点设置为非法。
	/// </summary>
	protected void ResertLevelAndRoot()
	{
		foreach (var child in this[..])
		{
			child.ResertLevelAndRoot();
		}
		Level = -1;
		Root = null;
	}
	public IEnumerable<TreeNodeBase<T>> IterateAncestors()
	{
		var current = Parent;
		while (current != null)
		{
			yield return current;
			current = current.Parent;
		}
	}
	#endregion
}