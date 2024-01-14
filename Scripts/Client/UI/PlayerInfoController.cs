using Godot;
using Kompas.Shared.Exceptions;
using System;

namespace Kompas.Client.UI
{
	public partial class PlayerInfoController : Node
	{
		[Export]
		private TextureRect? _avatarImage;
		private TextureRect AvatarImage => _avatarImage
			?? throw new UnassignedReferenceException();

		[Export]
		private Label? _pips;
		private Label Pips => _pips
			?? throw new UnassignedReferenceException();

		[Export]
		private Label? _nextTurnPips;
		private Label NextTurnPips => _nextTurnPips
			?? throw new UnassignedReferenceException();

		public Texture2D? AvatarTexture
		{
			set => AvatarImage.Texture = value;
		}

		public int PipsCount
		{
			set => Pips.Text = $"{value}";
		}

		public int NextTurnPipsCount
		{
			set => NextTurnPips.Text = $"+({value})";
		}
	}
}