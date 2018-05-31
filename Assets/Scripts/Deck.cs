using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.U2D;
using System.Linq;

public class Deck : MonoBehaviour {
    private Dictionary<CardType, Texture2D> cardTextures = new Dictionary<CardType,Texture2D>();
    private CardType[] drawPile;
    private LinkedList<Player> players = new LinkedList<Player>();
    private ScoreCards scoreCards = new ScoreCards();
    private int topCard = -1;
    private int roundNumber = 1;
    public GameObject playedCardPrefab;
    public GameObject playingCardPrefab;
    public DeckInfo deckInfo;
    public Text scoreText;
    public ContinueDialog continueDialog;
    private bool passLeft = false;
    void Start()
    {
        Populate();
        GetPlayers();
    }
    void Update()
    {

    }

    private bool AllPlayersAtState(Player.PlayerState stateToCheck)
    {
        foreach(Player player in players)
        {
            if (player.State() != stateToCheck)
            {
                return false;
            }
        }
        return true;
    }

    public void OnPlayerStateChanged(Player player, Player.PlayerState newState)
    {
        if (!AllPlayersAtState(newState)) { return; }
        switch(newState)
        {
            case Player.PlayerState.WaitingToPlay:
                PlayCards();
                break;
            case Player.PlayerState.WaitingToDraw:
                UpdateText();
                StartNextTurn();
                break;
            default:
                Debug.Log("Lol");
                break;
        }
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
    public Sprite spriteForCard(CardType card)
    {
        return deckInfo.byType(card).cardSprite;
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
    Player GetNeighbor(Player player)
    {
        LinkedListNode<Player> node = players.Find(player);
        if (passLeft)
        {
            return (node.Previous != null) ? node.Previous.Value : players.Last.Value;
        }
        else //passRight
        {
            return (node.Next != null) ? node.Next.Value : players.First.Value;
        }
    }
    
    void GetPlayers()
    {
        foreach (Player player in GetComponentsInChildren<Player>())
        {
            players.AddLast(player);
            player.dealHand(drawHand(8));
            player.evtStateChanged += OnPlayerStateChanged;
        }
    }
    public void finishRound()
    {
        scoreCards.scoreRolls();
        UpdateText();
        if (roundNumber < 3)
        {
            continueDialog.ShowDialog("Round " + roundNumber + " complete!\n"+scoreCards.SortByScore().First().Name()+" In the lead", "Continue", StartNextRound);
        }
        else if (roundNumber == 3)
        {
            scoreCards.scorePuddings();
            continueDialog.ShowDialog("Game Complete!\n" + scoreCards.SortByScore().First().Name() + " Wins!","Play Again",Reset,"Exit",Application.Quit);
        }
        roundNumber++;
        passLeft = !passLeft;
    }

    public void Reset()
    {
        roundNumber = 1;
        Populate();
        foreach (Player player in players)
        {
            player.reset();
            player.dealHand(drawHand(8));
        }
        UpdateText();
    }

    private void PlayCards()
    {
        foreach (Player player in players)
        {
            player.playCard();
        }
    }

    public void StartNextTurn()
    {
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
                GetNeighbor(player).dealHand(player.passedCards);
            }
        }
        else
        {
            finishRound();
        }
    }

    private void CleanUpRound()
    {
        foreach (Player player in players)
        {
            player.clearCards(true);
        }

    }

    public void StartNextRound()
    {
        CleanUpRound();
        foreach (Player player in players)
        {
            player.dealHand(drawHand(8));
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
        topCard = -1;
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
