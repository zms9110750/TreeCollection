using System.Text;

namespace zms9110750.TreeCollection.Abstract;

public abstract partial class TreeNodeBase<T>
{
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
}
