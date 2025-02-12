namespace zms9110750.TreeCollection.Abstract;
public abstract partial class TreeNodeBase<T>
{
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
}