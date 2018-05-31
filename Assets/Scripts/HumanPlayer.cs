using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanPlayer : Player
{
    List<PlayedCard> playedCards;
    public GameObject hand;
    public GameObject cardPrefab;
    public GameObject playingCardPrefab;
    private List<HandCard> cards = new List<HandCard>();
    public override void Init()
    {
        foreach (HandCard card in hand.GetComponentsInChildren<HandCard>())
        {
            cards.Add(card);
            card.player = this;
        }
    }
    public override void dealHand(List<CardType> dealthand)
    {
        hand.SetActive(true);
        handCards = dealthand;
        //DestroyCards();
        var myenumerator = dealthand.GetEnumerator();
        foreach (HandCard card in cards)
        {
            if (myenumerator.MoveNext())
            {
                if (!card.gameObject.activeInHierarchy)
                {
                    card.gameObject.SetActive(true);
                }
                card.ApplyCard(GetComponentInParent<Deck>().deckInfo.byType(myenumerator.Current));
            }
            else
            {
                card.gameObject.SetActive(false);
            }
        }
    }
    private void DestroyCards()
    {
        foreach (HandCard card in hand.GetComponentsInChildren<HandCard>())
        {
            card.gameObject.SetActive(false);
        }

    }


    public void PlayCard(CardType card)
    {
        pickCardToPlay(card);
        //DestroyCards();
        hand.SetActive(false);
        //GetComponentInParent<Deck>().StartNextTurn();
    }

}