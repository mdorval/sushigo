using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
public class CPUHand
{
    private GameObject hand = null;
    private List<Transform> cards = new List<Transform>();
    bool _isActive = false;
    public CPUHand(GameObject prefab, Transform transform)
    {
        hand = GameObject.Instantiate(Deck.Instance().cpuHandPrefab, transform);
        foreach(Transform card in hand.transform)
        {
            cards.Add(card);
        }

        //The cards rotate outwards in a fan like fashion
        //We want to sort by distance from center so we can go through the list in order and show the fan of cards symetrically
        cards = cards.OrderBy(card => Math.Abs(card.localPosition.x)).ToList();
    }
    ~CPUHand()
    {
        if (hand != null)
        {
            GameObject.Destroy(hand);
        }
    }
    public void hide()
    {
        if (hand != null)
        {
            hand.SetActive(false);
        }
        _isActive = false;
    }
    public bool isActive()
    {
        return _isActive;
    }
    public void show(int numberOfCards)
    {
        bool first = true;
        int count = 0;
        foreach(Transform card in cards)
        {
            if(first)
            {
                //The first card is a special case
                //it's a center card with no rotation
                //we only show this card if there's an odd number of cards
                //otherwise we hide it.
                if (numberOfCards % 2 == 0)
                {
                    card.gameObject.SetActive(false);
                }
                else
                {
                    card.gameObject.SetActive(true);
                    count++;
                }
                first = false;
            }
            else
            {
                if (count < numberOfCards)
                {
                    card.gameObject.SetActive(true);
                    count++;
                }
                else
                {
                    card.gameObject.SetActive(false);
                }
            }
        }
        hand.SetActive(true);
        _isActive = true;
    }
}

public class ComputerPlayer : Player {
    //private CardType cardtoplay = CardType.Null;
    private CPUHand cpuHand = null;

    public override void Init()
    {
        //subscribe to the human player
        HumanPlayer humanPlayer = FindObjectOfType<HumanPlayer>();
        humanPlayer.evtCardChosen += this.OnHumanCardChosen;
    }

    public void OnHumanCardChosen(Player player)
    {
        //The CPUs wait a random amount of time to make it look like they're waiting
        //But if the human player has decided on a card already, don't keep them waiting
        //CPUMadeChoice will be called twice but only the first time it will execute
        CPUMadeChoice();
    }

    protected override void OnHandDealt()
    {
        if (cpuHand == null)
        {
            cpuHand = new CPUHand(Deck.Instance().cpuHandPrefab,handPosition.transform);
        }
        if (handCards.Count > 1)
        {
            cpuHand.show(handCards.Count);
            //A bit of randomness to give it some character
            Invoke("CPUMadeChoice", UnityEngine.Random.Range(1.0f, 4.0f));
        }
        else
        {
            OnCardPicked(handCards.First());
        }
    }



    private void CPUMadeChoice()
    {
        CancelInvoke();
        if (cpuHand.isActive())
        {
            cpuHand.hide();
            int r = (int)UnityEngine.Random.Range(0.0f, handCards.Count - 1);
            cardToPlay = handCards[r];
            OnCardPicked(cardToPlay);
        }
    }
}
