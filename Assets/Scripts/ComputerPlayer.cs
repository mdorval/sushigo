using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
/// <summary>
/// Represents the CPU Hand prefab. It's a collection of quads that accurately represents the number of cards tied to the hand.
/// </summary>
public class CPUHand
{
    private GameObject hand = null;
    private List<Transform> cards = new List<Transform>();
    bool _isActive = false;
    /// <summary>
    /// Creates the Hand
    /// </summary>
    /// <param name="prefab">Prefab of hand</param>
    /// <param name="transform">Where to instantiate hand</param>
    public CPUHand(GameObject prefab, Transform transform)
    {
        hand = GameObject.Instantiate(Deck.Instance().cpuHandPrefab, transform);
        foreach(Transform card in hand.transform)
        {
            cards.Add(card);
        }

        //The cards rotate outwards in a fan like fashion
        //We want to sort by distance from center so we can hide the cards from the outside in.
        cards = cards.OrderBy(card => Math.Abs(card.localPosition.x)).ToList();
    }
    ~CPUHand()
    {
        if (hand != null)
        {
            GameObject.Destroy(hand);
        }
    }
    /// <summary>
    /// Hides the hand
    /// </summary>
    public void Hide()
    {
        if (hand != null)
        {
            hand.SetActive(false);
        }
        _isActive = false;
    }
    /// <summary>
    /// Shows if the hand is hidden
    /// </summary>
    /// <returns>If the hand is hidden</returns>
    public bool IsActive()
    {
        return _isActive;
    }
    /// <summary>
    /// Shows the Hand
    /// </summary>
    /// <param name="numberOfCards">The number of cards to show</param>
    public void Show(int numberOfCards)
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
                //We want to show the cards from the inside out, the list is already sorted as such
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

    /// <summary>
    /// Called from the HumanPlayer evtCardChosen event
    /// </summary>
    /// <param name="player">The Player who chose the card</param>
    private void OnHumanCardChosen(Player player)
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
            cpuHand.Show(handCards.Count);
            //A bit of randomness to give it some character
            Invoke("CPUMadeChoice", UnityEngine.Random.Range(0.5f, 2.5f));
        }
        else
        {
            //Don't even show the CPUHand object in this case, just instantiate the player card immediately
            OnCardPicked(handCards.First());
        }
    }

    /// <summary>
    /// Chooses a card randomly (TODO: proper AI)
    /// </summary>
    private void CPUMadeChoice()
    {
        CancelInvoke();
        if (cpuHand.IsActive())
        {
            cpuHand.Hide();
            int r = (int)UnityEngine.Random.Range(0.0f, handCards.Count - 1);
            cardToPlay = handCards[r];
            OnCardPicked(cardToPlay);
        }
    }
}
