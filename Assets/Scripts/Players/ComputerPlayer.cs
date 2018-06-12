using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

/// <summary>
/// Represents a computer player
/// </summary>
public class ComputerPlayer : Player {
    private CPUHand cpuHand = null;
    private BaseAI ai = null;

    public override void Init()
    {
        //subscribe to the human player
        HumanPlayer humanPlayer = FindObjectOfType<HumanPlayer>();
        humanPlayer.evtCardChosen += this.OnHumanCardChosen;
        ai = new NormalAI(this);
    }

    public override void Reset()
    {
        base.Reset();
        ai.Dispose();
        ai = new NormalAI(this);
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
    /// Chooses a card with given AI
    /// </summary>
    private void CPUMadeChoice()
    {
        CancelInvoke();
        if (cpuHand.IsActive())
        {
            cpuHand.Hide();
            cardToPlay = ai.ChooseCard(handCards);
            OnCardPicked(cardToPlay);
        }
    }
}
