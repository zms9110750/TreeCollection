using zms9110750.TreeCollection.Extension;

namespace zms9110750.TreeCollection.Abstract;
public abstract class TreeList<T> : ITreeList<T>
{ 
	/// <summary>
	/// 内部集合
	/// </summary>
	protected List<T> InnerList { get; } = []; 
	public virtual T AddAt(Index index, T item)
	{
		ValidateElement(item);
		var count = index.GetOffset(InnerList.Count);
		if (count == Count)
		{
			InnerList.Add(item);
		}
		else
		{
			InnerList.Insert(count, item);
		}
		return item;
	}
	/// <inheritdoc cref="AddAt(Index, IEnumerable{T})" /> 
	public virtual void AddAt(Index index, params ReadOnlySpan<T> item)
	{
		foreach (var i in item)
		{
			ValidateElement(i);
		}
		var count = index.GetOffset(InnerList.Count);
		if (count == Count)
		{
#if NET8_0_OR_GREATER

			InnerList.AddRange(item);
#else
			InnerList.AddRange(item.ToArray());
#endif 
		}
		else
		{
#if NET8_0_OR_GREATER
			InnerList.InsertRange(count, item);
#else
			InnerList.InsertRange(count, item.ToArray());
#endif
		} 
	}
	public virtual void AddAt(Index index, IEnumerable<T> item)
	{
		foreach (var i in item)
		{
			ValidateElement(i);
		}
		var count = index.GetOffset(InnerList.Count);
		if (count == Count)
		{
			InnerList.AddRange(item);
		}
		else
		{
			InnerList.InsertRange(count, item);
		}
	}
	public virtual T RemoveAt(Index index)
	{
		var item = this[index];
		InnerList.RemoveAt(index.GetOffset(InnerList.Count));
		return item;
	}
	public virtual List<T> RemoveAll(Predicate<T>? match = default, Range? range = default)
	{
		List<T> list = [];
		switch ((match, range))
		{
			case (null, null):
				list.AddRange(InnerList);
				InnerList.Clear();
				break;
			case (null, { } r):
				var (off, len) = r.GetOffsetAndLength(InnerList.Count);
#if NET8_0_OR_GREATER
				list.AddRange(InnerList.Slice(off, len));
#else
				list.AddRange(InnerList.GetRange(off, len));
#endif
				InnerList.RemoveRange(off, len);
				break;
			case ({ } m, null):
				for (int i = Count - 1; i >= 0; i--)
				{
					if (m(InnerList[i]))
					{
						list.Add(InnerList[i]);
						RemoveAt(i);
					}
				}
				break;
			case ({ } m, { } r):
				var start = r.Start.GetOffset(InnerList.Count);
				var end = r.End.GetOffset(InnerList.Count);
				for (int i = end; i >= start; i--)
				{
					if (m(InnerList[i]))
					{
						list.Add(InnerList[i]);
						RemoveAt(i);
					}
				}
				break;
		}
		return list;
	}
	public virtual T? Remove(T item, Range? range = default)
	{
		var index = IndexOf(item, range);
		return index >= 0 ? RemoveAt(index) : default;
	}
	public abstract bool Contains(T item, Range? range = default);
	public abstract int IndexOf(T item, Range? range = default);
	/// <summary>
	/// 验证元素是否合法。
	/// </summary>
	/// <remarks>若不合法则抛出异常。</remarks>
	/// <param name="item">验证项目</param>
	protected abstract void ValidateElement(T item);
	public abstract T this[int index] { get; set; }
	/// <summary>
	/// 获取切片
	/// </summary>
	/// <param name="start">开始位置</param>
	/// <param name="length">长度</param>
	/// <returns></returns>
	public Span<T> Slice(int start, int length) => InnerList.AsSpan().Slice(start, length);
	public int Count => ((ICollection<T>)InnerList).Count;
	public bool IsReadOnly => ((ICollection<T>)InnerList).IsReadOnly;

	public void CopyTo(T[] array, int arrayIndex)
	{
		((ICollection<T>)InnerList).CopyTo(array, arrayIndex);
	}

	public IEnumerator<T> GetEnumerator()
	{
		return ((IEnumerable<T>)InnerList).GetEnumerator();
	}

}
