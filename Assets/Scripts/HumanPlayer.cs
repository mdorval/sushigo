﻿using System.Collections;
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
            card.SetPlayer(this);
        }
    }
    protected override void OnHandDealt()
    {
        hand.SetActive(true);
        var myenumerator = handCards.GetEnumerator();
        foreach (HandCard card in cards)
        {
            if (myenumerator.MoveNext())
            {
                if (!card.gameObject.activeInHierarchy)
                {
                    card.gameObject.SetActive(true);
                }
                card.ApplyCard(Deck.Instance().deckInfo.byType(myenumerator.Current));
            }
            else
            {
                card.gameObject.SetActive(false);
            }
        }
    }

    public void PlayCard(CardType card)
    {
        OnCardPicked(card);
        hand.SetActive(false);
    }

}