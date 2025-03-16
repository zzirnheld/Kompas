using System;
using System.Threading.Tasks;
using Kompas.Gamestate.Exceptions;
using Newtonsoft.Json;

namespace Kompas.Server.Effects.Models.Subeffects;

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

	private async Task<int> AskForOptionChoice(IServerResolutionContext context)
	{
		var player = GetPlayerTarget(context) ?? throw new NullPlayerException(TargetWasNull);
		return await ServerGame.Awaiter
			.GetEffectOption(player,
							cardName: Effect.Card.CardName,
							choiceBlurb: choiceBlurb,
							optionBlurbs: optionBlurbs,
							hasDefault: hasDefault,
							showX: showX,
							x: context.X);
	}

	public override async Task<ResolutionInfo> Resolve(ServerEffectResolution resolution)
	{
		int choice = -1;
		_ = jumpIndices ?? throw new IllDefinedException();
		while (choice < 0 || choice >= jumpIndices.Length)
		{
			choice = await AskForOptionChoice(resolution.Context);
		}

		return ResolutionInfo.Index(jumpIndices[choice]);
	}
}
