using zms9110750.TreeCollection.Ordered;

namespace TreeCollection.Tests;

public class TreeBehaviorTests
{
    [Fact]
    public void SetParentNull_DoesNotRemoveFromOldParentList()
    {
        var root = new TreeNode<string>("root");
        var child = new TreeNode<string>("child");
        root.Add(child);

        // 初始状态：root 有1个子节点
        Assert.Single(root);
        Assert.Same(root, child.Parent);

        root.RemoveAll(_ => true); // 移除所有子节点

        Assert.Null(child.Parent);
        Assert.Empty(root);
    }

    [Fact]
    public void AddAt_SingleNode_IsConsistent()
    {
        var root = new TreeNode<string>("root");
        var child = new TreeNode<string>("child");
        var other = new TreeNode<string>("other");

        root.AddAt(0, child);
        Assert.Single(root);
        Assert.Same(root, child.Parent);

        other.AddAt(0, child);
        // child 应该从 root 移到了 other
        Assert.Empty(root);
        Assert.Single(other);
        Assert.Same(other, child.Parent);
    }

    [Fact]
    public void AddAt_Batch_InsertBeforeRemove()
    {
        var root = new TreeNode<string>("root");
        var child = new TreeNode<string>("child");
        root.AddAt(0, child);

        var other = new TreeNode<string>("other");

        other.AddAt(0, new[] { child });

        Assert.Empty(root);
        Assert.Single(other);
        Assert.Same(other, child.Parent);
    }

    [Fact]
    public void Remove_MovingNodeBetweenTrees_Works()
    {
        var root = new TreeNode<string>("root");
        var child = new TreeNode<string>("child");
        root.Add(child);

        var other = new TreeNode<string>("other");
        other.Add(child); // 用 Add (扩展方法) 移到另一棵树

        Assert.Empty(root);
        Assert.Single(other);
        Assert.Same(other, child.Parent);
    }

    [Fact]
    public void RemoveAll_WithPredicate_OnlyRemovesMatching()
    {
        var root = new TreeNode<string>("root");
        root.Add("a", "b", "c", "d");

        var removed = root.RemoveAll(n => string.Compare(n.Value, "b", StringComparison.Ordinal) > 0);

        Assert.Equal(2, removed);
        Assert.Equal(2, root.Count);
        Assert.Equal(["a", "b"], root.Select(n => n.Value));
    }

    [Fact]
    public void RemoveAll_NullPredicate_RemovesAll()
    {
        var root = new TreeNode<string>("root");
        root.Add("x", "y", "z");

        var removed = root.RemoveAll(null); // null predicate = 全移除

        Assert.Equal(3, removed);
        Assert.Empty(root);
    }

    [Fact]
    public void IsValidSlice_AfterCreation_IsTrue()
    {
        var root = new TreeNode<string>("root");
        root.Add("a", "b", "c");

        var slice = root[..2];

        Assert.True(slice.IsValid);
    }

    [Fact]
    public void IsValidSlice_AfterModification_IsFalse()
    {
        var root = new TreeNode<string>("root");
        root.Add("a", "b", "c");

        var slice = root[..2];
        root.Add("d");

        Assert.False(slice.IsValid);
    }

    [Fact]
    public void Slice_EnumerableInterface_Works()
    {
        var root = new TreeNode<string>("root");
        root.Add("x", "y", "z");

        var slice = root[..2];
        var enumerable = (IEnumerable<TreeNode<string>>)slice;
        var enumerator = enumerable.GetEnumerator();

        Assert.True(enumerator.MoveNext());
        Assert.Equal("x", enumerator.Current.Value);
        Assert.True(enumerator.MoveNext());
        Assert.Equal("y", enumerator.Current.Value);
        Assert.False(enumerator.MoveNext());
    }
}
