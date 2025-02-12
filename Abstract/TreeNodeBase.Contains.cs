using System.Diagnostics.CodeAnalysis;

namespace zms9110750.TreeCollection.Abstract;

public abstract partial class TreeNodeBase<T>
{
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
	#endregion
}
