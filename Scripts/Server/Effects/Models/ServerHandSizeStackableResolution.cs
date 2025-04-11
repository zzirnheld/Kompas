using System.Linq;
using System.Threading.Tasks;
using Godot;
using Kompas.Cards.Models;
using Kompas.Cards.Movement;
using Kompas.Effects.Models;
using Kompas.Effects.Models.Restrictions;
using Kompas.Effects.Models.TriggeringEvent;
using Kompas.Server.Gamestate;
using Kompas.Shared.Enumerable;

namespace Kompas.Server.Effects.Models;

public class ServerHandSizeStackableResolution
    : StackableResolution<ServerHandSizeStackable, IServerResolutionContext>,
        IServerStackableResolution<ServerHandSizeStackable>
{
	private readonly ServerGame serverGame;
	
	private bool awaitingChoices;

    public ServerHandSizeStackableResolution(ServerHandSizeStackable stackable, IServerResolutionContext context, ServerGame serverGame)
        : base(stackable, context)
    {
        this.serverGame = serverGame;
    }

	public void Declare() { }

    public async Task StartResolution() => await RequestTargets();

	private async Task RequestTargets()
	{
		Logger.Log("Trying to request hand size targets");
		awaitingChoices = true;

		var context = new ResolutionContext(new EventContext() { StackableCause = Stackable, StackableEvent = Stackable }, Stackable.InitialBlurb);
		int[] cardIds = serverGame.Cards
			.Where(c => Stackable.HandSizeCardRestriction.IsValid(c, context))
			.Select(c => c.ID)
			.ToArray();

		var player = Stackable.ControllingPlayer ?? throw new System.InvalidOperationException();
		int overHandSize = cardIds.Length - player.HandSizeLimit;
		if (overHandSize <= 0)
		{
			awaitingChoices = false;
			return;
		}

		var listRestriction = IListRestriction.ConstantCount(overHandSize);
		listRestriction.Initialize(new InitializationContext(serverGame, source: null));
		string listRestrictionJson = listRestriction.SerializeToJSON(context);

		int[]? choices = null;
		while (!TryAnswer(choices))
		{
			choices = await serverGame.Awaiter.GetHandSizeChoices(player, cardIds, listRestrictionJson);
		}
	}

	public bool TryAnswer(int[]? cardIds)
	{
		if (!awaitingChoices) return false;
		if (cardIds == null) return false;

		GameCard[] cards = cardIds
			.Distinct()
			.Select(i => serverGame.LookupCardByID(i))
			.NonNull()
			.ToArray();

		int count = cards.Length;
		var player = Stackable.ControllingPlayer ?? throw new System.InvalidOperationException();
		var context = new ResolutionContext(new EventContext() { StackableCause = Stackable, StackableEvent = Stackable }, Stackable.InitialBlurb);
		int correctCount = serverGame.Cards.Count(c => Stackable.HandSizeCardRestriction.IsValid(c, context)) - player.HandSizeLimit;

		if (count != correctCount || cards.Any(c => !Stackable.HandSizeCardRestriction.IsValid(c, context))) return false;

		foreach (var card in cards) card.Reshuffle();
		awaitingChoices = false;
		return true;
	}
}
