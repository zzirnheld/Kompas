using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using Kompas.Cards.Controllers;
using Kompas.Cards.Loading;
using Kompas.Cards.Models;
using Kompas.Shared;
using Kompas.Shared.Exceptions;
using Newtonsoft.Json;

namespace Kompas.UI.DeckBuilder
{
	public partial class DeckBuilderDeckController : Control
	{
		//TODO factor out this tab ui behavior to make this class just responsible for CRUD-y stuff
		public enum Tab { Normal, NewDeck, SaveAs }

		[Export]
		private SquareGridContainer? _deckNodesParent;
		private SquareGridContainer DeckNodesParent => _deckNodesParent
			?? throw new UnassignedReferenceException();

		[Export]
		private PackedScene? _deckCardControllerPrefab;
		private PackedScene DeckCardControllerPrefab => _deckCardControllerPrefab
			?? throw new UnassignedReferenceException();
		[Export]
		private DeckBuilderAvatarController? _avatarController;
		private DeckBuilderAvatarController AvatarController => _avatarController
			?? throw new UnassignedReferenceException();

		[Export]
		private DeckBuilderController? _deckBuilderController;
		private DeckBuilderController DeckBuilderController => _deckBuilderController
			?? throw new UnassignedReferenceException();

		[Export]
		private Control[]? _tabs;
		private Control[] Tabs => _tabs
			?? throw new UnassignedReferenceException();

		[Export]
		private OptionButton? _deckNameSelect;
		private OptionButton DeckNameSelect  => _deckNameSelect
			?? throw new UnassignedReferenceException();

		private readonly List<string> deckNames = new();

		private Decklist? currentDeck;
		//To maintain ordering of decks where copies of the same card aren't next to each other
		private readonly List<DeckBuilderDeckCardController> currentDeckCtrls = new();

		public DeckBuilderDeckCardController? Dragging { get; set; }


		private bool placeholdersWereActive;

		public override void _Ready()
		{
			foreach (var deckName in DeckAccess.GetDeckNames()) AddDeckName(deckName);

			AvatarController.Init(null, DeckBuilderController.CardView, this);
			if (deckNames.Count == 0) ShowController(Tab.NewDeck);
			else LoadDeck(0);
		}

		public void ShowController(Tab toShow)
		{
			foreach (Tab tab in Enum.GetValues(typeof(Tab)))
			{
				Tabs[(int)tab].Visible = tab == toShow;
			}
		}

		public void NewDeck(string name)
		{
			SaveDeck();
			ClearDeck();

			currentDeck = new() { deckName = name };
			AddDeckNameAndSelect(name);
			SaveDeck();
		}

		public void SaveAs(string name)
		{
			currentDeck ??= new();
			currentDeck = currentDeck.Copy(name);
			AddDeckNameAndSelect(name);
			SaveDeck();
		}

		private void AddDeckNameAndSelect(string name)
		{
			AddDeckName(name);
			DeckNameSelect.Select(DeckNameSelect.ItemCount - 1);
		}

		private void ClearDeck()
		{
			foreach (var card in currentDeckCtrls.ToArray()) card.Delete();

			AvatarController.Clear();
			
			if (currentDeckCtrls.Count > 0)
			{
				GD.PrintErr("Didn't delete all cards succesfully!?");
				currentDeckCtrls.Clear();
			}
		}

		private void SaveDeck()
		{
			if (currentDeck == null) return;
			DeckAccess.Save(currentDeck);
		}

		public void DeleteSelectedDeck()
		{
			if (currentDeck == null) return;

			int indexToSelect;
			if (currentDeck.deckName == null)
			{
				GD.PushWarning("Deleted deck with no name!?");
				indexToSelect = 0;
			}
			else
			{
				DeckAccess.Delete(currentDeck);
				int deckIndex = deckNames.IndexOf(currentDeck.deckName);
				DeckNameSelect.RemoveItem(deckIndex);
				deckNames.RemoveAt(deckIndex);
				indexToSelect = deckIndex == 0 ? 0 : deckIndex - 1;
			}

			//TODO handle deleting last deck. maybe force user onto the "create first deck" code path?
			//TODO create first deck code path that's new deck but you can't cancel out (forcing you to have at least 1 deck)
			LoadDeck(indexToSelect);
			DeckNameSelect.Select(indexToSelect);
		}

