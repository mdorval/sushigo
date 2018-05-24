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
    public DeckInfo deckInfo;
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


    public Texture2D textureForCard(CardType card)
    {
        if (!cardTextures.ContainsKey(card))
        {
            Sprite sprite = deckInfo.byType(card).cardSprite;
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
        //First count the number of cards
        int count = 0;
        foreach (CardInfo cardInfo in deckInfo.cards)
        {
            count += cardInfo.copiesInDeck;
        }
        drawPile = new CardType[count];
        foreach (CardInfo cardinfo in deckInfo.cards)
        {
            for(int i=0;i<cardinfo.copiesInDeck;i++)
            {
                drawPile[++topCard] = cardinfo.type;
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
