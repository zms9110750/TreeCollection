using System.Runtime.InteropServices;

namespace zms9110750.TreeCollection.Extension;
internal static class SpanExtensions
{
	/// <summary>
	/// 调整 span 中元素的位置。
	/// </summary>
	/// <typeparam name="T">span 中元素的类型。</typeparam>
	/// <param name="span">要调整的 span。</param>
	/// <param name="index">要移动的元素的索引。</param>
	/// <param name="moveTo">要移动到的位置索引。</param>
	public static void AdjustPositions<T>(this Span<T> span, int index, int moveTo)
	{
		var temp = span[moveTo];
		if (index < moveTo)
		{
			span = span[index..(moveTo + 1)];
			span[..^1].CopyTo(span[1..]);
			span[0] = temp;
		}
		else if (index > moveTo)
		{
			span = span[moveTo..(index + 1)];
			span[1..].CopyTo(span[..^1]);
			span[^1] = temp;
		}
	}

	/// <summary>
	/// 从 List 中获取一个 Span。
	/// </summary>
	/// <typeparam name="T">List 中元素的类型。</typeparam>
	/// <param name="list">要获取 Span 的 List。</param>
	/// <returns>从 List 获取的 Span。</returns>
	public static Span<T> AsSpan<T>(this List<T> list)
	{
		return CollectionsMarshal.AsSpan(list);
	}
}
