using Kompas.Cards.Models;
using Kompas.Effects.Models.Identities;
using Newtonsoft.Json;
using System.Linq;

namespace Kompas.Effects.Models.Restrictions.Cards
{
	public class Fighting : CardRestrictionBase
	{
		/// <summary>
		/// Can be null to represent checking whether the card is in any fight at all
		/// </summary>
		[JsonProperty] //DisallowNull means the json can't specify null
		public IIdentity<IGameCardInfo> fightingWho;
		/// <summary>
		/// Whether the character must be the defender in the fight in question
		/// </summary>
		[JsonProperty]
		public bool defending = false;
		/// <summary>
		/// Whether the character must be the attacker in the fight in question
		/// </summary>
		[JsonProperty]
		public bool attacking = false;

		public override void Initialize(EffectInitializationContext initializationContext)
		{
			base.Initialize(initializationContext);
			fightingWho?.Initialize(initializationContext);

			if (attacking && defending)
				throw new System.ArgumentException("Can't require a card to be attacking and defending using the same Fighting object");
		}

		private bool IsValidFight(IGameCardInfo card, IResolutionContext context, IStackable stackEntry)
		{
			if (!(stackEntry is Attack attack)) return false;

			//If need to be attacking or defending, check those
			if (attacking)
			{
				if (attack.attacker != card) return false;
			}
			else if (defending)
			{
				if (attack.defender != card) return false;
			}
			//Otherwise, just make sure the character is in the fight at all. If it's neither the attacker nor defender, it's not in the fight
			else if (attack.attacker != card && attack.defender != card) return false;

			//And if that card is in the fight, make sure any other card that needs to be in the fight, is in the fight.
			if (fightingWho == null) return true;

			var fightingWhoCard = fightingWho.From(context);
			return attack.attacker == fightingWhoCard || attack.defender == fightingWhoCard;
		}

		protected override bool IsValidLogic(IGameCardInfo card, IResolutionContext context)
			=> InitializationContext.game.StackController.StackEntries.Any(stackEntry => IsValidFight(card, context, stackEntry));
	}
}