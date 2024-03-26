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

		public override void Initialize(InitializationContext initializationContext)
		{
			base.Initialize(initializationContext);
			other.Initialize(initializationContext);
		}

		protected override IGameCardInfo? AbstractItemFrom(IResolutionContext context, IResolutionContext secondaryContext)
		{
			var triggeringContext = ContextToConsider(context, secondaryContext)?.TriggerContext
				?? throw new IllDefinedException();
			Attack attack = GetAttack(triggeringContext);
			var otherCard = other.From(context, secondaryContext);

			if (attack.attacker == otherCard) return attack.defender;
			if (attack.defender == otherCard) return attack.attacker;

			throw new NullCardException($"Neither card of attack {attack} was {otherCard}");
		}
	}
}