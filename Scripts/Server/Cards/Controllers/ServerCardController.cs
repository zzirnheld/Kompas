using System;
using Godot;
using Kompas.Cards.Controllers;
using Kompas.Cards.Models;
using Kompas.Client.UI;
using Kompas.UI.CardInfoDisplayers;

namespace Kompas.Server.Cards.Controllers;

public class ServerCardController : ICardController
{
	//FUTURE: when I want to display the card server side, have it store the card
	public Node3D Node => throw new NullReferenceException("Server cards don't have nodes yet");
	public IGameCardInfo Card => throw new NullReferenceException("Server cards don't have nodes yet");
	public bool Focused => false;

	public event EventHandler? Refreshed;
	public event EventHandler<GameCard?>? AnythingRefreshed;
	public event EventHandler<GameCard?>? StatsRefreshed;
	public event EventHandler<GameCard?>? LinksRefreshed;
	public event EventHandler<GameCard?>? AugmentsRefreshed;
	public event EventHandler<GameCard?>? TargetingRefreshed;
	public event EventHandler<GameCard?>? LocationRefreshed;

	public void Delete() { }

	public void MoveToBoard(Action placeOnBoard) { placeOnBoard(); }

	public void RefreshAugments()
	{
		AugmentsRefreshed?.Invoke(this, null);
		AnythingRefreshed?.Invoke(this, null);
	}
	public void RefreshLinks()
	{
		LinksRefreshed?.Invoke(this, null);
		AnythingRefreshed?.Invoke(this, null);
	}
	public void RefreshStats()
	{
		StatsRefreshed?.Invoke(this, null);
		AnythingRefreshed?.Invoke(this, null);
	}
	public void RefreshTargeting()
	{
		TargetingRefreshed?.Invoke(this, null);
		AnythingRefreshed?.Invoke(this, null);
	}
	public void RefreshLocation()
	{
		LocationRefreshed?.Invoke(this, null);
		AnythingRefreshed?.Invoke(this, null);
	}

	public void ShowEffectSource(bool current) { }

	IHoverableCardInfoDisplayer ICardController.PlaceCameraAboveCard(CameraFollowObject cameraNode, uint cameraMask, uint arrivedCameraMask, Action<IHoverableCardInfoDisplayer> action)
	{
		throw new NotImplementedException();
	}
}