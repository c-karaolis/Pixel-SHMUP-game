using Foxlair.Tools.Events;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Foxlair.Currencies
{

    public class Currency
    {

        private long coins;
        private const long SilverInCopper = 100;
        private const long GoldInCopper = SilverInCopper * 100;
        //private const ulong PlatinumInCopper = GoldInCopper * 100;

        private long Coins
        {
            get => coins;
            set
            {

                if (value <= 0)
                {
                    coins = 0;
                }
                else
                {
                    coins = value;
                }
                FoxlairEventManager.Instance.Currency_OnCurrencyChanged_Event?.Invoke();

            }
        }

        public Currency() => Coins = 0;
        public Currency(long startCoins) => Coins = startCoins;

        public void Add(long gold, long silver, long copper)
        {
            Coins += gold * GoldInCopper;
            Coins += silver * SilverInCopper;
            Coins += copper;
        }

        public void Add(long coins)
        {
            Coins += coins;
        }


        public void Remove(long gold, long silver, long copper)
        {
            Coins -= gold * GoldInCopper;
            Coins -= silver * SilverInCopper;
            Coins -= copper;
        }
        public void Remove(long coins)
        {
            Coins -= coins;
        }

        public long[] ConvertValueExchange(long coinsToConvert)
        {
            long[] exchangeRates = new long[3];

            exchangeRates[0] = coinsToConvert / GoldInCopper;
            exchangeRates[1] = coinsToConvert % GoldInCopper / SilverInCopper;
            exchangeRates[2] = coinsToConvert % SilverInCopper;

            return exchangeRates;
        }

        //Solution based on answer here: https://stackoverflow.com/questions/3810943/integer-convert-to-wow-gold/3811023

        public long Gold { get { return Coins / GoldInCopper; } } //or _coins/100000
        public long Silver { get { return Coins % GoldInCopper / SilverInCopper; } }
        public long Copper { get { return Coins % SilverInCopper; } }

    }
}