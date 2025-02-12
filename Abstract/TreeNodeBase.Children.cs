using zms9110750.TreeCollection.Extension;

namespace zms9110750.TreeCollection.Abstract;

public abstract partial class TreeNodeBase<T>
{
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
	public int Count => Children.Count;

	#endregion
}
