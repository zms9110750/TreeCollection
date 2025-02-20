using System.Collections;
using zms9110750.TreeCollection.Extension;

namespace zms9110750.TreeCollection.Abstract;
public abstract class TreeList<T> : IList<T>
{
	/// <summary>
	/// 内部集合
	/// </summary>
	protected List<T> InnerList { get; } = [];
	/// <summary>
	/// 添加一个项目到指定位置
	/// </summary>
	/// <param name="index">要放置位置</param>
	/// <param name="item">要添加的项目</param>
	/// <returns>添加进去的值</returns>
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
	/// <summary>
	/// 添加一堆项目到指定位置
	/// </summary>
	/// <param name="index">要放置的位置</param>
	/// <param name="item">要添加的项目</param>
	public virtual void AddAt(Index index, params ReadOnlySpan<T> item)
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
	/// <summary>
	/// 添加一堆项目到指定位置
	/// </summary>
	/// <param name="index">要放置的位置</param>
	/// <param name="item">要添加的项目</param>
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
	/// <summary>
	/// 移除指定位置的项目
	/// </summary>
	/// <param name="index">要移除的位置</param>
	/// <returns></returns>
	public virtual T RemoveAt(Index index)
	{
		var item = this[index];
		InnerList.RemoveAt(index.GetOffset(InnerList.Count));
		return item;
	}
	/// <summary>
	/// 移除符合条件的项目
	/// </summary>
	/// <param name="match">条件</param>
	/// <param name="range">仅筛选以下范围</param>
	/// <returns>被移除的项目列表</returns>
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
				list.AddRange(InnerList.Slice(off, len));
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
	/// <summary>
	/// 移除第一个匹配的的项目
	/// </summary>
	/// <param name="item">查找项目</param>
	/// <param name="range">查询范围</param>
	/// <returns>被移除的项目。若没有找到，则返回null。</returns>
	public virtual T? Remove(T item, Range? range = default)
	{
		var index = IndexOf(item, range);
		return index >= 0 ? RemoveAt(index) : default;
	}
	/// <summary>
	/// 查询项目是否存在于该列表中
	/// </summary>
	/// <param name="item">查询项目</param>
	/// <param name="range">查询范围</param>
	/// <returns>若存在则为true，否则为false。</returns>
	public abstract bool Contains(T item, Range? range = default);
	/// <summary>
	/// 查询项目在列表中的位置
	/// </summary>
	/// <param name="item">查找项目</param>
	/// <param name="range">查询范围</param>
	/// <returns>项目的索引。若不存在，则返回-1。</returns>
	public abstract int IndexOf(T item, Range? range = default);
	/// <summary>
	/// 验证元素是否合法。
	/// </summary>
	/// <remarks>若不合法则抛出异常。</remarks>
	/// <param name="item">严重项目</param>
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

	IEnumerator IEnumerable.GetEnumerator()
	{
		return ((IEnumerable)InnerList).GetEnumerator();
	}

	int IList<T>.IndexOf(T item)
	{
		return IndexOf(item, default);
	}

	void IList<T>.Insert(int index, T item)
	{
		AddAt(index, item);
	}

	void IList<T>.RemoveAt(int index)
	{
		RemoveAt(index);
	}

	void ICollection<T>.Add(T item)
	{
		AddAt(^0, item);
	}

	void ICollection<T>.Clear()
	{
		RemoveAll();
	}

	bool ICollection<T>.Contains(T item)
	{
		return Contains(item, default);
	}

	bool ICollection<T>.Remove(T item)
	{
		var index = IndexOf(item);
		if (index >= 0)
		{
			RemoveAt(index);
			return true;
		}
		return false;
	}
}
