﻿using System.Threading.Tasks;

namespace Kompas.Server.Effects.Models.Subeffects
{
	public class ChooseOption : ServerSubeffect
	{
		public string choiceBlurb;
		public string[] optionBlurbs;
		public bool hasDefault = true;
		public bool showX = false;

		private async Task<int> AskForOptionChoice()
			=> await ServerGame.Awaiter
				.GetEffectOption(PlayerTarget,
								 cardName: Card.CardName,
								 choiceBlurb: choiceBlurb,
								 optionBlurbs: optionBlurbs,
								 hasDefault: hasDefault,
								 showX: showX,
								 x: Effect.X);

		public override async Task<ResolutionInfo> Resolve()
		{
			int choice = -1;
			while (choice < 0 || choice >= jumpIndices.Length)
			{
				choice = await AskForOptionChoice();
			}

			return ResolutionInfo.Index(jumpIndices[choice]);
		}
	}
}