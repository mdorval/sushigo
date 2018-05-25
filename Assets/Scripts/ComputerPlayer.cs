using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComputerPlayer : Player {
    //private CardType cardtoplay = CardType.Null;

    public override void dealHand(List<CardType> dealthand)
    {
        handCards = dealthand;
        System.Random _random = new System.Random();
        int r = (int)(_random.NextDouble() * (handCards.Count-1));
        cardToPlay = handCards[r];
        pickCardToPlay(cardToPlay);
    }
}
