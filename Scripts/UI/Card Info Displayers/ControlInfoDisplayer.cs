using System;
using Godot;
using Kompas.Cards.Loading;
using Kompas.Cards.Models;
using Kompas.Shared.Exceptions;
using Kompas.UI.TextBehavior;

namespace Kompas.UI.CardInfoDisplayers
{
	public partial class ControlInfoDisplayer : Control, ICardInfoDisplayer
	{
		[Export]
		private TextureRect? _image;
		private TextureRect Image => _image
			?? throw new UnassignedReferenceException();
		[Export]
		private Label? _n;
		private Label N => _n
			?? throw new UnassignedReferenceException();
		[Export]
		private Label? _e;
		private Label E => _e
			?? throw new UnassignedReferenceException();
		[Export]
		private Label? _cost;
		private Label Cost => _cost
			?? throw new UnassignedReferenceException();
		[Export]
		private Label? _w;
		private Label W => _w
			?? throw new UnassignedReferenceException();
		[Export]
		private ShrinkOnOverrun? _cardName;
		private ShrinkOnOverrun CardName => _cardName
			?? throw new UnassignedReferenceException();
		[Export]
		private ShrinkOnOverrun? _subtypes;
		private ShrinkOnOverrun Subtypes => _subtypes
			?? throw new UnassignedReferenceException();
		[Export]
		private ShrinkRichTextOnOverrun? _effText;
		private ShrinkRichTextOnOverrun EffText => _effText
			?? throw new UnassignedReferenceException();

		//Consider moving these to a subclass if I ever want to 
		[Export]
		private TextureRect? _frameImage;
		private TextureRect FrameImage => _frameImage
			?? throw new UnassignedReferenceException();

		public event EventHandler<string>? HoverKeyword;
		public event EventHandler<string>? StopHoverKeyword;

		public bool ShowingInfo
		{
			set => Visible = value;
		}

		public override void _Ready()
		{
			base._Ready();
			EffText.MetaHoverStarted += keyword =>
			{
				if (keyword.VariantType != Variant.Type.String) throw new System.InvalidOperationException("Can't have a non-string keyword!");
				HoverKeyword?.Invoke(this, (string)keyword);
			};
			EffText.MetaHoverEnded += keyword =>
			{
				if (keyword.VariantType != Variant.Type.String) throw new System.InvalidOperationException("Can't have a non-string keyword!");
				StopHoverKeyword?.Invoke(this, (string)keyword);
			};
		}

		public void DisplayCardImage(CardBase card)
		{
			Image.Texture = card.CardFaceImage;
			FrameImage.Texture = card.CardType == 'C' ? CardRepository.CharCardFrameTexture : CardRepository.NoncharCardFrameTexture;
		}

		public void DisplayCardNumericStats(CardBase card)
		{
			N.Text = $"{card.N}";
			E.Text = $"{card.E}";
			Cost.Text = $"{card.Cost}";
			W.Text = $"{card.W}";
		}

		public void DisplayCardRulesText(CardBase card)
		{
			CardName.ShrinkableText = card.CardName;
			Subtypes.ShrinkableText = card.SubtypeText;
			EffText.ShrinkableText = card.BBCodeEffText;
		}
	}
}