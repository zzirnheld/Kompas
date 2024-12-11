using Kompas.Effects.Models;

namespace Kompas.Gamestate.Exceptions;

public class EffectNotResolvingException : KompasException
{
	public EffectNotResolvingException(Effect effect, string? debugMessage = null, string? message = null)
		: base(debugMessage ?? $"{effect} not resolving", $"{effect} not resolving")
	{ }
}