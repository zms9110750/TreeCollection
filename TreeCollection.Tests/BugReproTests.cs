using zms9110750.TreeCollection.Ordered;

namespace TreeCollection.Tests;

/// <summary>
/// 复现已知 bug 的测试。每个测试的名称标明对应的 bug 编号。
/// 当前预期结果 = 按现有代码的运行结果（即 bug 的表现）。
/// </summary>
public class BugReproTests
{
    // ============================================================
    // Bug 1: LastChild 返回的是第一个节点而不是最后一个
    // ============================================================
    [Fact]
    public void Bug1_LastChild_ReturnsFirstChild()
    {
        var root = new TreeNode<string>("root");
        root.Add("first", "second", "third");

        // 按照正确语义 LastChild 应该是 "third"（索引2）
        // 但 bug 让它返回了 instance[0]，也就是 "first"（索引0）
        var last = root.LastChild;

        Assert.NotNull(last);
        Assert.Equal("third", last!.Value); // Bug: 实际得到 "first"
    }

    // ============================================================
    // Bug 2: NodeListSlice.IsValid 语义取反
    // ============================================================
    [Fact]
    public void Bug2_IsValid_IsInverted()
    {
        var root = new TreeNode<string>("root");
        root.Add("a", "b", "c");

        var slice = root[..2]; // 获取前两个子节点的切片

        // 切片刚创建时，集合没有修改，Version 应该匹配 => IsValid 应为 true
        // bug：IsValid 在 Version 不匹配时返回 true，所以刚创建时可能返回 false
        Assert.True(slice.IsValid, "刚创建的切片应该是有效的");

        // 修改集合后，切片应该失效
        root.Add("d");
        Assert.False(slice.IsValid, "集合修改后切片应该失效");
    }

    // ============================================================
    // Bug 3: NodeListSlice 的 IEnumerator 接口（已修复）
    // ============================================================
    [Fact]
    public void Bug3_Slice_Enumerable_Works()
    {
        var root = new TreeNode<string>("root");
        root.Add("x", "y", "z");

        var slice = root[..2]; // NodeListSlice

        // 通过 IEnumerable<TreeNode<string>> 接口枚举 -> 应正常工作
        var enumerable = (IEnumerable<TreeNode<string>>)slice;
        var enumerator = enumerable.GetEnumerator();

        Assert.True(enumerator.MoveNext());
        Assert.Equal("x", enumerator.Current.Value);
        Assert.True(enumerator.MoveNext());
        Assert.Equal("y", enumerator.Current.Value);
        Assert.False(enumerator.MoveNext());
    }

    // ============================================================
    // Bug 4: TreeNode.RemoveAll 不从 ChildrenNode 列表中移除节点
    // ============================================================
    [Fact]
    public void Bug4_RemoveAll_DoesNotRemoveFromList()
    {
        var root = new TreeNode<string>("root");
        root.Add("a", "b", "c", "d");

        // 移除所有值 > "b" 的节点（即 "c", "d"）
        var removed = root.RemoveAll(node => string.Compare(node.Value, "b", StringComparison.Ordinal) > 0);

        // 断言预期移除了2个
        Assert.Equal(2, removed);

        // 但 Count 应该变成 2（只剩 "a", "b"）
        // bug：Count 仍然是 4，因为节点没从 ChildrenNode 里删掉
        Assert.Equal(2, root.Count);

        // 枚举应该只返回 "a" 和 "b"
        // bug：孤儿节点（Parent = null）仍然出现在枚举中
        var values = root.Select(n => n.Value).ToList();
        Assert.Equal(["a", "b"], values);
    }
}
