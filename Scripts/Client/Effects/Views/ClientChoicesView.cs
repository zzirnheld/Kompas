using System.Linq;
using Godot;
using Kompas.Shared.Exceptions;

namespace Kompas.Client.Effects.Views;

public partial class ClientChoicesView : Control
{
	[Export]
	private Label? _label;
	private Label Label => _label ?? throw new UnassignedReferenceException();

	[Export]
	private PackedScene? _choiceButton;
	private PackedScene ChoiceButton => _choiceButton ?? throw new UnassignedReferenceException();

	[Export]
	private Control? _choicesParent;
	private Control ChoicesParent => _choicesParent ?? throw new UnassignedReferenceException();

	public string Decision
	{
		set { Label.Text = value; }
	}

	public ClientChoiceView AddChoice(string choice)
	{
		var obj = ChoiceButton.Instantiate<ClientChoiceView>();
		obj.Init(choice);
		ChoicesParent.AddChild(obj);
		return obj;
	}

	public void Reset()
	{
		foreach (var formerChild in ChoicesParent.GetChildren().ToArray())
			formerChild.QueueFree();
	}
}
