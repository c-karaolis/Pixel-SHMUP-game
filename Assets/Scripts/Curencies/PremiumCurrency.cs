using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PremiumCurrency
{
    private int PlatinumBar { get; set; }

    public PremiumCurrency() => PlatinumBar = 0;
    public PremiumCurrency(int startingPlatinumBars) => PlatinumBar = startingPlatinumBars;


    public void Add(int PlatinumBars)
    {
        PlatinumBar += PlatinumBars;
    }
}
