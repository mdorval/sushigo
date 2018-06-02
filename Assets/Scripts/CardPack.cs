using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents a pack of cards being passed from one player to another
/// </summary>
public class CardPack : Mover {

    private List<CardType> cards = new List<CardType>();
    /// <summary>
    /// The Cards in this pack
    /// </summary>
    /// <returns>Returns the cards</returns>
    public List<CardType> Cards()
    {
        return cards;
    }

    /// <summary>
    /// Sets the cards from this pack and removes the cards from the calling list
    /// </summary>
    /// <param name="cardstoset">Cards</param>
    public void SetCards(List<CardType> cardstoset)
    {
        cards.Clear();
        cards.AddRange(cardstoset);
        cardstoset.Clear();
    }
	    
	// Update is called once per frame
	void Update () {
        Move();
	}
}
