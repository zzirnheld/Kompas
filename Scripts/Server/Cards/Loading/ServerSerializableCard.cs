using System.Collections.Generic;
using Kompas.Cards.Models;
using Kompas.Effects.Models;
using Kompas.Server.Effects.Models;

public class ServerSerializableCard : SerializableGameCard
{
	public ServerEffect[]? effects;

	public override IEnumerable<Effect>? Effects => effects;
}