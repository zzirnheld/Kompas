using Kompas.Cards.Models;
using Kompas.Client.Cards.Models;
using Kompas.Client.Gamestate;
using Kompas.Effects.Models;
using Kompas.Effects.Subeffects;
using Kompas.Gamestate;
using Kompas.Gamestate.Players;
using Kompas.Shared.Exceptions;

namespace Kompas.Client.Effects.Models;

//TODO refactor into serialized - unserialized thing? Or reformat like Identities?
public class ClientEffect : Effect, IClientStackable
{
	// Initialized in constructor
	private readonly TriggerData? triggerData;
	private readonly IPlayer owningPlayer;

	public ClientGame ClientGame { get; }

	public override Trigger? Trigger => ClientTrigger;
	public ClientTrigger? ClientTrigger { get; private set; }
	public override IGame Game => ClientGame;

	public DummySubeffect[] DummySubeffects { get; } = System.Array.Empty<DummySubeffect>();
	public override Subeffect[] Subeffects => DummySubeffects;
	public override IPlayer OwningPlayer => owningPlayer;

	//TODO controller? should have some way to track it client-side otherwise if effects ever can be activated by not the card's ocntroller something will break

	public ClientEffect(EffectData data, ClientGame clientGame, IPlayer owningPlayer) : base(data)
	{
		triggerData = data.triggerData;
		ClientGame = clientGame;
		this.owningPlayer = owningPlayer;
	}

	public void SetInfo(ClientGameCard card, int effectIndex)
	{
		base.SetInfo(card, effectIndex);
		if (triggerData != null && !string.IsNullOrEmpty(triggerData.triggerCondition))
			ClientTrigger = new ClientTrigger(triggerData, this);
	}

	//TODO eventually make client aware of activation contexts
	public void IncrementUses()
	{
		TimesUsedThisTurn++;
		TimesUsedThisRound++;
		TimesUsedThisStack++;
	}
}