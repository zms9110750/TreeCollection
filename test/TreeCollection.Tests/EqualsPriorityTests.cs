using zms9110750.TreeCollection.Ordered;

namespace TreeCollection.Tests;

public class EqualsPriorityTests
{
    [Fact]
    public void Test_Equals_Overload_Resolution()
    {
        var node = new TreeNode<string>("hello");

        // node.Equals("hello") 走的是 object.Equals(object?)，引用比较，返回 false
        // 扩展方法的 Equals(string) 参数更精确，但实例方法永远优先于扩展方法
        var result = node.Equals("hello");

        WriteLine($"node.Equals(\"hello\") = {result}");
        Assert.False(result, "object.Equals 优先于扩展方法，所以是引用比较");
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

    private static void WriteLine(string msg)
    {
        System.Diagnostics.Debug.WriteLine(msg);
    }
}
