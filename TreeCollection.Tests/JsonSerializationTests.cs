using System.Text.Json;
using zms9110750.TreeCollection.Ordered;

namespace TreeCollection.Tests;

/// <summary>
/// JSON 序列化/反序列化的语义测试
/// </summary>
public class JsonSerializationTests
{
    [Theory]
    [InlineData("""{"Value":"root"}""", "root", 0)]
    [InlineData("""{"Value":"root","Children":[{"Value":"a"},{"Value":"b"}]}""", "root", 2)]
    public void DeserializeRootNode(string json, string expectedValue, int expectedCount)
    {
        var node = JsonSerializer.Deserialize<TreeNode<string>>(json);
        Assert.NotNull(node);
        Assert.Equal(expectedValue, node!.Value);
        Assert.Equal(expectedCount, node.Count);
        if (expectedCount > 0)
        {
            Assert.Same(node, node[0].Parent);
        }
    }

    /// <summary>序列化包含 Value 和 Children</summary>
    [Fact]
    public void SerializeProducesJsonWithValueAndChildren()
    {
        var root = new TreeNode<string>("root");
        root.Add("a", "b");
        var json = JsonSerializer.Serialize(root);
        Assert.Contains("root", json);
        Assert.Contains("Children", json);
    }

    /// <summary>序列化再反序列化后数据一致</summary>
    [Fact]
    public void RoundtripPreservesStructure()
    {
        var root = new TreeNode<string>("root");
        root.Add("x", "y", "z");
        var json = JsonSerializer.Serialize(root);
        var des = JsonSerializer.Deserialize<TreeNode<string>>(json);
        Assert.NotNull(des);
        Assert.Equal("root", des!.Value);
        Assert.Equal(3, des.Count);
        Assert.Equal(["x", "y", "z"], des.Select(n => n.Value));
    }

    /// <summary>缺少 Value 属性的 JSON 抛出异常</summary>
    [Fact]
    public void MissingValueThrows()
    {
        Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<TreeNode<string>>("""{"Children":[]}"""));
    }
}
