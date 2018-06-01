using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardPack : Mover {

    private List<CardType> cards = new List<CardType>();
    public List<CardType> Cards()
    {
        return cards;
    }
    public void SetCards(List<CardType> cardstoset)
    {
        cards.Clear();
        cards.AddRange(cardstoset);
        cardstoset.Clear();
    }
	// Use this for initialization
	void Start () {
		
	}
	
    
	// Update is called once per frame
	void Update () {
        Move();
	}
}
