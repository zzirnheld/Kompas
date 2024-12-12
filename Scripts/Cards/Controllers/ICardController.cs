using System;
using Godot;
using Kompas.Cards.Models;
using Kompas.Client.UI;
using Kompas.UI.CardInfoDisplayers;

namespace Kompas.Cards.Controllers;

public interface ICardController
{
	/// <summary>
	/// Anything on the card has been refreshed
	/// </summary>
	public event EventHandler<GameCard?>? AnythingRefreshed;

	/// <summary>
	/// Refreshes the stats displayed for this card.
	/// Should refresh anything showing this card: the model for this card, the mouse-over UI, etc.
	/// </summary>
	public void RefreshStats();
	public event EventHandler<GameCard?>? StatsRefreshed;

	/// <summary>
	/// Refreshes showing cards linked to this card.
	/// </summary>
	public void RefreshLinks();
	public event EventHandler<GameCard?>? LinksRefreshed;
	
	public void RefreshAugments();
	public event EventHandler<GameCard?>? AugmentsRefreshed;

	public void RefreshTargeting();
	public event EventHandler<GameCard?>? TargetingRefreshed;

	public void ShowEffectSource(bool current);

	public void Delete();

	public Node3D Node { get; }
	public IGameCardInfo Card { get; }

	/// <summary>
	/// Place the given camera above the card, and return the control info displayer with events we can hook up.
	/// Potential refactor: make the return type have an interface with just the events.
	///</summary>
	public IHoverableCardInfoDisplayer PlaceCameraAboveCard(CameraFollowObject cameraNode, uint cameraMask, uint arrivedCameraMask, Action<IHoverableCardInfoDisplayer> whenArrive);

	/// <summary>
	/// Exists so that I can animate the card flying up and down again.
	/// Maybe there's a better way to do this, but if there is, I don't know it.
	///</summary>
	public void MoveToBoard(Action placeOnBoard);
}