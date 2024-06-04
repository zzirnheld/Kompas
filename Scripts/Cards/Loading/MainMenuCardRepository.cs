
namespace Kompas.Cards.Loading
{
	public class MainMenuCardRepository : CardRepository
	{
		public MainMenuCardRepository()
			: base(IFileLoader.Godot, true)
		{
		}

		public new void Initialize() => base.Initialize();
	}
}