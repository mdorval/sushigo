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
    private Texture2D texture = null;
    public Texture2D Texture()
    {
        if (texture == null)
        {
            Sprite sprite = cardSprite;
            texture = new Texture2D((int)sprite.rect.width, (int)sprite.rect.height);
            var pixels = sprite.texture.GetPixels((int)sprite.textureRect.x, (int)sprite.textureRect.y, (int)sprite.textureRect.width, (int)sprite.textureRect.height);
            texture.SetPixels(pixels);
            texture.Apply();
        }
        return texture;
    }
}
