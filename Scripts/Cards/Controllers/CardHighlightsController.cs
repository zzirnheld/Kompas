using Godot;
using Kompas.Shared.Exceptions;

public partial class CardHighlightsController : Node3D
{
	[Export]
	private MeshInstance3D? _negated;
	private MeshInstance3D Negated => _negated
		?? throw new UnassignedReferenceException(nameof(_negated), this);

	[Export]
	private MeshInstance3D? _activated;
	private MeshInstance3D Activated => _activated
		?? throw new UnassignedReferenceException(nameof(_activated), this);

	private StandardMaterial3D? _negatedMaterial;
	private StandardMaterial3D NegatedMaterial => _negatedMaterial
		?? throw new NotReadyYetException(nameof(_negatedMaterial), this);

	private StandardMaterial3D? _activatedMaterial;
	private StandardMaterial3D ActivatedMaterial => _activatedMaterial
		?? throw new NotReadyYetException(nameof(_activatedMaterial), this);

	private double time = 0;

	public override void _Ready()
	{
		base._Ready();
		_negatedMaterial = Negated.MaterialOverride as StandardMaterial3D
			?? throw new System.InvalidOperationException($"Wrong material for {Name}.{nameof(_negated)}");
		_activatedMaterial = Activated.MaterialOverride as StandardMaterial3D
			?? throw new System.InvalidOperationException($"Wrong material for {Name}.{nameof(_activated)}");

		// Negated.Visible = System.Random.Shared.Next() % 2 == 0;
		// Activated.Visible = System.Random.Shared.Next() % 2 == 0;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		time += delta;

		var negatedColor = NegatedMaterial.AlbedoColor;
		int negatedAlpha = (int)(50 + (100 * System.MathF.Sin((float) time)));
		negatedColor.A8 = negatedAlpha;
		NegatedMaterial.AlbedoColor = negatedColor;

		var activatedColor = ActivatedMaterial.AlbedoColor;
		activatedColor.A8 = 150 - negatedAlpha;
		ActivatedMaterial.AlbedoColor = activatedColor;
	}
}
