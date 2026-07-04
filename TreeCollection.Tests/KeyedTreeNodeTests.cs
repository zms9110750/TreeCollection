using zms9110750.TreeCollection.Keyed;

namespace TreeCollection.Tests;

public class KeyedTreeNodeTests
{
    private static IKeyedTree<TKey, TValue, KeyedTreeNode<TKey, TValue>> AsTree<TKey, TValue>(KeyedTreeNode<TKey, TValue> node)
        where TKey : notnull
    {
        return node;
    }

    [Fact]
    public void Create_Root_HasCorrectState()
    {
        var root = new KeyedTreeNode<string, string>("root");
        Assert.Equal("root", root.Value);
        Assert.Null(root.Parent);
        Assert.Same(root, root.Root);
        Assert.Equal(0, root.Depth);
        Assert.Empty(root);
    }

    [Fact]
    public void Add_ChildByKey_Works()
    {
        var root = new KeyedTreeNode<string, string>("root");
        var child = new KeyedTreeNode<string, string>("child");
        root.Add("a", child);

        Assert.Single(root);
        Assert.Same(child, root["a"]);
        Assert.Same(root, child.Parent);
        Assert.Equal(1, child.Depth);
    }

    [Fact]
    public void Add_MultipleChildren_KeyOrdered()
    {
        var root = new KeyedTreeNode<string, string>("root");
        root.Add("c", new KeyedTreeNode<string, string>("c"));
        root.Add("a", new KeyedTreeNode<string, string>("a"));
        root.Add("b", new KeyedTreeNode<string, string>("b"));

        Assert.Equal(3, root.Count);
        Assert.Equal(["a", "b", "c"], root.Keys.ToList());
    }

    [Fact]
    public void Add_ByValue_CreatesNode()
    {
        var root = new KeyedTreeNode<string, string>("root");
        var tree = AsTree(root);
        var child = tree.Add("k1", "v1");

        Assert.NotNull(child);
        Assert.Equal("v1", child.Value);
        Assert.Same(root, child.Parent);
        Assert.Same(child, root["k1"]);
    }

    [Fact]
    public void Get_ByKey_ThrowsIfMissing()
    {
        var root = new KeyedTreeNode<string, string>("root");
        Assert.Throws<KeyNotFoundException>(() => root["missing"]);
    }

    [Fact]
    public void TryGetValue_Works()
    {
        var root = new KeyedTreeNode<string, string>("root");
        root.Add("k", new KeyedTreeNode<string, string>("v"));

        Assert.True(root.TryGetValue("k", out var node));
        Assert.Equal("v", node!.Value);
        Assert.False(root.TryGetValue("x", out _));
    }

    [Fact]
    public void ContainsKey_Works()
    {
        var root = new KeyedTreeNode<string, string>("root");
        root.Add("k", new KeyedTreeNode<string, string>("v"));
        Assert.True(root.ContainsKey("k"));
        Assert.False(root.ContainsKey("x"));
    }

    [Fact]
    public void Remove_ByKey_RemovesAndOrphans()
    {
        var root = new KeyedTreeNode<string, string>("root");
        root.Add("k", new KeyedTreeNode<string, string>("v"));
        var removed = root.RemoveBy("k");
        Assert.NotNull(removed);
        Assert.Equal("v", removed!.Value);
        Assert.Null(removed.Parent);
        Assert.Empty(root);
    }

    [Fact]
    public void Remove_NonExistentKey_ReturnsNull()
    {
        var root = new KeyedTreeNode<string, string>("root");
        Assert.Null(root.RemoveBy("missing"));
    }

    [Fact]
    public void Clear_RemovesAllAndOrphans()
    {
        var root = new KeyedTreeNode<string, string>("root");
        var a = new KeyedTreeNode<string, string>("1");
        var b = new KeyedTreeNode<string, string>("2");
        root.Add("a", a);
        root.Add("b", b);
        root.Clear();
        Assert.Empty(root);
        Assert.Null(a.Parent);
        Assert.Null(b.Parent);
    }

    [Fact]
    public void ChangeKey_ReKeys()
    {
        var root = new KeyedTreeNode<string, string>("root");
        root.Add("old", new KeyedTreeNode<string, string>("value"));
        var tree = AsTree(root);
        tree.ChangeKey("old", "new");
        Assert.False(root.ContainsKey("old"));
        Assert.True(root.ContainsKey("new"));
        Assert.Equal("value", root["new"].Value);
    }

    [Fact]
    public void ChangeKey_TargetExists_ThrowsAndRestores()
    {
        var root = new KeyedTreeNode<string, string>("root");
        root.Add("a", new KeyedTreeNode<string, string>("1"));
        root.Add("b", new KeyedTreeNode<string, string>("2"));
        var tree = AsTree(root);
        Assert.Throws<ArgumentException>(() => tree.ChangeKey("a", "b"));
        Assert.True(root.ContainsKey("a"));
        Assert.Equal("1", root["a"].Value);
    }

