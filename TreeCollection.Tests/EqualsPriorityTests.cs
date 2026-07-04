using zms9110750.TreeCollection.Ordered;

namespace TreeCollection.Tests;

public class EqualsPriorityTests
{
    [Fact]
    public void Test_Equals_Overload_Resolution()
    {
        var node = new TreeNode<string>("hello");

        // 调用 node.Equals("hello") — object.Equals(object) 还是扩展的 Equals(string)?
        var result = node.Equals("hello");

        // 如果走 object.Equals，是引用比较，node 不是 "hello"，返回 false
        // 如果走扩展的 Equals(string)，是值比较，返回 true
        WriteLine($"node.Equals(\"hello\") = {result}");
        Assert.True(result, "应该走值比较，返回 true");
    }

    [Fact]
    public void Test_Equals_With_Different_Value()
    {
        var node = new TreeNode<string>("hello");

        var result = node.Equals("world");

        WriteLine($"node.Equals(\"world\") = {result}");
        Assert.False(result);
    }

    [Fact]
    public void Test_Equals_Explicit_Interface()
    {
        var node = new TreeNode<string>("hello");
        var equatable = (IEquatable<string>)node;

        var result = equatable.Equals("hello");

        WriteLine($"((IEquatable<string>)node).Equals(\"hello\") = {result}");
        Assert.True(result);
    }

    private static void WriteLine(string msg) =>
        System.Diagnostics.Debug.WriteLine(msg);
}
