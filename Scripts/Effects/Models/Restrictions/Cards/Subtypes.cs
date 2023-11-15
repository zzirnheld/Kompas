using System.Linq;
using Kompas.Cards.Models;
using Newtonsoft.Json;

namespace Kompas.Effects.Models.Restrictions.Cards
{
	public class Subtypes : CardRestrictionBase
	{
		#nullable disable
		[JsonProperty(Required = Required.Always)]
		public string[] subtypes;
		#nullable restore

		[JsonProperty]
		public bool exclude = false; //default to include
		[JsonProperty]
		public bool any = false; //default to all

		[JsonProperty]
		public bool spell = false; //whether to consider all or spell subtypes

		public override void Initialize(EffectInitializationContext initializationContext)
		{
			base.Initialize(initializationContext);
			if (subtypes == null) throw new System.ArgumentNullException("subtypes");
		}

		protected override bool IsValidLogic(IGameCardInfo card, IResolutionContext context)
		{
			bool HasSubtype(string subtype) => spell ? card.SpellSubtypes.Contains(subtype) : card.HasSubtype(subtype);
			bool includes = any
				? subtypes.Any(HasSubtype)
				: subtypes.All(HasSubtype);
			//If you're excluding all subtypes (exclude = true) you want all subtypes to not be present on the card (all = false)
			//If you're including all subtypes (exclude = false) you want all subtypes to be present on the card (all = true)
			return includes != exclude;
		}
	}
}