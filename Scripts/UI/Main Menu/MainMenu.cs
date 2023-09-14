using Godot;
using System;

namespace Kompas.UI.MainMenu
{
	public partial class MainMenu : Control
	{
		private const string ClientScenePath = "res://Scenes/ClientScene.tscn";
		private const string BuildDeckPath = "res://Scenes/BuildDeck.tscn";

		private void ConnectToServer() => GetTree().ChangeSceneToFile(ClientScenePath);
		private void BuildDeck() => GetTree().ChangeSceneToFile(BuildDeckPath);
	}
}
