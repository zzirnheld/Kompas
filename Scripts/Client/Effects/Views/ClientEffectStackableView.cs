using Godot;
using Kompas.Client.Effects.Models;
using Kompas.Effects.Models;
using Kompas.Shared.Exceptions;

namespace Kompas.Client.Effects.Views;

public partial class ClientEffectStackableView
	: ClientStackableView
{
	[Export]
	private TextureRect? _primaryCardImage;
	private TextureRect PrimaryCardImage => _primaryCardImage
		?? throw new UnassignedReferenceException(nameof(_primaryCardImage), this);

	[Export]
	private Label? _effectBlurbLabel;
	private Label EffectBlurbLabel => _effectBlurbLabel
		?? throw new UnassignedReferenceException(nameof(_effectBlurbLabel), this);

	[Export]
	private TextureRect? _primaryCardImageLarge;
	private TextureRect PrimaryCardImageLarge => _primaryCardImageLarge
		?? throw new UnassignedReferenceException(nameof(_primaryCardImageLarge), this);

	[Export]
	private Label? _effectBlurbLabelLarge;
	private Label EffectBlurbLabelLarge => _effectBlurbLabelLarge
		?? throw new UnassignedReferenceException(nameof(_effectBlurbLabelLarge), this);

	private ClientEffect? effect;

	public void Initialize(IStackableResolution<ClientEffect> effectResolution)
	{
		if (effect != null) throw new AlreadyInitializedException();
		effect = effectResolution.Stackable;

		PrimaryCardImage.Texture = effect.Card.CardFaceImage;
		EffectBlurbLabel.Text = effectResolution.Context.Blurb;

		PrimaryCardImageLarge.Texture = effect.Card.CardFaceImage;
		EffectBlurbLabelLarge.Text = effectResolution.Context.Blurb;
	}
}