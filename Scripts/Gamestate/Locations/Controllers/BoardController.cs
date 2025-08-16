using Godot;
using Kompas.Cards.Controllers;

namespace Kompas.Gamestate.Locations.Controllers;

//Note: it may be more accurate to call this a view, and/or split it into view and controller
public abstract partial class BoardController : Node, IBoardController
{
	//TODO: replace with a "move" and a "play" function, to eventually add animations distinct to each
	public abstract void Play(ICardController card);

	public abstract void Move(ICardController card, MovePath path);

	public virtual void Remove(ICardController card) { }

	//TODO: it shouldn't be refreshed, change the contract of the ILocationController
	// so that BoardController doesn't have these methods
	public void Refresh() => throw new System.NotImplementedException();
	public void Refresh(ICardController cardController) => Refresh();
}

public interface IBoardController : ILocationController
{
	public void Play(ICardController card);
	public void Move(ICardController card, MovePath path);
}