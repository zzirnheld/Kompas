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
}

public interface IBoardController
{
	public void Play(ICardController card);
	public void Move(ICardController card, MovePath path);
	public void Remove(ICardController card);
}