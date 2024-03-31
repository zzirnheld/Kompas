using Godot;
using Kompas.Shared.Exceptions;

namespace KOmpas.Client.Effects.Views;

public partial class ClientChoiceView : Node
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

	public void Show(string decision, string choices)
	{
		Label.Text = decision;
		foreach(var choice in choices)
		{
			//TODO
		}
	}
}
