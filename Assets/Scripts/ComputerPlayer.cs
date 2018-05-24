using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComputerPlayer : Player {
    private CardType cardtoplay = CardType.Null;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public override void dealHand(List<CardType> dealthand)
    {
        handCards = dealthand;
        //Pick a random card to play
        if (cardtoplay != CardType.Null)
        {
            //Only play the card when next pick up next deck is called. That way the player only sees opponnents cards after they've played theirs
            AddCardToPlayArea(cardtoplay);
        }

        System.Random _random = new System.Random();
        int r = (int)(_random.NextDouble() * (handCards.Count-1));
        cardtoplay = handCards[r];
        handCards.Remove(cardtoplay);
    }
}
