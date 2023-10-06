
using Kompas.Client.Gamestate;
using Kompas.Cards.Models;
using KompasCore.Effects;
using Kompas.Networking.Packets;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Kompas.Networking.Packets
{
	public class EditCardLinkPacket : Packet
	{
		public int[] linkedCardsIDs;

		public int effIndex;
		public int whoseEffectID;

		public bool add; //true for add, false for remove

		public Color32 linkColor;

		public EditCardLinkPacket() : base(EditCardLink) { }

		public EditCardLinkPacket(int[] linkedCardsIDs, int effIndex, int whoseEffectID, bool add, Color32 linkColor) : this()
		{
			this.linkedCardsIDs = linkedCardsIDs;
			this.effIndex = effIndex;
			this.whoseEffectID = whoseEffectID;
			this.add = add;
			this.linkColor = linkColor;
		}

		public EditCardLinkPacket(IEnumerable<int> cardIDs, Effect eff, bool add)
			: this(cardIDs.ToArray(), eff.EffectIndex, eff.Source.ID, add, CardLink.DefaultColor)
		{ }

		public EditCardLinkPacket(CardLink cardLink, bool add)
			: this(cardLink.CardIDs, cardLink.LinkingEffect, add)
		{ }

		public override Packet Copy() => new EditCardLinkPacket(linkedCardsIDs, effIndex, whoseEffectID, add, linkColor);
	}
}

namespace Kompas.Client.Networking
{
	public class EditCardLinkClientPacket : EditCardLinkPacket, IClientOrderPacket
	{
		public void Execute(ClientGame clientGame)
		{
			var effect = clientGame.LookupCardByID(whoseEffectID)?.Effects.ElementAt(effIndex);
			var cards = linkedCardsIDs.Select(clientGame.LookupCardByID).Where(c => c != null);

			if (effect == default || cards.Count() == 0) throw new System.ArgumentException($"Bad edit card args {linkedCardsIDs}, {effIndex}, {whoseEffectID}");
			var linkHandler = cards.First().CardLinkHandler;

			if (add) linkHandler.CreateLink(linkedCardsIDs, effect, linkColor);
			else linkHandler.RemoveEquivalentLink(linkedCardsIDs, effect);
		}
	}
}