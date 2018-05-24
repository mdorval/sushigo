using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="DeckInfo", menuName = "DeckInfo", order = 1)]
public class DeckInfo : ScriptableObject {
    public List<CardInfo> cards;
    public CardInfo byType(CardType type)
    {
        foreach(CardInfo card in cards)
        {
            if (card.type == type)
                return card;
        }
        return null;
    }
}
