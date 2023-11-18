using Godot;
using System;

namespace Kompas.Client.UI
{
	public partial class PlayerInfoController : Node
	{
		[Export]
		private TextureRect AvatarImage { get; set; }

		[Export]
		private Label Pips { get; set; }

		[Export]
		private Label NextTurnPips { get; set; }

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