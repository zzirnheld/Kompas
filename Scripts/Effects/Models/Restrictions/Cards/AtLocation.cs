using System.Collections.Generic;
using System.Linq;
using Kompas.Cards.Models;
using Kompas.Gamestate.Locations;
using Newtonsoft.Json;

namespace Kompas.Effects.Models.Restrictions.Cards
{
	public class AtLocation : CardRestrictionBase
	{
		#nullable disable
		[JsonProperty(Required = Required.Always)]
		public string[] locations;

		public AtLocation() { }
		#nullable restore
		public AtLocation(Location location)
		{
			locations = new string[] { LocationHelpers.StringVersion(location) };
		}

		protected IReadOnlyCollection<Location> Locations => locations.Select(LocationHelpers.FromString).ToArray();

		public override void Initialize(InitializationContext initializationContext)
		{
			base.Initialize(initializationContext);
			if (locations == null) throw new System.ArgumentNullException("locations");
		}

		protected override bool IsValidLogic(IGameCardInfo? card, IResolutionContext context)
			=> card != null && Locations.Any(loc => card.Location == loc);

		public override string ToString() => $"must be in {string.Join(", ", Locations)}";
	}
}