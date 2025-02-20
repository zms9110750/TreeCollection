using System.Diagnostics.CodeAnalysis;
using System.Text;
using zms9110750.TreeCollection.Abstract;
using zms9110750.TreeCollection.Extension;
namespace zms9110750.TreeCollection;
public class TreeNode<T>(T initValue) : TreeList<TreeNode<T>>, IEquatable<T>
{
	#region override
	/// <summary>  
	/// 获取或替换目标索引的节点  
	/// </summary>  
	/// <param name="index">索引</param>  
	/// <returns>指定索引的节点</returns>  
	/// <value>替换原节点的新节点</value>  
	/// <remarks>  
	/// 可能发生以下情形  
	/// <list type="bullet">  
	/// <item>移除节点（value为null）</item>  
	/// <item>排列节点（value已经是自己的子节点）</item>  
	/// <item>添加节点（index为count且不是自己的子节点)</item>  
	/// <item>替换节点（不是自己的子节点)</item>  
	/// </list>  
	/// </remarks>  
	/// <exception cref="ArgumentException">目标节点是自己的祖先节点</exception>  
	public override TreeNode<T> this[int index]
	{
		get => InnerList[index];
		set
		{
			ThrowIfArgumentOutOfRange(index, true);
			if (value == this[index])
			{
				return;
			}
			// 如果新值为null，则从列表中移除当前节点  
			else if (value == null)
			{
				RemoveAt(index); // 移除指定索引的节点   
			}
			// 如果新值已经是当前节点的子节点，则更新其索引  
			else if (Contains(value))
			{
				MoveNode(value.Index, index);  // 更新新值的索引   
			}
			// 如果索引等于当前节点子节点的数量，并且新值不是当前节点的子节点，则将其添加到末尾  
			else if (index == Count)
			{
				AddAt(^0, value); // 在列表末尾添加新值   
			}
			// 如果新值不是当前节点的子节点，则替换旧节点  
			else
			{
				// 断开旧节点的连接，并连接到新节点  
				ReplaceNode(index, value);
			}
		}
	}
	#endregion
	#region Add
	public override void AddAt(Index index, IEnumerable<TreeNode<T>> item)
	{
		base.AddAt(index, item);
		foreach (var child in item)
		{
			child.Parent = this;
		}
		UpdateIndex(index, ^1);
	}
	public override void AddAt(Index index, params ReadOnlySpan<TreeNode<T>> item)
	{
		base.AddAt(index, item);
		foreach (var child in item)
		{
			child.Parent = this;
		}
		UpdateIndex(index, ^1);
	}
	public override TreeNode<T> AddAt(Index index, TreeNode<T> item)
	{
		base.AddAt(index, item);
		item.Parent = this;
		UpdateIndex(index, ^1);
		return item;
	}
	#endregion
	#region Remove
	public override TreeNode<T>? Remove(TreeNode<T> item, Range? range = null)
	{
		if (base.Remove(item, range) is TreeNode<T> node)
		{
			node.Parent = null;
			node.Index = 0;
			UpdateIndex(node.Index, ^1);
			return node;
		}
		return default;
	}
	public override List<TreeNode<T>> RemoveAll(Predicate<TreeNode<T>>? match = null, Range? range = null)
	{
		var list = base.RemoveAll(match, range);
		if (list.Count > 0)
		{
			foreach (var node in list)
			{
				node.Parent = null;
				node.Index = 0;
			}
			UpdateIndex(0, ^1);
		}
		return list;
	}
	public override TreeNode<T> RemoveAt(Index index)
	{
		var node = base.RemoveAt(index);
		node.Parent = null;
		node.Index = 0;
		UpdateIndex(node.Index, ^1);
		return node;
	}
	/// <summary>
	/// 移除具有指定值的节点
	/// </summary>
	/// <param name="item">查找值</param>
	/// <param name="range">查找范围</param>
	/// <returns>被移除的节点。若没找到，则返回null。</returns>
	public TreeNode<T>? Remove(T item, Range? range = null)
	{
		var index = IndexOf(item, range);
		return index >= 0 ? RemoveAt(index) : null;
	}
	#endregion
	#region Level
	/// <summary>
	/// 根节点
	/// </summary>
	[field: AllowNull]
	[AllowNull]
	public TreeNode<T> Root { get => field ?? Parent ?? this; private set; }
	/// <summary>
	/// 节点层级
	/// </summary>
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
	/// <summary>
	/// 父节点
	/// </summary>
	public TreeNode<T>? Parent
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
	public TreeNode<T> RequiredParent => Parent ?? throw new InvalidOperationException("根节点不能执行需要父节点的操作");
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
	#endregion
	#region Index
	/// <summary>
	/// 节点索引
	/// </summary>
	public int Index { get; protected set; }
	/// <summary>
	/// 前一个兄弟节点
	/// </summary>
	public TreeNode<T>? PreviousSibling => RequiredParent[Index - 1];
	/// <summary>
	/// 后一个兄弟节点
	/// </summary>
	public TreeNode<T>? NextSibling => RequiredParent[Index + 1];
	/// <summary>
	/// 获取第一个子节点。
	/// </summary>
	public TreeNode<T>? FirstChild => Count > 0 ? this[0] : null;
	/// <summary>
	/// 获取最后一个子节点。
	/// </summary>
	public TreeNode<T>? LastChild => Count > 0 ? this[^1] : null;
	/// <summary>
	/// 更新索引
	/// </summary>
	/// <param name="range">范围</param>
	protected void UpdateIndex(Range range)
	{
		var start = range.Start.GetOffset(Count);
		foreach (var item in this[range])
		{
			item.Index = start++;
		}
	}
	/// <summary>
	/// 更新索引
	/// </summary>
	private void UpdateIndex(Index index1, Index index2)
	{
		if (index1.GetOffset(Count) > index2.GetOffset(Count))
		{
			UpdateIndex(index2..index1);
		}
		else
		{
			UpdateIndex(index1..index2);
		}
	}
	/// <summary>
	/// 将子节点从旧索引位置移动到新索引位置。
	/// </summary>
	/// <param name="oldIndex">要移动的子节点的旧索引位置。</param>
	/// <param name="newIndex">要移动的子节点的新索引位置。</param>
	/// <returns>移动的子节点。</returns>
	public void MoveNode(Index oldIndex, Index newIndex)
	{
		int oldOffset = ThrowIfArgumentOutOfRange(oldIndex);
		var newOffset = ThrowIfArgumentOutOfRange(newIndex);
		if (oldOffset == newOffset)
		{
			return;
		}
		InnerList.AsSpan().AdjustPositions(oldOffset, newOffset);
		UpdateIndex(oldIndex, newIndex);
	}
	#endregion
	#region Validate
	/// <summary>
	/// 验证索引是否合法
	/// </summary>
	/// <param name="index"></param>
	/// <returns></returns>
	protected int ThrowIfArgumentOutOfRange(Index index, bool add = false)
	{
		var offset = index.GetOffset(Count);
#if NET8_0_OR_GREATER
		ArgumentOutOfRangeException.ThrowIfGreaterThan(offset, Count);
		ArgumentOutOfRangeException.ThrowIfNegativeOrZero(offset);
		if (add)
		{
			ArgumentOutOfRangeException.ThrowIfEqual(offset, Count);
		}
#else
		if (offset < 0 || offset > Count || (add && offset == Count))
		{
			throw new ArgumentOutOfRangeException(nameof(index));
		}
#endif
		return offset;
	}