    [Fact]
    public void SwapChildren_Works()
    {
        var root = new KeyedTreeNode<string, string>("root");
        var nodeA = new KeyedTreeNode<string, string>("1");
        var nodeB = new KeyedTreeNode<string, string>("2");
        root.Add("a", nodeA);
        root.Add("b", nodeB);
        var tree = AsTree(root);
        tree.SwapChildren("a", "b");
        Assert.Same(nodeB, root["a"]);
        Assert.Same(nodeA, root["b"]);
    }

    [Fact]
    public void Replace_ReplacesNode()
    {
        var root = new KeyedTreeNode<string, string>("root");
        var old = new KeyedTreeNode<string, string>("old");
        root.Add("k", old);
        var fresh = new KeyedTreeNode<string, string>("new");
        var tree = AsTree(root);
        Assert.True(tree.Replace(old, fresh));
        Assert.Same(fresh, root["k"]);
        Assert.Equal("new", fresh.Value);
        Assert.Same(root, fresh.Parent);
        Assert.Null(old.Parent);
    }

    [Fact]
    public void RemoveAll_RemovesMatching()
    {
        var root = new KeyedTreeNode<string, string>("root");
        root.Add("a", new KeyedTreeNode<string, string>("keep"));
        root.Add("b", new KeyedTreeNode<string, string>("remove"));
        root.Add("c", new KeyedTreeNode<string, string>("remove"));
        var tree = AsTree(root);
        int removed = tree.RemoveAll(n => n.Value == "remove");
        Assert.Equal(2, removed);
        Assert.Single(root);
        Assert.True(root.ContainsKey("a"));
    }

    [Fact]
    public void RemoveAll_NullPredicate_RemovesAll()
    {
        var root = new KeyedTreeNode<string, string>("root");
        root.Add("a", new KeyedTreeNode<string, string>("1"));
        root.Add("b", new KeyedTreeNode<string, string>("2"));
        var tree = AsTree(root);
        int removed = tree.RemoveAll(null);
        Assert.Equal(2, removed);
        Assert.Empty(root);
    }

    [Fact]
    public void Contains_ByValue_FindsMatch()
    {
        var root = new KeyedTreeNode<string, string>("root");
        root.Add("a", new KeyedTreeNode<string, string>("hello"));
        var tree = AsTree(root);
        Assert.True(tree.Contains("hello"));
        Assert.False(tree.Contains("world"));
    }

    [Fact]
    public void DeepTree_DepthAndRoot()
    {
        var root = new KeyedTreeNode<string, string>("root");
        var l1 = new KeyedTreeNode<string, string>("1");
        root.Add("l1", l1);
        var l2 = new KeyedTreeNode<string, string>("2");
        l1.Add("l2", l2);
        Assert.Equal(0, root.Depth);
        Assert.Equal(1, l1.Depth);
        Assert.Equal(2, l2.Depth);
        Assert.Same(root, l2.Root);
    }

    [Fact]
    public void MoveNode_BetweenKeyedTrees()
    {
        var root1 = new KeyedTreeNode<string, string>("r1");
        var root2 = new KeyedTreeNode<string, string>("r2");
        var child = new KeyedTreeNode<string, string>("v");
        root1.Add("k", child);
        root2.Add("k", child);
        Assert.Empty(root1);
        Assert.Single(root2);
        Assert.Same(root2, child.Parent);
    }

    [Fact]
    public void Enumerator_YieldsKeyValuePairs()
    {
        var root = new KeyedTreeNode<string, string>("root");
        root.Add("b", new KeyedTreeNode<string, string>("2"));
        root.Add("a", new KeyedTreeNode<string, string>("1"));
        var pairs = root.ToList();
        Assert.Equal(2, pairs.Count);
        Assert.Equal("a", pairs[0].Key);
        Assert.Equal("1", pairs[0].Value.Value);
    }

    [Fact]
    public void ForEach_OverChildren_Works()
    {
        var root = new KeyedTreeNode<string, string>("root");
        root.Add("x", new KeyedTreeNode<string, string>("1"));
        root.Add("y", new KeyedTreeNode<string, string>("2"));
        var values = new List<string>();
        foreach (var kvp in root)
        {
            values.Add(kvp.Value.Value);
        }

        Assert.Equal(["1", "2"], values);
    }

    [Fact]
    public void IDictionary_Interface_Works()
    {
        var root = new KeyedTreeNode<string, string>("root");
        var dict = (IDictionary<string, KeyedTreeNode<string, string>>)root;
        dict.Add("a", new KeyedTreeNode<string, string>("1"));
        Assert.True(dict.ContainsKey("a"));
        Assert.Equal("1", dict["a"].Value);
        Assert.Single(dict.Keys);
    }

    #region IDictionary 显式实现覆盖

