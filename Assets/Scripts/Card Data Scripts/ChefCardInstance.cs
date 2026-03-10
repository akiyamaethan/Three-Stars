using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ThreeStars;

public class ChefCardInstance
{
    public ChefCard cardData;
    public int remainingShifts;

    public ChefCardInstance(ChefCard data)
    {
        cardData = data;
        remainingShifts = data.durability;
    }
}
