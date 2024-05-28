using System.Linq;
using Godot;

namespace Kompas.Godot
{
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
		public static void ClearChildren(this Node parent)
		{
			foreach (var child in parent.GetChildren().ToArray())
			{
				parent.RemoveChild(child);
				child.QueueFree();
			}
		}

		public static Vector2 GlobalCenter(this Control node)
		{
			return node.GlobalPosition + (node.Size / 2);
		}
	}
}
