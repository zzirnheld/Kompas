using Kompas.Cards.Models;
using Kompas.Gamestate.Exceptions;
using Newtonsoft.Json;

namespace Kompas.Effects.Models.Identities.Cards
{
	public class OtherInFight : ContextualParentIdentityBase<IGameCardInfo>
	{
		#nullable disable
		[JsonProperty(Required = Required.Always)]
		public IIdentity<IGameCardInfo> other;
		#nullable restore

		public override void Initialize(EffectInitializationContext initializationContext)
		{
			base.Initialize(initializationContext);
			other.Initialize(initializationContext);
		}

		protected override IGameCardInfo AbstractItemFrom(IResolutionContext? context, IResolutionContext? secondaryContext)
		{
			Attack attack = GetAttack(ContextToConsider(context, secondaryContext).TriggerContext);
			var otherCard = other.From(context, secondaryContext);

			if (attack.attacker == otherCard) return attack.defender;
			if (attack.defender == otherCard) return attack.attacker;

			throw new NullCardException($"Neither card of attack {attack} was {otherCard}");
		}
	}
}