    /// <summary>ICollection&lt;KVP&gt;.Contains 通过 key+引用判断</summary>
    [Fact]
    public void ICollectionContainsKVP()
    {
        var root = new KeyedTreeNode<string, string>("root");
        var child = new KeyedTreeNode<string, string>("v");
        root.Add("k", child);
        var coll = (ICollection<KeyValuePair<string, KeyedTreeNode<string, string>>>)root;
        Assert.True(coll.Contains(new KeyValuePair<string, KeyedTreeNode<string, string>>("k", child)));
        Assert.False(coll.Contains(new KeyValuePair<string, KeyedTreeNode<string, string>>("x", child)));
    }

    /// <summary>ICollection&lt;KVP&gt;.Remove 通过 key+引用删除并孤立</summary>
    [Fact]
    public void ICollectionRemoveKVP()
    {
        var root = new KeyedTreeNode<string, string>("root");
        var child = new KeyedTreeNode<string, string>("v");
        root.Add("k", child);
        var coll = (ICollection<KeyValuePair<string, KeyedTreeNode<string, string>>>)root;
        Assert.True(coll.Remove(new KeyValuePair<string, KeyedTreeNode<string, string>>("k", child)));
        Assert.Empty(root);
        Assert.Null(child.Parent);
    }

    /// <summary>ICollection.CopyTo 复制到数组</summary>
    [Fact]
    public void ICollectionCopyTo()
    {
        var root = new KeyedTreeNode<string, string>("root");
        root.Add("b", new KeyedTreeNode<string, string>("2"));
        root.Add("a", new KeyedTreeNode<string, string>("1"));
        var coll = (ICollection<KeyValuePair<string, KeyedTreeNode<string, string>>>)root;
        var arr = new KeyValuePair<string, KeyedTreeNode<string, string>>[2];
        coll.CopyTo(arr, 0);
        Assert.Equal(2, arr.Length);
    }

    /// <summary>索引器 set 覆盖旧节点并孤立</summary>
    [Fact]
    public void IndexerSetOverwritesAndOrphans()
    {
        var root = new KeyedTreeNode<string, string>("root");
        var old = new KeyedTreeNode<string, string>("old");
        root.Add("k", old);
        root["k"] = new KeyedTreeNode<string, string>("new");
        Assert.Equal("new", root["k"].Value);
        Assert.Null(old.Parent);
    }

    /// <summary>RemoveBy 不存在的 key 返回 null</summary>
    [Fact]
    public void RemoveByMissingKeyReturnsNull()
    {
        var root = new KeyedTreeNode<string, string>("root");
        Assert.Null(root.RemoveBy("missing"));
    }

    /// <summary>Contains(TValue) 查找不到返回 false</summary>
    [Fact]
    public void ContainsByValueNotFound()
    {
        var root = new KeyedTreeNode<string, string>("root");
        root.Add("a", new KeyedTreeNode<string, string>("x"));
        var tree = (IKeyedTree<string, string, KeyedTreeNode<string, string>>)root;
        Assert.False(tree.Contains("y"));
    }

    /// <summary>Replace 非子节点返回 false</summary>
    [Fact]
    public void ReplaceNonChildReturnsFalse()
    {
        var root = new KeyedTreeNode<string, string>("root");
        root.Add("a", new KeyedTreeNode<string, string>("1"));
        var tree = (IKeyedTree<string, string, KeyedTreeNode<string, string>>)root;
        Assert.False(tree.Replace(new KeyedTreeNode<string, string>("x"), new KeyedTreeNode<string, string>("y")));
    }

    /// <summary>ChangeKey 源 key 不存在时抛出异常</summary>
    [Fact]
    public void ChangeKeySourceNotFoundThrows()
    {
        var root = new KeyedTreeNode<string, string>("root");
        var tree = (IKeyedTree<string, string, KeyedTreeNode<string, string>>)root;
        Assert.Throws<KeyNotFoundException>(() => tree.ChangeKey("missing", "new"));
    }

    /// <summary>SwapChildren 任一 key 不存在时抛出异常</summary>
    [Fact]
    public void SwapChildrenKeyNotFoundThrows()
    {
        var root = new KeyedTreeNode<string, string>("root");
        root.Add("a", new KeyedTreeNode<string, string>("1"));
        var tree = (IKeyedTree<string, string, KeyedTreeNode<string, string>>)root;
        Assert.Throws<KeyNotFoundException>(() => tree.SwapChildren("a", "missing"));
    }

    /// <summary>Add 重复 key 抛出异常</summary>
    [Fact]
    public void AddDuplicateKeyThrows()
    {
        var root = new KeyedTreeNode<string, string>("root");
        root.Add("a", new KeyedTreeNode<string, string>("1"));
        Assert.Throws<ArgumentException>(() => root.Add("a", new KeyedTreeNode<string, string>("2")));
    }

    #endregion
}
