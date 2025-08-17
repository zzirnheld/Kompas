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
	//TODO move these to constructor
	private IPlayer? owningPlayer;
	public override IPlayer OwningPlayer => owningPlayer ?? throw new System.NullReferenceException("Tried to get owning player of uninitialized effect");

	private ClientGameCard? card;
	public override GameCard Card => card ?? throw new System.NullReferenceException("Tried to get card of uninitialized effect");

	private ClientGame? _clientGame;
	public ClientGame ClientGame
	{
		get => _clientGame ?? throw new NotInitializedException();
		private set => _clientGame = value;
	}

	public override Trigger? Trigger => ClientTrigger;
	public ClientTrigger? ClientTrigger { get; private set; }
	public override IGame Game => ClientGame;

	public DummySubeffect[] DummySubeffects { get; } = System.Array.Empty<DummySubeffect>();
	public override Subeffect[] Subeffects => DummySubeffects;

	//TODO controller? should have some way to track it client-side otherwise if effects ever can be activated by not the card's ocntroller something will break

	public ClientEffect(EffectData data) : base(data)
	{
		if (data.triggerData != null && !string.IsNullOrEmpty(data.triggerData.triggerCondition))
			ClientTrigger = new ClientTrigger(data.triggerData, this);
	}

	public void SetInfo(ClientGameCard card, ClientGame clientGame, int effectIndex, IPlayer owningPlayer)
	{
		this.card = card;
		ClientGame = clientGame;
		this.owningPlayer = owningPlayer;
		base.SetInfo(effectIndex);
	}

	//TODO eventually make client aware of activation contexts
	public void IncrementUses()
	{
		TimesUsedThisTurn++;
		TimesUsedThisRound++;
		TimesUsedThisStack++;
	}
}