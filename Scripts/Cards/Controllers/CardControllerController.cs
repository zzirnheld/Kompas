using Godot;

namespace Kompas.Cards.Controllers
{
	/// <summary>
    /// Used to be able to have the same prefab for server and client (and future, solo?) cards
    /// </summary>
	public partial class CardControllerController : Node
	{
		[Export]
		public CardController[] CardControllers { get; private set; }
	}
}