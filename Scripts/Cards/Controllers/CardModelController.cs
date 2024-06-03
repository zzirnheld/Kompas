using Godot;
using Kompas.Cards.Views;
using Kompas.Shared.Exceptions;
using System;

namespace Kompas.Cards.Controllers
{
	public partial class CardModelController : Node
	{
		[Export]
		private Zoomable3DCardInfoDisplayer? _infoDisplayer;
		public Zoomable3DCardInfoDisplayer InfoDisplayer => _infoDisplayer
			?? throw new UnassignedReferenceException();

		[Export]
		private CardMouseController? _mouseController;
		public CardMouseController MouseController => _mouseController
			?? throw new UnassignedReferenceException();
	}
}