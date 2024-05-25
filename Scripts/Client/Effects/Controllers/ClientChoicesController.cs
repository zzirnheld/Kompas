using System;
using System.Collections.Generic;
using Kompas.Client.Effects.Views;
using Kompas.Shared.Enumerable;

namespace Kompas.Client.Effects.Controllers;

public class ClientChoicesController
{
	private readonly ClientChoicesView choiceView;

	public event EventHandler<int>? ChooseIndex;

	public ClientChoicesController(ClientChoicesView choiceView)
	{
		this.choiceView = choiceView;
	}

	public void Show(string decision, IEnumerable<string> choices)
	{
		choiceView.Reset();
		choiceView.Decision = decision;
		foreach (var (index, choice) in choices.Enumerate())
		{
			var obj = choiceView.AddChoice(choice);
			obj.Pressed += () => Chose(index);
		}
		choiceView.Visible = true;
	}

	private void Chose(int index)
	{
		ChooseIndex?.Invoke(this, index);
		choiceView.Visible = false;
	}
}