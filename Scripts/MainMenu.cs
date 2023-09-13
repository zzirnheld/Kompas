using Godot;
using System;

public partial class MainMenu : Control
{
	private const string ClientScenePath = "res://Scenes/ClientScene.tscn";

	private void _ConnectToServer()
	{
		GetTree().ChangeSceneToFile(ClientScenePath);
	}
}
