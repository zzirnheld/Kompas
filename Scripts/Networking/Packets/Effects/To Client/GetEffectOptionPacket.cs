using Kompas.Networking.Packets;
using Kompas.Client.Gamestate;

namespace Kompas.Networking.Packets
{
	public class GetEffectOptionPacket : Packet
	{
		public string? cardName;
		public string? choiceBlurb;
		public string[]? optionBlurbs;
		public bool hasDefault;
		public bool showX;
		public int x;

		public GetEffectOptionPacket() : base(GetEffectOption) { }

		public GetEffectOptionPacket(string cardName, string choiceBlurb, string[] optionBlurbs, bool hasDefault, bool showX, int x) : this()
		{
			this.cardName = cardName;
			this.choiceBlurb = choiceBlurb;
			this.optionBlurbs = optionBlurbs;
			this.hasDefault = hasDefault;
			this.x = x;
			this.showX = showX;
		}

		public override Packet Copy() => new GetEffectOptionPacket()
		{
			cardName = cardName,
			choiceBlurb = choiceBlurb,
			optionBlurbs = optionBlurbs,
			hasDefault = hasDefault,
			showX = showX,
			x = x
		};
	}
}

namespace Kompas.Client.Networking
{
	public class GetEffectOptionClientPacket : GetEffectOptionPacket, IClientOrderPacket
	{
		public void Execute(ClientGame clientGame)
		{
			throw new System.NotImplementedException();
			/*
			if (hasDefault && clientGame.clientUIController.effectsUIController.OptionalEffAutoResponse == EffectsUIController.OptionalEffYes)
				clientGame.clientNotifier.RequestChooseEffectOption(0);
			else if (hasDefault && clientGame.clientUIController.effectsUIController.OptionalEffAutoResponse == EffectsUIController.OptionalEffNo)
				clientGame.clientNotifier.RequestChooseEffectOption(1);
			else clientGame.clientUIController.effectsUIController.ShowEffectOptions(choiceBlurb, optionBlurbs, showX, x);
			*/
		}
	}
}