
namespace Kompas.Cards.Loading;

public class MainMenuCardRepository : CardRepository
{
	public MainMenuCardRepository()
		: base(IFileLoader.Godot, true)
	{
	}

	public static void Load() => new MainMenuCardRepository().Initialize();
}