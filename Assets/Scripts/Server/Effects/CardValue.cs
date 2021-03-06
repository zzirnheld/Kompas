﻿using KompasCore.Cards;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KompasCore.Effects
{
    [Serializable]
    public class CardValue
    {
        #region values
        private const string Nimbleness = "N";
        private const string Endurance = "E";
        private const string SummoningCost = "S";
        private const string Wounding = "W";
        private const string CastingCost = "C";
        private const string AugmentCost = "A";

        private const string Cost = "Cost";
        #endregion values

        public string value;
        public int multiplier = 1;
        public int divisor = 1;
        public int modifier = 0;

        public int GetValueOf(IGameCardInfo card)
        {
            if (card == null) throw new ArgumentException("Cannot get value of null card", "card");

            switch (value)
            {
                case Nimbleness:    return card.N;
                case Endurance:     return card.E;
                case SummoningCost: return card.S;
                case Wounding:      return card.W;
                case CastingCost:   return card.C;
                case AugmentCost:   return card.A;

                case Cost:          return card.Cost;
                default: throw new ArgumentException($"Invalid value string {value}", "value");
            }
        }
    }
}