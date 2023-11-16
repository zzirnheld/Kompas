using System.Collections.Generic;
using System.IO;
using Godot;
using Kompas.Cards.Controllers;
using Kompas.Cards.Models;
using Kompas.Effects.Models;
using Kompas.Shared.Enumerable;
using Newtonsoft.Json;

namespace Kompas.Cards.Loading
{
	public abstract partial class GameCardRepository<TSerializableCard, TEffect, TCardController> : CardRepository
		where TSerializableCard : SerializableGameCard
		where TEffect : Effect
		where TCardController : class, ICardController
	{
		private PackedScene CardPrefab { get; }

		protected GameCardRepository(PackedScene cardPrefab)
		{
			CardPrefab = cardPrefab;
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
				if (eff == null)
				{
					GD.PushError($"Failed to load {keywordJson}");
					continue;
				}
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

		protected TGameCard? InstantiateGameCard<TGameCard>(string json, ConstructCard<TGameCard> cardConstructor, Validate? validation = null)
			where TGameCard : GameCard
		{
			GD.Print($"Loading {JsonPrettify(json)}");
			TSerializableCard? cardInfo;
			var effects = new List<TEffect>();

			try
			{
				cardInfo = JsonConvert.DeserializeObject<TSerializableCard>(json, CardLoadingSettings);
				if (cardInfo == null)
				{
					GD.PushError($"Failed to load {json}");
					return default;
				}
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

			var ctrl = GetCardController();
			var card = cardConstructor(cardInfo, effects.ToArray(), ctrl);
			return card;
		}

		protected virtual TCardController GetCardController()
		{
			if (CardPrefab.Instantiate() is not TCardController ctrl)
				throw new System.ArgumentNullException(nameof(CardControllerController), "Was not the right type");

			return ctrl;
		}
	}
}