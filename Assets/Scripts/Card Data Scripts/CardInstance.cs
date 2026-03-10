using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ThreeStars;


namespace ThreeStars
{
    [System.Serializable]
    public class CardInstance
    {
        public PlayingCard cardData;
        public CardInstance(PlayingCard data)
        {
            cardData = data;
        }
    }
}