		public void LoadDeck(int index)
		{
			if (deckNames.Count <= index) return;
			LoadDeck(deckNames[index]);
		}

		private void LoadDeck(string deckName)
		{
			var decklist = DeckAccess.Load(deckName);
			if (decklist == null)
			{
				GD.PrintErr($"Failed to load deck {deckName} in deck edit");
				return;
			}

			ClearDeck();
			
			if (decklist.avatarName != null)
			{
				var avatar = DeckBuilderCardRepository.CreateDeckBuilderCard(decklist.avatarName);
				AvatarController.UpdateAvatar(avatar);
			}

			if (decklist.deck != null)
				foreach (string cardName in decklist.deck) AddToDeck(cardName);

			currentDeck = decklist;
		}

		private void AddToDeck(string cardName)
		{
			var card = DeckBuilderCardRepository.CreateDeckBuilderCard(cardName);
			AddToDeck(card);
		}

		public void AddToDeck(DeckBuilderCard card)
		{
			_ = currentDeck ?? throw new InvalidOperationException("Tried to add card to a null deck!");

			var ctrl = CreateCardController();
			DeckNodesParent.AddChild(ctrl);
			//DeckNodesParent.MoveChild(ctrl, -1 - DeckSpacingPlaceholders.Length);
			ctrl.Init(card, DeckBuilderController.CardView, this);
			currentDeck.deck.Add(card.CardName); //It's ok that we add to the decklist before replacing it in LoadDeck because it just gets garbage collected
			currentDeckCtrls.Add(ctrl);
		}

		public void RemoveFromDeck(DeckBuilderDeckCardController card)
		{
			int index = currentDeckCtrls.IndexOf(card);
			if (index < 0) return;

			DeckNodesParent.RemoveChild(card);
			currentDeck?.deck.RemoveAt(index);
			currentDeckCtrls.RemoveAt(index);
		}

		public void BecomeAvatar(DeckBuilderCardController cardCtrl)
		{
			_ = currentDeck ?? throw new InvalidOperationException("Tried to become avatar of a null deck!");

			var card = cardCtrl.Card ?? throw new InvalidOperationException("Card control had no card!");
			AvatarController.UpdateAvatar(card);
			currentDeck.avatarName = card.CardName;
		}

		public void DragSwap(DeckBuilderDeckCardController card)
		{
			if (Dragging == null) return;
			_ = Dragging.Card ?? throw new InvalidOperationException("Drag swapping a card ctrl with no card!");
			_ = currentDeck ?? throw new InvalidOperationException("Can't drag card through a null deck!");

			int argIndex = currentDeckCtrls.IndexOf(card);
			if (argIndex < 0) { GD.Print($"{card} not in deck"); return; }

			int draggingIndex = currentDeckCtrls.IndexOf(Dragging);

			if (!currentDeckCtrls.Remove(Dragging)) return;
			currentDeckCtrls.Insert(argIndex, Dragging);

			currentDeck.deck.RemoveAt(draggingIndex);
			currentDeck.deck.Insert(argIndex, Dragging.Card.CardName);

			DeckNodesParent.MoveChild(Dragging, argIndex);
		}

		private DeckBuilderDeckCardController CreateCardController()
		{
			if (DeckCardControllerPrefab.Instantiate() is not DeckBuilderDeckCardController controller)
					throw new System.ArgumentNullException(nameof(DeckCardControllerPrefab), "Was not the right type");
			return controller;
		}

		private void AddDeckName(string deckName)
		{
			deckNames.Add(deckName);
			DeckNameSelect.AddItem(deckName);
		}
	}
}