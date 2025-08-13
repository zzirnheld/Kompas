using Godot;
using Kompas.Shared.Exceptions;

namespace Kompas.Client.Effects.Views;

public abstract partial class ClientStackableView : HBoxContainer
{
	[Export]
	private Control? _onStackParent;
	private Control OnStackParent => _onStackParent
		?? throw new UnassignedReferenceException(nameof(_onStackParent), this);

	[Export]
	private Control? _currentlyResolvingParent;
	private Control CurrentlyResolvingParent => _currentlyResolvingParent
		?? throw new UnassignedReferenceException(nameof(_currentlyResolvingParent), this);

	public void ShowCurrentlyResolving(bool currentlyResolving)
	{
		OnStackParent.Visible = !currentlyResolving;
		CurrentlyResolvingParent.Visible = currentlyResolving;
	}
}