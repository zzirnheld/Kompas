using System.Collections.Generic;
using Godot;

namespace Kompas.Effects.Models
{
	/// <summary>
	/// NOTE: Never edit a link after creating it. Delete it and create a new one.
	/// Otherwise, the client won't know the difference between two links of the same cards, from the same effects,
	/// which is a scenario I want to allow. (Two activations of the same card's effect linking the same cards a second time over)
	/// </summary>
	public class CardLink
	{
		public static readonly Color DefaultColor = new ("c300c3", 0.8f);

		public HashSet<int> CardIDs { get; }
		public IEffect LinkingEffect { get; }
		public Color LinkColor { get; }

		public CardLink(HashSet<int> cardIDs, IEffect linkingEffect, Color linkColor)
		{
			CardIDs = cardIDs;
			LinkingEffect = linkingEffect;
			LinkColor = linkColor;
		}

		public bool Matches(IEnumerable<int> cardIDs, IEffect linkingEffect)
		{
			return LinkingEffect == linkingEffect && CardIDs.SetEquals(cardIDs);
		}
	}
}