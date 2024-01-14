using System.Collections.Generic;
using Kompas.Cards.Models;
using Kompas.Client.Effects.Models;
using Kompas.Effects.Models;

namespace Kompas.Client.Cards.Models
{
	public class ClientSerializableCard : SerializableGameCard
	{
		public ClientEffect[]? effects;

		public override IEnumerable<Effect>? Effects => effects;
	}
}