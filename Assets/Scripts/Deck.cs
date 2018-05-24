using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class Deck : MonoBehaviour {
    private Dictionary<CardType, Texture2D> cardTextures = new Dictionary<CardType,Texture2D>();
    private CardType[] drawPile;
    private Player[] players;
    private int topCard = -1;
    public SpriteAtlas cardFronts;
    public GameObject playedCardPrefab;
    //public Sprite cardFronts;
        // Use this for initialization
    void Start()
    {
        Populate();
        GetPlayers();
    }
    void Update()
    {

    }

    string textureNameForCard(CardType card)
    {
        switch (card)
        {
            case CardType.Chopsticks: return "Chopsticks";
            case CardType.Roll_Single: return "RollSingle";
            case CardType.Roll_Double: return "RollDouble";
            case CardType.Roll_Triple: return "RollTriple";
            case CardType.Dumpling: return "Dumpling";
            case CardType.Nigiri_Squid: return "NigiriSquid";
            case CardType.Nigiri_Salmon: return "NigiriSalmon";
            case CardType.Nigiri_Egg: return "NigiriEgg";
            case CardType.Wasabi: return "Wasabi";
            case CardType.Sashimi: return "Sashimi";
            case CardType.Tempura: return "Tempura";
            case CardType.Pudding: return "Pudding";
            default: return "";
        }
    }

    public Texture2D textureForCard(CardType card)
    {
        if (!cardTextures.ContainsKey(card))
        {
            Sprite sprite = cardFronts.GetSprite(textureNameForCard(card));
            Texture2D cardTexture = new Texture2D((int)sprite.rect.width, (int)sprite.rect.height);
            var pixels = sprite.texture.GetPixels((int)sprite.textureRect.x, (int)sprite.textureRect.y, (int)sprite.textureRect.width, (int)sprite.textureRect.height);
            cardTexture.SetPixels(pixels);
            cardTexture.Apply();
            cardTextures[card] = cardTexture;
        }
        return cardTextures[card];
    }
    int GetNeighborIndex(int playerIndex)
    {
        if ((playerIndex - 1) >=0)
        {
            return playerIndex - 1;
        }
        else
        {
            //Wrap around
            return players.GetLength(0) - 1;
        }
    }
    void GetPlayers()
    {
        players = GetComponentsInChildren<Player>();
        foreach (Player player in players)
        {
            players[player.playerIndex] = player;
            player.dealHand(drawHand(8));
        }
    }
    public void StartNextTurn()
    {
        foreach (Player player in players)
        {
            player.passHand();
        }

        foreach (Player player in players)
        {
            players[GetNeighborIndex(player.playerIndex)].dealHand(player.passedCards);
        }
    }

    public static Deck _deck = null;
    public static Deck GetInstance()
    {
        if (_deck == null)
        {
            _deck = new Deck();
        }
        return _deck;
    }

    private List<CardType> drawHand(int size)
    {
        List<CardType> hand = new List<CardType>();
        for (int i=0;i<size;i++)
        {
            hand.Add(draw());
        }
        return hand;
    }

    public CardType draw()
    {
        CardType card = drawPile[topCard--];
        return card;
    }
	
    void Populate()
    {
        drawPile = new CardType[108];
        for (int i = 0; i < 14; i++)
        {
            drawPile[++topCard] = CardType.Tempura;
            drawPile[++topCard] = CardType.Sashimi;
            drawPile[++topCard] = CardType.Dumpling;
            if (i < 12)
            {
                drawPile[++topCard] = CardType.Roll_Double;
            }
            if (i < 10)
            {
                drawPile[++topCard] = CardType.Nigiri_Salmon;
                drawPile[++topCard] = CardType.Pudding;
            }
            if (i < 8)
            {
                drawPile[++topCard] = CardType.Roll_Triple;
            }
            if (i < 6)
            {
                drawPile[++topCard] = CardType.Wasabi;
                drawPile[++topCard] = CardType.Roll_Single;
            }
            if (i < 5)
            {
                drawPile[++topCard] = CardType.Nigiri_Egg;
                drawPile[++topCard] = CardType.Nigiri_Squid;
            }
            if (i < 4)
            {
                drawPile[++topCard] = CardType.Chopsticks;
            }
        }
        Shuffle();
    }

    void Shuffle()
    {
        System.Random _random = new System.Random();
        for (int i = 0, n = topCard; i <= n; i++)
        {
            int r = i + (int)(_random.NextDouble() * (n - i));
            CardType temp = drawPile[r];
            drawPile[r] = drawPile[i];
            drawPile[i] = temp;
        }
    }

}
