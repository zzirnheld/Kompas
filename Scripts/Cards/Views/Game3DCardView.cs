using Godot;
using Kompas.Cards.Models;
using Kompas.UI.CardInfoDisplayers;
using System;

namespace Kompas.Cards.Views
{
	public partial class Game3DCardView : CardViewBase<GameCard, Zoomable3DCardInfoDisplayer>
	{
		public Game3DCardView(Zoomable3DCardInfoDisplayer infoDisplayer) : base(infoDisplayer)
		{
		}
	}
}