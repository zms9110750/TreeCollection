namespace zms9110750.TreeCollection.Abstract;

public abstract partial class TreeNodeBase<T>
{
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
}