	protected override void ValidateElement(TreeNode<T> item)
	{
		ArgumentNullException.ThrowIfNull(item);
		if ((item & this) == this)
		{
			throw new ArgumentException("不能把自己的祖先节点设置为自己的子节点", nameof(item));
		}
	}
	#endregion
	#region Contains 
	public override bool Contains(TreeNode<T> item, Range? range = null)
	{
		return item.Parent == this
			&& (range is not Range r ||
			r.GetOffsetAndLength(Count) is (int start, int length)
			&& start <= item.Index && item.Index < start + length
			);
	}

	public bool Contains(T item, Range? range = null)
	{
		return IndexOf(item, range) >= 0;
	}

	public override int IndexOf(TreeNode<T> item, Range? range = null)
	{
		return Contains(item, range) ? item.Index : -1;
	}
	public int IndexOf(T item, Range? range = null)
	{
		if (range is not Range r)
		{
			return InnerList.IndexOf(item);
		}

		var (off, len) = range?.GetOffsetAndLength(Count) ?? (0, Count);
		for (int i = off; i < off + len; i++)
		{
			if (InnerList[i].Equals(item))
			{
				return i;
			}
		}
		return -1;
	}
	#endregion
	#region Replace
	/// <summary>
	/// 替换某个节点
	/// </summary>
	/// <param name="index">索引</param>
	/// <param name="node">新节点</param>
	protected virtual void ReplaceNode(Index index, TreeNode<T> node)
	{
		var offset = ThrowIfArgumentOutOfRange(index, true);
		ValidateElement(node);
		this[offset].Parent = null;
		this[offset].Index = 0;
		this[offset].ResertLevelAndRoot();
		InnerList[offset] = node;
		node.Parent = this;
		node.Index = offset;
	}
	/// <summary>
	/// 替换子节点的值
	/// </summary>
	/// <param name="node">要替换值的节点</param>
	/// <param name="newValue">新值</param>
	protected void ReplaceNodeValue(TreeNode<T> node, T? newValue)
	{
		node.Value = newValue;
	}
	#endregion
	#region Equatable
	/// <summary>
	/// 获取或设置节点的值。
	/// </summary>
	public T? Value
	{
		get; set
		{
			if (Equals(value))
			{
				return;
			}
			field = value;
			Parent?.ReplaceNodeValue(this, value);
		}
	} = initValue;
	public bool Equals(T? other)
	{
		return EqualityComparer<T>.Default.Equals(Value, other);
	}
	public override bool Equals(object? obj)
	{
		return obj is T node && Equals(node) || obj == this;
	}
	public override int GetHashCode()
	{
		return HashCode.Combine(Value, InnerList, Parent, Index);
	}
	#endregion
	#region IEnumerable
	/// <summary>
	/// 遍历树
	/// </summary>
	/// <returns></returns>
	public IEnumerable<TreeNode<T>> EnumTree()
	{
		var parent = Parent;
		var index = Index;
		yield return this;
		foreach (var item in FirstChild ?? Enumerable.Empty<TreeNode<T>>())
		{
			if (parent != Parent)
			{
				break;
			}
			index = Index;
			yield return item;
		}
		if (parent != null && parent.Count > index)
		{
			foreach (var item in parent[index].EnumTree() ?? Enumerable.Empty<TreeNode<T>>())
			{
				yield return item;
			}
		}
	}
	/// <summary>
	/// 从自己到根节点的路径
	/// </summary>
	/// <returns></returns>
	public IEnumerable<TreeNode<T>> IterateAncestors()
	{
		var current = Parent;
		while (current != null)
		{
			yield return current;
			current = current.Parent;
		}
	}
	#endregion
	#region operator
	/// <summary>
	/// 获取两个节点的最近公共节点
	/// </summary>
	/// <param name="left">参数1</param>
	/// <param name="right">参数2</param>
	/// <returns>公共节点</returns>
	public static TreeNode<T>? operator &(TreeNode<T>? left, TreeNode<T>? right)
	{
		if (left == null || right == null || left.Root != right.Root)
		{
			return null;
		}
		while (left!.Level > right.Level)
		{
			left = left.Parent;
		}
		while (left.Level < right!.Level)
		{
			right = right.Parent;
		}
		while (left != right)
		{
			left = left!.Parent;
			right = right!.Parent;
		}
		return left;
	}
	/// <summary>
	/// 两个节点间的路径
	/// </summary>
	/// <param name="left">参数1</param>
	/// <param name="right">参数2</param>
	/// <returns>路径的迭代</returns>
	/// <remarks>不包含公共节点。包含不是公共节点的参数。</remarks>
	public static IEnumerable<TreeNode<T>> operator |(TreeNode<T>? left, TreeNode<T>? right)
	{
		if (left == right)
		{
			yield break;
		}
		var ancestor = left & right;
		if (ancestor == right)
		{
			while (left != right)
			{
				yield return left!;
				left = left!.Parent;
			}
		}
		else if (ancestor == left)
		{
			foreach (var item in (right | left).Reverse())
			{
				yield return item;
			}
		}
		else
		{
			foreach (var item in left | ancestor)
			{
				yield return item;
			}
			foreach (var item in (right | ancestor).Reverse())
			{
				yield return item;
			}
		}
	}

	public static implicit operator TreeNode<T>(T value)
	{
		return new TreeNode<T>(value);
	}
	#endregion
	#region ToString 
	public override string ToString()
	{
		StringBuilder sb = new StringBuilder();
		Stack<string> stack = new Stack<string>();
		stack.Push("");
		Append(sb, stack);
		return sb.ToString();
	}
	private void Append(StringBuilder sb, Stack<string> stack)
	{
		const string V0 = "└─ ";
		const string V1 = "├─ ";
		const string V2 = "   ";
		const string V3 = "│  ";
		sb.AppendJoin(null, stack.Reverse()).AppendLine(Value?.ToString());
		if (stack.Peek() == V0)
		{
			stack.Pop();
			stack.Push(V2);
		}
		else if (stack.Peek() == V1)
		{
			stack.Pop();
			stack.Push(V3);
		}
		foreach (var node in this[..])
		{
			stack.Push(node.NextSibling != null ? V1 : V0);
			node.Append(sb, stack);
		}
		stack.Pop();
	}
	#endregion
}