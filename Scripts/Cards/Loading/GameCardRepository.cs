using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Godot;
using Kompas.Cards.Controllers;
using Kompas.Cards.Models;
using Kompas.Effects.Models;
using Kompas.Shared.Enumerable;
using Newtonsoft.Json;

namespace Kompas.Cards.Loading
{
	public partial class GameCardRepository<TSerializableCard, TEffect, TCardController> : CardRepository
		where TSerializableCard : SerializableGameCard
		where TEffect : Effect
		where TCardController : class, ICardController
	{

		[Export]
		private PackedScene CardPrefab { get; set; }

		private static TCardController GetCardController(CardControllerController ccc)
		{
			TCardController ret = null;
			foreach (var c in ccc.CardControllers)
			{
				if (c is TCardController cc) ret = cc;
				else c.QueueFree();
			}

			if (ret == null) throw new System.NotImplementedException($"None of the card controllers were a {typeof(TCardController)}");
			return ret;
		}

		private static IList<TEffect> GetKeywordEffects(SerializableCard card)
		{
			var effects = new List<TEffect>();
			foreach (var (index, keyword) in card.keywords.Enumerate())
			{
				if (!keywordJsons.ContainsKey(keyword))
					GD.PrintErr($"Failed to add {keyword} length {keyword.Length} to {card.cardName}"
					+ $"Not present in {string.Join(", ", keywordJsons.Keys)}");
				var keywordJson = keywordJsons[keyword];
				var eff = JsonConvert.DeserializeObject<TEffect>(keywordJson, CardLoadingSettings);
				eff.arg = card.keywordArgs.Length > index ? card.keywordArgs[index] : 0;
				effects.Add(eff);
			}
			return effects;
		}

		protected delegate TGameCard ConstructCard<TGameCard>(TSerializableCard cardInfo, TEffect[] effects, TCardController ctrl);
		protected delegate void Validate(SerializableCard card);

		public static string JsonPrettify(string json)
		{
			using var stringReader = new StringReader(json);
			using var stringWriter = new StringWriter();
			var jsonReader = new JsonTextReader(stringReader);
			var jsonWriter = new JsonTextWriter(stringWriter) { Formatting = Formatting.Indented };
			jsonWriter.WriteToken(jsonReader);
			return stringWriter.ToString();
		}

		protected TGameCard InstantiateGameCard<TGameCard>(string json, ConstructCard<TGameCard> cardConstructor, Validate validation = null)
			where TGameCard : GameCard
		{
			GD.Print($"Loading {JsonPrettify(json)}");
			TSerializableCard cardInfo;
			var effects = new List<TEffect>();

			try
			{
				cardInfo = JsonConvert.DeserializeObject<TSerializableCard>(json, CardLoadingSettings);
				validation?.Invoke(cardInfo);

				effects.AddRangeWithCast(cardInfo.Effects);
				effects.AddRange(GetKeywordEffects(cardInfo));
			}
			catch (System.ArgumentException argEx)
			{
				//Catch JSON parse error
				GD.PrintErr($"Failed to load {json}, argument exception with message {argEx.Message}, stacktrace {argEx.StackTrace}");
				return default;
			}

			if (CardPrefab.Instantiate() is not CardControllerController controllerController)
					throw new System.ArgumentNullException(nameof(CardControllerController), "Was not the right type");
			var ctrl = GetCardController(controllerController);
			var card = cardConstructor(cardInfo, effects.ToArray(), ctrl);
			controllerController.Name = card.CardName + card.ID;
			return card;
		}
	}
}