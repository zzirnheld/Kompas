using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Godot;
using Kompas.Shared;

namespace Kompas.Godot;

public static class NodeExtensions
{
	/// <summary>
	/// Removes the child from its previous parent (if it had any),
	/// and adds it as a child of this node.
	/// </summary>
	public static void TransferChild(this Node parent, Node child)
	{
		child.GetParent()?.RemoveChild(child);
		parent.AddChild(child);
	}

	/// <summary>
	/// Queues the freeing (aka deletion) of each of the parent's children.
	/// </summary>
	public static void QueueFreeChildren(this Node parent)
	{
		foreach (var child in parent.GetChildren().ToArray())
		{
			child.QueueFree();
		}
	}

	/// <summary>
	/// Removes each of the parent node's children from the children list, and queues them to be freed
	/// </summary>
	/// <param name="parent"></param>
	public static void RemoveAndQueueFreeChildren(this Node parent)
	{
		foreach (var child in parent.GetChildren().ToArray())
		{
			parent.RemoveChild(child);
			child.QueueFree();
		}
	}

	/// <summary>
	/// Removes each of the parent node's children from the children list.
	/// Does NOT queue the children for freeing - use when you want the nodes to persist
	/// </summary>
	/// <param name="parent"></param>
	public static void RemoveChildren(this Node parent)
	{
		foreach (var child in parent.GetChildren().ToArray())
		{
			parent.RemoveChild(child);
		}
	}

	public static Vector2 GlobalCenter(this Control node)
	{
		return node.GlobalPosition + (node.Size / 2);
	}

	public delegate void EachFrame(float delta);
	public delegate Result<T> EachFrame<T>(float delta);
	public static Result<T> ResultOf<T>(T item) => Result<T>.Of(item);

	/// <summary>
	/// Asynchronously runs the given function each frame,
	/// and returns once the given function returns something other than None.
	/// </summary>
	public static async Task<T?> DoEachFrame<T>(this Node node, EachFrame<T> eachLoop)
	{
		ulong frameMsec = Time.GetTicksMsec();
		while (true)
		{
			ulong nowMsec = Time.GetTicksMsec();
			float delta = (nowMsec - frameMsec) / 1000f;
			frameMsec = nowMsec;

			var ret = eachLoop(delta);
			if (ret.HasResult) return ret.Item;

			var tree = node.GetTree();
			if (tree == null) return default;
			await node.ToSignal(tree, SceneTree.SignalName.ProcessFrame);
		}
	}

	/// <summary>
	/// Asynchronously does the given function each frame,
	/// and never returns.
	/// The returned Task exists to keep a handle on any exceptions that might bubble up.
	/// ...if I've understood this Task stuff correctly.
	/// </summary>
	public static async Task DoEachFrame(this Node node, EachFrame eachLoop)
	{
		await node.DoEachFrame(delta => { eachLoop(delta); return Result<object>.None; });
	}

	public static IEnumerable<Node> GetDescendants(this Node node)
	{
		return node.GetChildren()
			.SelectMany(child => child.GetDescendants().Prepend(child));
	}
}
