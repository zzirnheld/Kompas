namespace Kompas.Gamestate.Locations
{
	﻿public enum Location
	{
		Nowhere, Board, Discard, Hand, Deck, Annihilation
	}

	public static class LocationHelpers
	{
		public static Location FromString(string str)
		{
			return str switch
			{
				"Board" or "Field" 	=> Location.Board,
				"Hand" 				=> Location.Hand,
				"Discard" 			=> Location.Discard,
				"Annihilation"		=> Location.Annihilation,
				"Deck" 				=> Location.Deck,
				_ => throw new System.NotImplementedException($"Unknown string to convert to CardLocation {str}"),
			};
		}

		/// <summary>
		/// Should reflect the CardLocationHelpers.FromString function
		/// </summary>
		public static string StringVersion(this Location cardLocation) => cardLocation switch
		{
			Location.Nowhere 	=> "Nowhere",
			Location.Board 		=> "Board",
			Location.Hand 		=> "Hand",
			Location.Discard 	=> "Discard",
			Location.Annihilation => "Annihilation",
			Location.Deck 		=> "Deck",

			_ => throw new System.NotImplementedException($"Unknown CardLocation {cardLocation} to convert to string")
		};
	}
}