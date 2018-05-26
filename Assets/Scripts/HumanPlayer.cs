using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanPlayer : Player
{
    List<PlayedCard> playedCards;
    public GameObject hand;
    public GameObject cardPrefab;
    public override void dealHand(List<CardType> dealthand)
    {
        handCards = dealthand;
        foreach (HandCard card in hand.GetComponentsInChildren<HandCard>())
        {
            Destroy(card);
        }
        foreach (CardType card in dealthand)
        {
            GameObject mycard = Instantiate(cardPrefab,hand.transform);
            mycard.GetComponent<HandCard>().ApplyCard(card, GetComponentInParent<Deck>().spriteForCard(card), this);
        }
    }
    public void PlayCard(CardType card)
    {
        pickCardToPlay(card);
        GetComponentInParent<Deck>().StartNextTurn();
    }
}