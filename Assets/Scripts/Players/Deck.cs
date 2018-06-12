using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.U2D;
using System.Linq;

/// <summary>
/// Manager for Sushi Go. Controls the global state of the game
/// </summary>
public class Deck : MonoBehaviour {
    private DrawPile drawPile;
    private LinkedList<Player> players = new LinkedList<Player>();
    private List<ScoreCard> scoreCards = new List<ScoreCard>();
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

    /// <summary>
    /// Gets the current instance of the deck
    /// </summary>
    /// <returns>Deck</returns>
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

    /// <summary>
    /// Starts the game
    /// </summary>
    void Start()
    {
        drawPile = new DrawPile(deckInfo);
        InitPlayers();
    }

    /// <summary>
    /// Called whenever one of this Deck's Players plays a card
    /// Subscribe to this event to hear all of the CardPlayed events for all children
    /// </summary>
    public PlayerEvent.CardPlayed evtCardPlayed = null;

    /// <summary>
    /// This hooks up to all of the players evtCardPlayed events and echos them all
    /// </summary>
    /// <param name="player">Player who played card</param>
    /// <param name="card">Card played</param>
    private void OnPlayerCardPlayed(Player player, CardType card)
    {
        if (evtCardPlayed != null)
        {
            evtCardPlayed(player, card);
        }
    }

    /// <summary>
    /// This returns readonly versions of the scorecards, not including the passed-in player
    /// This allows a predictor to calculate the potential value of something based on this round's
    /// current scores
    /// </summary>
    /// <param name="mainPlayer">Player to ignore</param>
    /// <returns>List of ReadOnly ScoreCards</returns>
    public IEnumerable<IRankScorer> GetOpponentPredictorScorers(Player mainPlayer)
    {
        List<IRankScorer> scorers = new List<IRankScorer>();
        foreach (Player player in players)
        {
            if (player == mainPlayer) continue;
            scorers.Add(player.scoreCard.GetRankScorer());
        }
        return scorers;
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
    delegate IEnumerable<ScoreCard> scoreCardSort(List<ScoreCard> scoreCards);
    delegate int potentialPoints(ScoreCard card);
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
                int value = potentialPointsFunction(scoreCard);
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
        scoreCards.UpdateRollScores();
        scoreCards.UpdatePuddingScores();
        //ScoreCards
        FillScorePanel(scorePanel,  
            scoreCards => scoreCards.SortByScoreAndName(),
            scoreCard => scoreCard.Score().ToString());
        //Puddings
        FillScorePanel(puddingPanel, 
            scoreCards => scoreCards.SortByPudding().ThenBy(r => r.Name()),
            scoreCard => scoreCard.Puddings().ToString(),
            scoreCard => scoreCard.GetPuddingPotentialScore());
        //Rolls
        FillScorePanel(rollPanel, 
            scoreCards => scoreCards.SortByRolls().ThenBy(r => r.Name()),
            scoreCard => scoreCard.Rolls().ToString(),
            scoreCard => scoreCard.GetRollsPotentialScore());
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
    /// Calculates how many turns it takes for playerA's deck to get to playerB
    /// </summary>
    /// <param name="playerA">The first player</param>
    /// <param name="playerB">The player we want to calculate towards</param>
    /// <returns></returns>
    public int CalculatePacksBetween(Player playerA, Player playerB)
    {
        int count = 0;
        Player playerSearch = playerA;
        do
        {
            playerSearch = GetNeighbor(playerSearch);
            count++;
        } while (playerSearch != playerB);
        return count;
    }

    /// <summary>
    /// Initializes Deck with the players
    /// </summary>
    void InitPlayers()
    {
        foreach (Player player in GetComponentsInChildren<Player>())
        {
            players.AddLast(player);
            player.evtStateChanged += OnPlayerStateChanged;
            player.evtCardPlayed += OnPlayerCardPlayed;
        }
        foreach (Player player in GetComponentsInChildren<Player>())
        {
            player.DealHand(drawPile.DrawHand(8));
        }
    }
    /// <summary>
    /// Finishes a given round
    /// </summary>
    public void FinishRound()
    {
        foreach (ScoreCard card in scoreCards)
        {
            card.ScoreRolls();
        }
        if (roundNumber < 3)
        {
            continueDialog.ShowDialog("Round " + roundNumber + " complete!\n"+scoreCards.SortByScoreAndName().First().Name()+" In the lead", "Continue", StartNextRound);
        }
        else if (roundNumber == 3)
        {
            foreach (ScoreCard card in scoreCards)
            {
                card.ScorePuddings();
            }
            continueDialog.ShowDialog("Game Complete!\n" + scoreCards.SortByScoreAndName().First().Name() + " Wins!","Play Again",Reset,"Exit",Application.Quit);
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
        drawPile.Populate();
        foreach (Player player in players)
        {
            player.Reset();
            player.DealHand(drawPile.DrawHand(8));
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
            player.DealHand(drawPile.DrawHand(8));
        }
    }
}
