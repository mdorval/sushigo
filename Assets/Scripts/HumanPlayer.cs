using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanPlayer : Player
{
    List<PlayedCard> playedCards;
    public GameObject hand;
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public override void dealHand(List<CardType> dealthand)
    {
        handCards = dealthand;
        int n = 0;
        foreach (HandCard card in hand.GetComponentsInChildren<HandCard>())
        {
            if (n < handCards.Count)
            {
                card.gameObject.SetActive(true);
                card.ApplyCard(handCards[n], GetComponentInParent<Deck>().textureForCard(handCards[n]), this);
            }
            else
            {
                card.gameObject.SetActive(false);
            }
            n++;
        }
    }
    public void PlayCard(CardType card)
    {
        AddCardToPlayArea(card);
        handCards.Remove(card);
        GetComponentInParent<Deck>().StartNextTurn();
    }
}