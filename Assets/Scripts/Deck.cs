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

    public GameObject playingCardPrefab;
    public GameObject cardPackPrefab;
    public GameObject cpuHandPrefab;

    public GameObject scorePanel;
    public GameObject rollPanel;
    public GameObject puddingPanel;

    public DeckInfo deckInfo;
    public Text scoreText;
    public ContinueDialog continueDialog;

    private bool passLeft = true;

    private static Deck instance = null;
    public static Deck Instance()
    {
        if (instance == null)
        {
            instance = GameObject.FindObjectOfType<Deck>();
            if (instance == null)
            {
                Debug.LogError("Deck not found!");
            }
        }
        return instance;
    }

    public bool PassingLeft()
    {
        return passLeft;
    }
    void Start()
    {
        Populate();
        GetPlayers();
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
            case Player.PlayerState.WaitingForNextRound:
                UpdateText();
                FinishRound();
                break;
        }
    }

    public void AddScoreCard(ScoreCard card)
    {
        scoreCards.Add(card);
        UpdateText();
    }

    delegate string valueString(ScoreCard card);
    delegate IEnumerable<ScoreCard> scoreCardSort(ScoreCards scoreCards);
    delegate int potentialPoints(ScoreCard card, ScoreCards cards);
    void FillScorePanel(GameObject scorePanel,ScoreCards scoreCards, scoreCardSort sortFunction, valueString valueStringFunction,potentialPoints potentialPointsFunction = null)
    {
        var scorePanels = scorePanel.GetComponentsInChildren<Text>();
        int counter = 0;
        foreach (ScoreCard scoreCard in sortFunction(scoreCards))
        {
            Text tplayer = scorePanels[counter++];
            tplayer.color = scoreCard.TextColor();
            tplayer.text = scoreCard.Name();
            Text tscore = scorePanels[counter++];
            tscore.color = scoreCard.TextColor();
            tscore.text = valueStringFunction(scoreCard);
            if(potentialPointsFunction != null)
            {
                int value = potentialPointsFunction(scoreCard,scoreCards);
                //ignore value == 0
                if (value > 0)
                {
                    tscore.text += "  <color='#00FF00'>+" + value.ToString() + "</color>";
                }
                else if (value < 0)
                {
                    tscore.text += "  <color='#FF0000'>" + value.ToString() + "</color>";
                }
            }
        }
    }

    void UpdateText()
    {
        scoreCards.UpdateRankScores();
        //ScoreCards
        FillScorePanel(scorePanel, scoreCards, 
            scoreCards => scoreCards.SortByScore(),
            scoreCard => scoreCard.Score().ToString());
        //Puddings
        FillScorePanel(puddingPanel, scoreCards,
            scoreCards => scoreCards.SortByPudding(),
            scoreCard => scoreCard.Puddings().ToString(),
            (scoreCard,scoreCards) => scoreCards.PuddingScore(scoreCard));
        //Rolls
        FillScorePanel(rollPanel, scoreCards,
            scoreCards => scoreCards.SortByRolls(),
            scoreCard => scoreCard.Rolls().ToString(),
            (scoreCard, scoreCards) => scoreCards.RollScore(scoreCard));
    }
    public Player GetNeighbor(Player player)
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
            player.evtStateChanged += OnPlayerStateChanged;
        }
        foreach (Player player in GetComponentsInChildren<Player>())
        {
            player.DealHand(DrawHand(8));
        }
    }
    public void FinishRound()
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
            player.Reset();
            player.DealHand(DrawHand(8));
        }
        UpdateText();
    }

    private void PlayCards()
    {
        foreach (Player player in players)
        {
            player.PlayCard();
        }
    }

    public void StartNextTurn()
    {
        foreach (Player player in players)
        {
            player.DrawCardPack();
        }
    }

    private void CleanUpRound()
    {
        foreach (Player player in players)
        {
            player.ClearCards(true);
        }

    }

    public void StartNextRound()
    {
        CleanUpRound();
        foreach (Player player in players)
        {
            player.DealHand(DrawHand(8));
        }
    }

    private List<CardType> DrawHand(int size)
    {
        List<CardType> hand = new List<CardType>();
        for (int i=0;i<size;i++)
        {
            hand.Add(drawPile[topCard--]);
        }
        return hand;
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
