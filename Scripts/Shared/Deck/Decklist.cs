using System.Collections.Generic;

namespace Kompas.Shared
{
	public class Decklist
	{
		public string deckName;
		public string avatarName;
		public List<string> deck = new();

		public Decklist Copy(string newName) => new()
		{
			deckName = newName,
			avatarName = avatarName,
			deck = new(deck),
		};
	}
}