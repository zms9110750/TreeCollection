namespace zms9110750.TreeCollection.Abstract;

public abstract partial class TreeNodeBase <T> {
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
}
