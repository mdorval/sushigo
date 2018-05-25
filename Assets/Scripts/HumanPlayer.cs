using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanPlayer : Player
{
    List<PlayedCard> playedCards;
    public GameObject hand;
    public override void dealHand(List<CardType> dealthand)
    {
        handCards = dealthand;
        int n = 0;
        foreach (HandCard card in hand.GetComponentsInChildren<HandCard>())
        {
            if (n < handCards.Count)
            {
                card.GetComponent<Renderer>().enabled = true;
                card.ApplyCard(handCards[n], GetComponentInParent<Deck>().textureForCard(handCards[n]), this);
            }
            else
            {
                card.GetComponent<Renderer>().enabled = false;
            }
            n++;
        }
    }
    public void PlayCard(CardType card)
    {
        pickCardToPlay(card);
        GetComponentInParent<Deck>().StartNextTurn();
    }
}