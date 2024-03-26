
using Kompas.Client.Gamestate;
using Kompas.Networking.Packets;
using System.Collections.Generic;
using System.Linq;
using Godot;
using Kompas.Effects.Models;
using Kompas.Shared.Enumerable;
using System;

namespace Kompas.Networking.Packets
{
	public class EditCardLinkPacket : Packet
	{
		public int[]? linkedCardsIDs;

		public int effIndex;
		public int whoseEffectID;

		public bool add; //true for add, false for remove

		public Color linkColor;

		public EditCardLinkPacket() : base(EditCardLink) { }

		public EditCardLinkPacket(int[] linkedCardsIDs, int effIndex, int whoseEffectID, bool add, Color linkColor) : this()
		{
			this.linkedCardsIDs = linkedCardsIDs;
			this.effIndex = effIndex;
			this.whoseEffectID = whoseEffectID;
			this.add = add;
			this.linkColor = linkColor;
		}

		public EditCardLinkPacket(IEnumerable<int> cardIDs, IEffect eff, bool add)
			: this(cardIDs.ToArray(), eff.EffectIndex, eff.Card?.ID ?? throw new InvalidOperationException("Eff with no card"),
				add, CardLink.DefaultColor)
		{ }

		public EditCardLinkPacket(CardLink cardLink, bool add)
			: this(cardLink.CardIDs, cardLink.LinkingEffect, add)
		{ }

		public override Packet Copy() => new EditCardLinkPacket()
		{
			linkedCardsIDs = linkedCardsIDs,
			effIndex = effIndex,
			whoseEffectID = whoseEffectID,
			add = add,
			linkColor = linkColor
		};
	}
}

namespace Kompas.Client.Networking
{
	public class EditCardLinkClientPacket : EditCardLinkPacket, IClientOrderPacket
	{
		public void Execute(ClientGame clientGame)
		{
			if (linkedCardsIDs == null)
			{
				Logger.Warn("null card ids or color for link");
				return;
			}

			var effect = clientGame.LookupCardByID(whoseEffectID)?.Effects.ElementAt(effIndex);
			var cards = linkedCardsIDs.Select(clientGame.LookupCardByID).NonNull();

			if (effect == default || !cards.Any()) throw new System.ArgumentException($"Bad edit card args {linkedCardsIDs}, {effIndex}, {whoseEffectID}");
			var linkHandler = cards.First().CardLinkHandler;

			if (add) linkHandler.CreateLink(linkedCardsIDs, effect, linkColor);
			else linkHandler.RemoveEquivalentLink(linkedCardsIDs, effect);
		}
	}
}