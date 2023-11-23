﻿using Kompas.Effects.Models.Restrictions;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace Kompas.Server.Effects.Models.Subeffects
{
	public class PlayerChooseX : ServerSubeffect
	{
		#nullable disable
		[JsonProperty(Required = Required.Always)]
		public IRestriction<int> XRest;
		#nullable restore

		public override void Initialize(ServerEffect eff, int subeffIndex)
		{
			base.Initialize(eff, subeffIndex);
			XRest.Initialize(DefaultInitializationContext);
		}

		private async Task<int> AskForX() => await ServerGame.Awaiter.GetPlayerXValue(PlayerTarget);

		public override async Task<ResolutionInfo> Resolve()
		{
			bool xLegal = false;
			while (!xLegal)
			{
				int x = await AskForX();
				xLegal = SetXIfLegal(x);
			}
			return ResolutionInfo.Next;
		}

		public bool SetXIfLegal(int x)
		{
			if (XRest.IsValid(x, ResolutionContext))
			{
				ServerEffect.X = x;
				return true;
			}
			return false;
		}
	}
}