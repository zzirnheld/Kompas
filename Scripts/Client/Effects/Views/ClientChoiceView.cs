using Godot;

namespace Kompas.Client.Effects.Views;

public partial class ClientChoiceView : Button
{
	public void Init(string choice)
	{
		Text = choice;
	}
}