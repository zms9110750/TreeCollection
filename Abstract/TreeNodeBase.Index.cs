using zms9110750.TreeCollection.Extension;

namespace zms9110750.TreeCollection.Abstract;

public abstract partial class TreeNodeBase<T>
{
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
}
