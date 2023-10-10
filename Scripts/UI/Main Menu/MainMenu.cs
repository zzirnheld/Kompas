using Godot;
using System;

namespace Kompas.UI.MainMenu
{
	public partial class MainMenu : Control
	{
		private const string ServerScenePath = "res://Scenes/ServerScene.tscn";
		private const string ClientScenePath = "res://Scenes/ClientScene.tscn";
		private const string BuildDeckPath = "res://Scenes/BuildDeckScene.tscn";

		private void HostServer() => GetTree().ChangeSceneToFile(ServerScenePath);
		private void ConnectToServer() => GetTree().ChangeSceneToFile(ClientScenePath);
		private void BuildDeck() => GetTree().ChangeSceneToFile(BuildDeckPath);
		private void Quit() => GetTree().Quit();

		public override void _Ready()
		{
			base._Ready();
			DisplayServer.WindowSetMode(DisplayServer.WindowMode.Maximized);
		}
	}
}
