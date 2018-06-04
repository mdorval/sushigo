using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.U2D;
using System.Linq;

/// <summary>
/// Manager for Sushi Go. Controls the global state of the game and the Deck of random cards
/// </summary>
public class Deck : MonoBehaviour {
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
    void Start()
    {
        Populate();
        GetPlayers();
    }

    /// <summary>
    /// Called whenever a Player's state changes. If every player is at the same waiting state, Deck triggers the next step
    /// </summary>
    /// <param name="player">The Player </param>
    /// <param name="newState">The state the player was in</param>
    private void OnPlayerStateChanged(Player player, Player.PlayerState newState)
    {
        foreach (Player playercheck in players)
        {
            if (playercheck.State() != newState)
            {
                return;
            }
        }
        switch (newState)
        {
            case Player.PlayerState.WaitingToPlay:
                foreach (Player playerToPlay in players)
                {
                    playerToPlay.PlayCard();
                }
                break;
            case Player.PlayerState.WaitingToDraw:
                UpdateText();
                foreach (Player playerToDraw in players)
                {
                    playerToDraw.DrawCardPack();
                }
                break;
            case Player.PlayerState.WaitingForNextRound:
                FinishRound();
                break;
        }
    }

    /// <summary>
    /// Adds a ScoreCard for tracking to the Deck
    /// </summary>
    /// <param name="card">The Card To Add</param>
    public void AddScoreCard(ScoreCard card)
    {
        scoreCards.Add(card);
        UpdateText();
    }

    delegate string valueString(ScoreCard card);
    delegate IEnumerable<ScoreCard> scoreCardSort(ScoreCards scoreCards);
    delegate int potentialPoints(ScoreCard card, ScoreCards cards);
    /// <summary>
    /// Fills a given score panel with Score information
    /// </summary>
    /// <param name="scorePanel">The Panel to Fill</param>
    /// <param name="sortFunction">The function to sort the players by</param>
    /// <param name="valueStringFunction">The function to show the value given the scorecard</param>
    /// <param name="potentialPointsFunction">(Optional) The function to show the potential points for this rank</param>
    void FillScorePanel(GameObject scorePanel, scoreCardSort sortFunction, valueString valueStringFunction,potentialPoints potentialPointsFunction = null)
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

    /// <summary>
    /// Updates the Score Text In The UI
    /// </summary>
    void UpdateText()
    {
        scoreCards.UpdateRankScores();
        //ScoreCards
        FillScorePanel(scorePanel,  
            scoreCards => scoreCards.SortByScore(),
            scoreCard => scoreCard.Score().ToString());
        //Puddings
        FillScorePanel(puddingPanel, 
            scoreCards => scoreCards.SortByPudding(),
            scoreCard => scoreCard.Puddings().ToString(),
            (scoreCard,scoreCards) => scoreCards.PuddingScore(scoreCard));
        //Rolls
        FillScorePanel(rollPanel, 
            scoreCards => scoreCards.SortByRolls(),
            scoreCard => scoreCard.Rolls().ToString(),
            (scoreCard, scoreCards) => scoreCards.RollScore(scoreCard));
    }

    /// <summary>
    /// Whether this round players are passing left or right - alternates every round
    /// </summary>
    /// <returns>if we are passing left</returns>
    public bool PassingLeft()
    {
        return passLeft;
    }

    /// <summary>
    /// Gets the neighbor for the given player. Neighbor will be different dependent on round
    /// </summary>
    /// <param name="player">The Player to check the neighbor for</param>
    /// <returns>The player's neighbor</returns>
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
    
    /// <summary>
    /// Initializes Deck with the players
    /// </summary>
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
    /// <summary>
    /// Finishes a given round
    /// </summary>
    public void FinishRound()
    {
        scoreCards.ScoreRolls();
        if (roundNumber < 3)
        {
            continueDialog.ShowDialog("Round " + roundNumber + " complete!\n"+scoreCards.SortByScore().First().Name()+" In the lead", "Continue", StartNextRound);
        }
        else if (roundNumber == 3)
        {
            scoreCards.ScorePuddings();
            continueDialog.ShowDialog("Game Complete!\n" + scoreCards.SortByScore().First().Name() + " Wins!","Play Again",Reset,"Exit",Application.Quit);
        }
        UpdateText();
        roundNumber++;
        passLeft = !passLeft;
    }

    /// <summary>
    /// Resets the game 
    /// </summary>
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
    
    /// <summary>
    /// Starts the next round of play
    /// </summary>
    public void StartNextRound()
    {
        foreach (Player player in players)
        {
            player.ClearCards(true);
            player.scoreCard.ClearRolls();
            UpdateText();
        }
        foreach (Player player in players)
        {
            player.DealHand(DrawHand(8));
        }
    }

    /// <summary>
    /// Draws a hand of random cards remaining in the deck
    /// </summary>
    /// <param name="size">The number of cards to draw</param>
    /// <returns>List of Cards</returns>
    private List<CardType> DrawHand(int size)
    {
        List<CardType> hand = new List<CardType>();
        for (int i=0;i<size;i++)
        {
            hand.Add(drawPile[topCard--]);
        }
        return hand;
    }
	
    /// <summary>
    /// Populates and Shuffles the Deck
    /// </summary>
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
