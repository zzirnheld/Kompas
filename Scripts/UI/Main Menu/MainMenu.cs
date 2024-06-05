using Godot;
using Kompas.Godot;
using Kompas.Shared;
using Kompas.Shared.Exceptions;
using System.Threading.Tasks;

namespace Kompas.UI.MainMenu
{
	public partial class MainMenu : Control
	{
		private const string ServerScenePath = "res://Scenes/ServerScene.tscn";
		private const string ClientScenePath = "res://Scenes/ClientScene.tscn";
		private const string BuildDeckPath = "res://Scenes/BuildDeckScene.tscn";

		private void HostServer() => LoadScene(ServerScenePath);
		private void ConnectToServer() => LoadScene(ClientScenePath);
		private void BuildDeck() => LoadScene(BuildDeckPath);
		private void Quit() => GetTree().Quit();

		[Export]
		private MainMenuLogoController? _mainMenuLogoController;
		private MainMenuLogoController MainMenuLogoController => _mainMenuLogoController
			?? throw new UnassignedReferenceException(nameof(_mainMenuLogoController), this);

		private bool loadingAnotherScene = false;

		//Async void because called from main menu buttons' handlers
		private async void LoadScene(string scenePath)
		{
			if (loadingAnotherScene) return;
			loadingAnotherScene = true;

			Task spinPastMenu = MainMenuLogoController.SpinForSceneChange();

			ResourceLoader.LoadThreadedRequest(scenePath);
			Task<bool> load = this.DoEachFrame(_ =>
			{
				var status = ResourceLoader.LoadThreadedGetStatus(scenePath);
				if (status == ResourceLoader.ThreadLoadStatus.InProgress)
					return Result<bool>.None;

				return Result<bool>.Of(status == ResourceLoader.ThreadLoadStatus.Loaded);
			});

			await spinPastMenu;
			await Task.WhenAny(load, MainMenuLogoController.ChangeScenesLoadingSpinning());

			if (load.IsCompleted && load.Result)
			{
				var res = ResourceLoader.LoadThreadedGet(scenePath);
				if (res is not PackedScene scene) throw new System.InvalidOperationException("Resource was not a packed scene!");
				GetTree().ChangeSceneToPacked(scene);
			}
		}

		public override void _Ready()
		{
			base._Ready();
			DisplayServer.WindowSetMode(DisplayServer.WindowMode.Maximized);
		}
	}
}
