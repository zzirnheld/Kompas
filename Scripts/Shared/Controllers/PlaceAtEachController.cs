using System.Collections.Generic;
using Godot;
using Kompas.Shared.Enumerable;
using Kompas.Shared.Exceptions;

namespace Kompas.Shared.Controllers
{
	public partial class PlaceAtEachController : Node
	{
		[Export]
		private Node[]? _nodes;
		private Node[] Nodes => _nodes
			?? throw new UnassignedReferenceException();
		[Export]
		private Node? _overflow;
		private Node Overflow => _overflow
			?? throw new UnassignedReferenceException();

		public void Spread(IReadOnlyCollection<Node> nodes)
		{
			foreach (var (index, node) in nodes.Enumerate())
			{
				node.GetParent()?.RemoveChild(node);
				Nodes[index].AddChild(node);
			}
		}
	}
}