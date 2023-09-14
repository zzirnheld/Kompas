using System.Collections.Generic;
using Kompas.Cards.Models;

namespace Kompas.Effects.Models.Identities
{
	public class IdentityOverrides
	{
		private readonly Stack<GameCardBase> targetOverrides = new();
		public GameCardBase TargetCardOverride => targetOverrides.Count > 0 ? targetOverrides.Peek() : null;

		public T WithTargetCardOverride<T> (GameCardBase targetOverride, System.Func<T> action)
		{
			targetOverrides.Push(targetOverride);
			var t = action();
			targetOverrides.Pop();
			return t;
		}
	}
}