using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CardInfo  {
    public CardType type;
    public int copiesInDeck = 0;
    public Sprite cardSprite;
    [TextArea(3,10)]
    public string tooltipText;
    [TextArea(10,10)]
    public string rulesPageText;
}
