using Godot;
using Kompas.Client.Effects.Models;
using Kompas.Shared.Exceptions;

namespace Kompas.Client.Effects.Views
{
	public partial class ClientAttackStackableView
		: ClientStackableView
	{
		[Export]
		private TextureRect? _primaryCardImage;
		private TextureRect PrimaryCardImage => _primaryCardImage
			?? throw new UnassignedReferenceException(nameof(_primaryCardImage));

		[Export]
		private TextureRect? _secondaryCardImage;
		private TextureRect SecondaryCardImage => _secondaryCardImage
			?? throw new UnassignedReferenceException(nameof(_secondaryCardImage));

		private ClientAttack? attack;

		public void Initialize(ClientAttack attack)
		{
			if (this.attack != null) throw new AlreadyInitializedException();
			this.attack = attack;

			PrimaryCardImage.Texture = attack.attacker.CardFaceImage;
			SecondaryCardImage.Texture = attack.defender.CardFaceImage;
		}
	}
}