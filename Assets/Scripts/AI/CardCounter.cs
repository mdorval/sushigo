using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Keeps Track of Cards Played and Cards in other players packs
/// For any unknown packs we keep our own draw pile to guess what we think is in those packs
/// And make a calculation based on that
/// </summary>
class CardCounter: IDisposable
{
    DrawPile pile = null;
    private List<CardType>[] packs;
    int currentCardCount = 0;
    Player _player;
    /// <summary>
    /// Creates a CardCounter
    /// </summary>
    /// <param name="player">Player to cardcount for (player at position 0)</param>
    public CardCounter(Player player)
    {
        _player = player;
        packs = new List<CardType>[4] { new List<CardType>(), new List<CardType>(), new List<CardType>(), new List<CardType>() };
        pile = new DrawPile(Deck.Instance().deckInfo);
        Deck.Instance().evtCardPlayed += this.OnPlayerCardPlayed;
    }

    /// <summary>
    /// Called when a card is played. Used to keep track of what's remaining in packs
    /// </summary>
    /// <param name="player">Player who played the card</param>
    /// <param name="card">Card played</param>
    private void OnPlayerCardPlayed(Player player, CardType card)
    {
        List<CardType> pack = PackForPlayer(player);
        if (!pack.Contains(card))
        {
            //If we make it here we're using a predicted pack and we predicted the cards wrong (likely)
            //Swap out one of the predicted cards with our drawpile
            pile.AddSpecificCard(pack[0]);
            pack[0] = pile.DrawSpecificCard(card);
        }
        pack.Remove(card);
    }

    /// <summary>
    /// Gets the pack for given player
    /// </summary>
    /// <param name="player">The player to get the pack for</param>
    /// <param name="fastForward">Number of turns passed this one to look at</param>
    /// <returns></returns>
    private List<CardType> PackForPlayer(Player player = null, int fastForward = 0)
    {
        //Pack index is based on starting pack of this player
        //So index 0 is the first pack this player saw. Index 1 is the first pack our immediate (pass-to) neighbor saw
        //1 turn in, this player is looking at pack at index 3. Immediate (pass-to) neighbor is looking at pack at index 0 (our old pack)
        //2 turn in, this player is looking at pack at index 2. Immediate neighbor is looking at index 1

        int index = (player == null || player == _player) ? 0 : Deck.Instance().CalculatePacksBetween(_player, player);
        return packs[(currentCardCount - fastForward + index) % 4];
    }

    /// <summary>
    /// Counts the number of playable cards from now until end of round of a given type
    /// </summary>
    /// <param name="card">Card to check</param>
    /// <returns>count of playable cards</returns>
    public int MaxCopiesPlayableThisRound(CardType card)
    {
        int count = 0;
        for (int i=0;i < 4 && i < currentCardCount;i++)
        {
            //integer divide by number of rounds gives us 
            //how many times we should check this hand for the card
            //we need to +1 since we need to 
            //i < currentCardCount keeps us from looking at hands we'll never see
            int maxPlayableInThisPack = Mathf.CeilToInt((float)(currentCardCount - i) / 4.0f);
            List<CardType> pack = PackForPlayer(_player, i);
            int countInThisPack = pack.FindAll(r => r == card).Count;
            //only count the number of cards we can play from this pack. Usually 1 or 2
            count += countInThisPack > maxPlayableInThisPack ? maxPlayableInThisPack : countInThisPack;
        }
        return count;
    }

    /// <summary>
    /// Inits internal pack info at the beginning of the turn
    /// </summary>
    /// <param name="cards">Cards to remember</param>
    public void InitPack(List<CardType> cards)
    {
        currentCardCount = cards.Count;
        //Case: First hand of the game
        if (cards.Count == 8)
        {
            //Init all packs, starting with 0
            foreach (CardType card in cards)
            {
                //Draw the pile to make our predictions more accurate
                //when we predict the opponent's hands we take into account how many
                //of each card are in the whole deck
                //and which cards have already been played
                packs[0].Add(pile.DrawSpecificCard(card));
            }
            //Opponents packs we just guess for now, and replace with actual cards when we see the pack
            for (int i = 1; i < 4; i++)
            {
                packs[i].AddRange(pile.DrawHand(8));
            }
        }
        //Case: This was a predicted pack we now know the cards for
        else if (cards.Count >= 5)
        {
            List<CardType> mypack = PackForPlayer();
            //We need to consolidate the actual hand with our random prediction
            foreach (CardType card in mypack)
            {
                pile.AddSpecificCard(card);
            }
            mypack.Clear();
            foreach (CardType card in cards)
            {
                mypack.Add(pile.DrawSpecificCard(card));
            }
            pile.Shuffle();
        }
        //Case: We should know all of the cards in this pack already
        else
        {
            List<CardType> thispack = PackForPlayer(this._player);
            if (!thispack.SequenceEqual(cards))
            {
                Debug.LogError("Prediction incorrect "+ thispack.Count + cards.Count);
            }
        }
    }

    public void Dispose()
    {
        Deck.Instance().evtCardPlayed -= this.OnPlayerCardPlayed;
    }
}