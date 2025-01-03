using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using zms9110750.TreeCollection.Extension;

namespace zms9110750.TreeCollection.Abstract;
public abstract class TreeNodeBase<T>(T? initValue) : IList<TreeNodeBase<T>>, IEquatable<T>
{
	#region Add
	/// <summary>
	/// 在指定索引位置添加子节点。
	/// </summary>
	/// <param name="index">要添加子节点的索引位置。</param>
	/// <param name="child">要添加的子节点。</param>
	/// <returns>添加的子节点。</returns>
	public virtual TreeNodeBase<T> AddAt(Index index, [NotNull] TreeNodeBase<T> node)
	{
		var offset = ThrowIfArgumentOutOfRange(index, true);
		ValidateElement(node);
		if (offset == Count)
		{
			Children.Add(node);
		}
		else
		{
			Children.Insert(offset, node);
		}
		node.Parent = this;
		UpdateIndex(index..);
		return node;
	}
	/// <summary>
	/// 添加子节点，并递归构建子树。
	/// </summary>
	/// <param name="index">添加位置</param>
	/// <param name="value">值</param>
	/// <param name="childrenFactory">创建子值方法</param>
	/// <returns>直接子节点</returns>
	public abstract TreeNodeBase<T> AddAt(Index index, T value, Func<T, IEnumerable<T>> childrenFactory);
	#endregion
	#region Replace
	/// <summary>
	/// 替换某个节点
	/// </summary>
	/// <param name="index">索引</param>
	/// <param name="node">新节点</param>
	protected virtual void ReplaceNode(Index index, TreeNodeBase<T> node)
	{
		var offset = ThrowIfArgumentOutOfRange(index, true);
		ValidateElement(node);
		this[offset].Parent = null;
		this[offset].Index = 0;
		this[offset].ResertLevelAndRoot();
		Children[offset] = node;
		node.Parent = this;
		node.Index = offset;
	}
	/// <summary>
	/// 替换子节点的值
	/// </summary>
	/// <param name="node">要替换值的节点</param>
	/// <param name="newValue">新值</param>
	protected abstract void ReplaceNodeValue(TreeNodeBase<T> node, T? newValue);
	#endregion
	#region Children
	protected List<TreeNodeBase<T>> Children { get; } = [];
	public virtual TreeNodeBase<T> this[int index]
	{
		get => Children[index]; set
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
				AddAt(^1, value); // 在列表末尾添加新值   
			}
			// 如果新值不是当前节点的子节点，则替换旧节点  
			else
			{
				// 断开旧节点的连接，并连接到新节点  
				ReplaceNode(index, value);
			}
		}
	}
	/// <summary>
	/// 裁切指定范围的跨度
	/// </summary>
	/// <param name="start">开始位置</param>
	/// <param name="length">数量</param>
	/// <returns>跨度</returns>
	public Span<TreeNodeBase<T>> Slice(int start, int length) => Children.AsSpan().Slice(start, length);
	public void CopyTo(TreeNodeBase<T>[] array, int arrayIndex) => Children.CopyTo(array, arrayIndex);
	public IEnumerator<TreeNodeBase<T>> GetEnumerator() => Children.GetEnumerator();
	#endregion
	#region Contains
	/// <summary>
	/// 验证添加进来的元素是否合法
	/// </summary>
	/// <param name="element">要添加的元素</param>
	/// <exception cref="ArgumentException">当元素不合法时抛出异常</exception>
	protected virtual void ValidateElement([NotNull] TreeNodeBase<T> node)
	{
		ArgumentNullException.ThrowIfNull(node);
		if ((node & this) == this)
		{
			throw new ArgumentException("不能把自己的祖先节点设置为自己的子节点", nameof(node));
		}
	}
	public int IndexOf(TreeNodeBase<T> node)
	{
		return Contains(node) ? node.Index : -1;
	}
	public int IndexOf(T? value, Range? range = null)
	{
		if (!PreCheckExistence(value))
		{
			return -1;
		}
		foreach (var item in this[range ?? ..])
		{
			if (item.Value?.Equals(value) ?? false)
			{
				return item.Index;
			}
		}
		return -1;
	}
	/// <summary>
	/// 先验检查某个值是否存在于集合中
	/// </summary>
	/// <param name="value">需要检查的值</param>
	/// <returns>如果确定不存在返回 false；否则返回 true</returns>
	protected abstract bool PreCheckExistence(T? value);

	public bool Contains([NotNullWhen(true)] TreeNodeBase<T> node)
	{
		return node?.Parent == this;
	}
	/// <summary>
	/// 检索是否存在包含指定值的节点。
	/// </summary>
	/// <param name="value">要检索的值。</param>
	/// <param name="range">要检索的范围。默认值为null，表示在整个子树中检索。</param>
	/// <returns>如果存在包含指定值的节点，则返回true；否则返回false。</returns>
	public bool Contains(T? value, Range? range = null)
	{
		if (!PreCheckExistence(value))
		{
			return false;
		}
		else if ((..).Equals(range ?? ..))
		{
			return true;
		}
		foreach (var item in this[range ?? ..])
		{
			if (item.Value?.Equals(value) ?? false)
			{
				return true;
			}
		}
		return false;
	}
	/// <summary>
	/// 验证索引是否合法
	/// </summary>
	/// <param name="index"></param>
	/// <returns></returns>
	protected int ThrowIfArgumentOutOfRange(Index index, bool add = false)
	{
		var offset = index.GetOffset(Count);
		ArgumentOutOfRangeException.ThrowIfGreaterThan(offset, Count);
		ArgumentOutOfRangeException.ThrowIfNegativeOrZero(offset);
		if (add)
		{
			ArgumentOutOfRangeException.ThrowIfEqual(offset, Count);
		}
		return offset;
	}
	#endregion
	#region Index
	public int Index { get; protected set; }
	public TreeNodeBase<T>? PreviousSibling => RequiredParent[Index - 1];
	public TreeNodeBase<T>? NextSibling => RequiredParent[Index + 1];
	/// <summary>
	/// 获取第一个子节点。
	/// </summary>
	public TreeNodeBase<T>? FirstChild => Count > 0 ? this[0] : null;
	/// <summary>
	/// 获取最后一个子节点。
	/// </summary>
	public TreeNodeBase<T>? LastChild => Count > 0 ? this[^1] : null;
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
		Children.AsSpan().AdjustPositions(oldOffset, newOffset);
		UpdateIndex(oldIndex, newIndex);
	}
	#endregion
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
	#region Remove
	/// <summary>
	/// 移除指定索引位置的子节点。
	/// </summary>
	/// <param name="index">要移除子节点的索引位置。</param>
	/// <returns>移除的子节点。</returns>
	public virtual TreeNodeBase<T>? Remove(TreeNodeBase<T> node)
	{
		if (Contains(node))
		{
			Children.RemoveAt(node.Index);
			UpdateIndex(node.Index..);
			node.Parent = null;
			return node;
		}
		return null;
	}
	public TreeNodeBase<T>? Remove(T? value, Range? range = null)
	{
		if (!PreCheckExistence(value))
		{
			return null;
		}
		foreach (var item in this[range ?? ..])
		{
			if (item.Value?.Equals(value) ?? false)
			{
				return Remove(item);
			}
		}
		return null;
	}
	public int RemoveAll(Predicate<TreeNodeBase<T>>? predicate = null, Range? range = null)
	{
		int count = 0;
		var (off, len) = (range ?? ..).GetOffsetAndLength(Count);
		for (int i = off + len; i >= off; i--)
		{
			if (predicate?.Invoke(this[i]) ?? true)
			{
				RemoveAt(i);
				count++;
			}
		}
		return count;
	}
	public TreeNodeBase<T>? RemoveAt(Index index)
	{
		ThrowIfArgumentOutOfRange(index);
		return Remove(this[index]);
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
	#region Value
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
	#endregion
	#region ICollection
	public int Count => Children.Count;
	bool ICollection<TreeNodeBase<T>>.IsReadOnly => false;
	void ICollection<TreeNodeBase<T>>.Add(TreeNodeBase<T> item) => AddAt(^1, item);
	void ICollection<TreeNodeBase<T>>.Clear() => RemoveAll();
	bool ICollection<TreeNodeBase<T>>.Remove(TreeNodeBase<T> item) => Remove(item) != null;
	void IList<TreeNodeBase<T>>.Insert(int index, TreeNodeBase<T> item) => AddAt(index, item);
	void IList<TreeNodeBase<T>>.RemoveAt(int index) => RemoveAt(index);
	public IEnumerable<TreeNodeBase<T>> EnumTree()
	{
		var parent = Parent;
		var index = Index;
		yield return this;
		foreach (var item in FirstChild ?? Enumerable.Empty<TreeNodeBase<T>>())
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
			foreach (var item in parent[index].EnumTree() ?? Enumerable.Empty<TreeNodeBase<T>>())
			{
				yield return item;
			}
		}
	}
	IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
	/// <summary>
	/// 获取两个节点的最近公共节点
	/// </summary>
	/// <param name="left">参数1</param>
	/// <param name="right">参数2</param>
	/// <returns>公共节点</returns>
	public static TreeNodeBase<T>? operator &(TreeNodeBase<T>? left, TreeNodeBase<T>? right)
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
	public static IEnumerable<TreeNodeBase<T>> operator |(TreeNodeBase<T>? left, TreeNodeBase<T>? right)
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
	#endregion
}

