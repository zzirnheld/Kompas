using System.Collections.Generic;
using Kompas.Effects.Models.Client;
using Kompas.Effects.Models;

namespace Kompas.Cards.Models.Client
{
	public class ClientSerializableCard : SerializableGameCard
	{
		public ClientEffect[] effects;

		public override IEnumerable<Effect> Effects => effects;
	}
}