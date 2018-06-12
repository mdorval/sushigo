using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/// <summary>
/// Represents a SushiGo drawpile
/// Includes all cards available
/// </summary>
class DrawPile
{
    DeckInfo _info;
    int _topCard = -1;
    int _count = 0;
    CardType[] _cards;

    /// <summary>
    /// Creates a DrawPile
    /// </summary>
    /// <param name="info">The Deck info</param>
    public DrawPile(DeckInfo info)
    {
        _info = info;
        Populate();
    }

    /// <summary>
    /// Populates the DrawPile with all cards
    /// </summary>
    public void Populate()
    {
        foreach (CardInfo cardInfo in _info.cards)
        {
            _count += cardInfo.copiesInDeck;
        }
        _cards = new CardType[_count];
        foreach (CardInfo cardinfo in _info.cards)
        {
            for (int i = 0; i < cardinfo.copiesInDeck; i++)
            {
                _cards[++_topCard] = cardinfo.type;
            }
        }
        Shuffle();
    }

    /// <summary>
    /// Shuffles the DrawPile
    /// </summary>
    public void Shuffle()
    {
        System.Random _random = new System.Random();
        for (int i = 0, n = _topCard; i <= n; i++)
        {
            int r = i + (int)(_random.NextDouble() * (n - i));
            SwapCard(r, i);
        }
    }

    /// <summary>
    /// Swap Cards in the drawPile Array
    /// </summary>
    /// <param name="cardAIndex">first card</param>
    /// <param name="cardBIndex">second card</param>
    private void SwapCard(int cardAIndex, int cardBIndex)
    {
        CardType temp = _cards[cardAIndex];
        _cards[cardAIndex] = _cards[cardBIndex];
        _cards[cardBIndex] = temp;
    }

    /// <summary>
    /// Used for predictive decks only - adds an incorrectly predicted card
    /// back into the deck
    /// </summary>
    /// <param name="cardToAdd">card to add back</param>
    public void AddSpecificCard(CardType cardToAdd)
    {
        _cards[++_topCard] = cardToAdd;
    }

    /// <summary>
    /// Used for predictive decks only - draws a specific card from the deck
    /// </summary>
    /// <param name="cardToDraw">Card to draw</param>
    /// <returns>requested card</returns>
    public CardType DrawSpecificCard(CardType cardToDraw)
    {
        bool found = false;
        int findcard;
        for (findcard = 0;findcard <= _topCard;findcard++)
        {
            if (_cards[findcard] == cardToDraw)
            {
                SwapCard(findcard, _topCard);
                found = true;
                break;
            }
        }
        if (!found)
        {
            //We can still return the card here,
            //but don't touch the deck
            //we're likely near the bottom
            //this is only used for predictions anyways
            return cardToDraw;
        }
        return _cards[_topCard--];

    }

    /// <summary>
    /// Draws a hand of random cards remaining in the deck
    /// </summary>
    /// <param name="size">The number of cards to draw</param>
    /// <returns>List of Cards</returns>
    public List<CardType> DrawHand(int size)
    {
        List<CardType> hand = new List<CardType>();
        for (int i = 0; i < size; i++)
        {
            hand.Add(_cards[_topCard--]);
        }
        return hand;
    }
}
