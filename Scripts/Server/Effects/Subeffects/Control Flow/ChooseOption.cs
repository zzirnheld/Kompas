using System;
using System.Threading.Tasks;
using Kompas.Gamestate.Exceptions;
using Newtonsoft.Json;

namespace Kompas.Server.Effects.Models.Subeffects
{
	public class ChooseOption : ServerSubeffect
	{
		[JsonProperty]
		public string choiceBlurb = string.Empty;
		[JsonProperty]
		public string[] optionBlurbs = Array.Empty<string>();
		[JsonProperty]
		public bool hasDefault = true;
		[JsonProperty]
		public bool showX = false;

		private async Task<int> AskForOptionChoice()
		{
			var player = PlayerTarget ?? throw new NullPlayerException(TargetWasNull);
			return await ServerGame.Awaiter
				.GetEffectOption(PlayerTarget,
								cardName: Effect.Card.CardName,
								choiceBlurb: choiceBlurb,
								optionBlurbs: optionBlurbs,
								hasDefault: hasDefault,
								showX: showX,
								x: Effect.X);
		}

		public override async Task<ResolutionInfo> Resolve()
		{
			int choice = -1;
			_ = jumpIndices ?? throw new IllDefinedException();
			while (choice < 0 || choice >= jumpIndices.Length)
			{
				choice = await AskForOptionChoice();
			}

			return ResolutionInfo.Index(jumpIndices[choice]);
		}
	}
}
