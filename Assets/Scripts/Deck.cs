using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.U2D;
using System.Linq;

public class Deck : MonoBehaviour {
    private Dictionary<CardType, Texture2D> cardTextures = new Dictionary<CardType,Texture2D>();
    private CardType[] drawPile;
    private Player[] players;
    private ScoreCards scoreCards = new ScoreCards();
    private int topCard = -1;
    private int roundNumber = 1;
    public GameObject playedCardPrefab;
    public DeckInfo deckInfo;
    public Text scoreText;
    void Start()
    {
        Populate();
        GetPlayers();
    }
    void Update()
    {

    }

    public void AddScoreCard(ScoreCard card)
    {
        scoreCards.Add(card);
        UpdateText();
    }

    void UpdateText()
    {
        scoreCards.UpdateRankScores();
        string rettext = "Score\n";
        foreach (ScoreCard score in scoreCards.SortByScore())
        {
            rettext += "<color='#" + (score.HtmlColor()) + "'>" + score.Name() + ": " + score.Score() + "</color>\n";
        }
        rettext += "\n\nRolls\n";
        foreach (ScoreCard score in scoreCards.SortByRolls())
        {
            int rollScore = scoreCards.RollScore(score);
            rettext += "<color='#" + (score.HtmlColor()) + "'>" + score.Name() + ": " + score.Rolls() 
                + (rollScore != 0 ? "("+rollScore+" Points)" : "")
                +"</color>\n";
        }
        rettext += "\n\nPuddings\n";
        foreach (ScoreCard score in scoreCards.SortByPudding())
        {
            int puddingScore = scoreCards.PuddingScore(score);
            rettext += "<color='#" + (score.HtmlColor()) + "'>" + score.Name() + ": " + score.Puddings()
                + (puddingScore != 0 ? "(" + puddingScore + " Points)" : "")
                + "</color>\n";
        }
        scoreText.text = rettext;
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
    public void finishRound()
    {
        scoreCards.scoreRolls();
        if (roundNumber == 3)
        {
            scoreCards.scorePuddings();
        }
        UpdateText();
        foreach (Player player in players)
        {
            foreach(ScoreGroup group in player.GetComponentsInChildren<ScoreGroup>())
            {                
                if (group.GetType() == typeof(PuddingScoreGroup))
                {
                    group.transform.localPosition = new Vector3(0, 0, 0);
                }
                else
                {
                    Destroy(group);
                }
            }
        }
        roundNumber++;
    }
    public void StartNextTurn()
    {
        foreach (Player player in players)
        {
            player.playCard();
        }
        UpdateText();

        bool handPassed = false;
        foreach (Player player in players)
        {
            handPassed = player.PassHand();
        }
        if (handPassed)
        {
            foreach (Player player in players)
            {
                players[GetNeighborIndex(player.playerIndex)].dealHand(player.passedCards);
            }
        }
        else
        {
            finishRound();
            if (roundNumber < 4)
            {
                foreach (Player player in players)
                {
                    players[GetNeighborIndex(player.playerIndex)].dealHand(drawHand(8));
                }
            }
        }
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
