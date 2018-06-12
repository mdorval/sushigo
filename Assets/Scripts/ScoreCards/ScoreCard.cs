using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public interface IRankScorer : IRollScorer, IPuddingScorer { }

/// <summary>
/// A representation of the Score for a given player
/// </summary>
public class ScoreCard: IRankScorer
{
    /// <summary>
    /// A dictionary of roll cards and their values in Roll Count
    /// </summary>
    public static readonly Dictionary<CardType, int> RollValues = new Dictionary<CardType,int> { { CardType.Roll_Single, 1 }, { CardType.Roll_Double, 2 }, { CardType.Roll_Triple, 3 } };
    /// <summary>
    /// A dictionary of nigiri cards and their values in Points
    /// </summary>
    public static readonly Dictionary<CardType, float> NigiriValues = new Dictionary<CardType, float>() { { CardType.Nigiri_Egg, 1.0f }, { CardType.Nigiri_Salmon, 2.0f }, { CardType.Nigiri_Squid, 3.0f } };

    /// <summary>
    /// Creates a Score
    /// </summary>
    public ScoreCard()
    {
    }

    /// <summary>
    /// Sets the username for the player
    /// </summary>
    /// <param name="userName">The player's name</param>
    public void SetUserName(string userName)
    {
        _userName = userName;
    }
    /// <summary>
    /// Sets the color for the player's scoreboard
    /// </summary>
    /// <param name="textColor">The player's color</param>
    public void SetColor(Color textColor)
    {
        _color = textColor;
    }

    private Color _color;
    private string _userName;
    private int _score = 0;
    private int _rolls = 0;
    private int _puddings = 0;
    private int _puddingPotentialScore = 0;
    private int _rollPotentialScore = 0;
    private RankingScoreCard _rankingScoreCard = null;

    /// <summary>
    /// A readonly version of this scorecard
    /// For making roll and pudding predictions
    /// </summary>
    /// <returns>Ranked Score Card</returns>
    public IRankScorer GetRankScorer()
    {
        if (_rankingScoreCard == null)
        {
            _rankingScoreCard = new RankingScoreCard(this);
        }
        return _rankingScoreCard;
    }

    /// <summary>
    /// Adds To the Score
    /// </summary>
    /// <param name="scoreCount">Score to Add</param>
    public void AddToScore(int scoreCount)
    {
        _score += scoreCount;
    }

    /// <summary>
    /// Adds to the pudding count
    /// </summary>
    /// <param name="puddingCount">pudding</param>
    public void AddToPuddings(int puddingCount)
    {
        _puddings += puddingCount;
    }

    /// <summary>
    /// Adds to the roll count
    /// </summary>
    /// <param name="rollCard">the Card to add</param>
    public void AddToRolls(CardType rollCard)
    {
        _rolls += RollValues[rollCard];
    }
    /// <summary>
    /// Clears the Roll Count
    /// </summary>
    public void ClearRolls()
    {
        _rolls = 0;
        _rollPotentialScore = 0;
    }
    /// <summary>
    /// Clears all numbers on scorecard
    /// </summary>
    public void Clear()
    {
        _score = 0;
        _puddings = 0;
        _rolls = 0;
        _rollPotentialScore = 0;
        _puddingPotentialScore = 0;
    }

    /// <summary>
    /// The Name associated with this ScoreCard
    /// </summary>
    /// <returns>Name</returns>
    public string Name() { return _userName; }

    /// <summary>
    /// The Color associated with this ScoreCard
    /// </summary>
    /// <returns>Color</returns>
    public Color TextColor() { return _color; }

    /// <summary>
    /// The Current Score for this ScoreCard
    /// </summary>
    /// <returns>Score</returns>
    public int Score() { return _score;  }

    /// <summary>
    /// The Rolls associated with this ScoreCard
    /// </summary>
    /// <returns>Roll Count</returns>
    public int Rolls() { return _rolls; }

    /// <summary>
    /// The Puddings associated with this ScoreCard
    /// </summary>
    /// <returns>Pudding Count</returns>
    public int Puddings() { return _puddings; }

    /// <summary>
    /// the potential score of the rolls played this round 
    /// (potential because it could change before end of round)
    /// </summary>
    /// <returns>Roll Score</returns>
    public int GetRollsPotentialScore() { return _rollPotentialScore; }

    /// <summary>
    /// the potential score of the puddings played this game
    /// (potential because it could change before end of game
    /// </summary>
    /// <returns>Pudding Score</returns>
    public int GetPuddingPotentialScore() { return _puddingPotentialScore; }

    /// <summary>
    /// Sets the potential roll score. This score is added at the end of round to the player's score
    /// In the interim it is used for scoreboards and AI
    /// </summary>
    /// <param name="rollScore"></param>
    public void SetRollsPotentialScore(int rollScore) { _rollPotentialScore = rollScore; }

    /// <summary>
    /// Sets the potential pudding score. This score is added at the end of game to the player's score
    /// In the interim it is used for scoreboards and AI
    /// </summary>
    /// <param name="puddingScore"></param>
    public void SetPuddingPotentialScore(int puddingScore) { _puddingPotentialScore = puddingScore; }

    /// <summary>
    /// Adds the rollPotentialScore to the actual score
    /// </summary>
    public void ScoreRolls()
    {
        _score += _rollPotentialScore;
        //Rolls are cleared at a different time for UI purposes
    }

    /// <summary>
    /// Adds the puddingPotentialScore to the actual score
    /// </summary>
    public void ScorePuddings()
    {
        _score += _puddingPotentialScore;
        //puddings are never cleared for UI purposes. Reset() handles clearing for a new game
    }

    /// <summary>
    /// Creates a new pudding predictor for AI to predict possible values of playing a pudding card
    /// </summary>
    /// <param name="extraPuddingCount">extra puddings to add</param>
    /// <returns>Pudding Predictor</returns>
    public IPuddingScorer MakePuddingPredictor(int extraPuddingCount)
    {
        return new PredictPuddingScoreCard(this.Puddings() + extraPuddingCount); 
    }

    /// <summary>
    /// Creates a new roll predictor for AI to predict possible values of playing a roll card
    /// </summary>
    /// <param name="extraRollCard">Card to add to rolls</param>
    /// <returns>Roll Predictor</returns>
    public IRollScorer MakeRollsPredictor(CardType extraRollCard)
    {
        return new PredictRollScoreCard(this.Rolls() + RollValues[extraRollCard]);
    }

    /// <summary>
    /// Special Class that is used for predictions when not the target (i.e. opponent)
    /// It is read-only, so it can be reused for every prediction 
    /// And it simply echoes a ScoreCard's values
    /// </summary>
    class RankingScoreCard : IRankScorer
    {
        private ScoreCard _parent;
        public RankingScoreCard(ScoreCard parent)
        {
            _parent = parent;
        }

        public int GetPuddingPotentialScore()
        {
            return ((IRankScorer)_parent).GetPuddingPotentialScore();
        }

        public int GetRollsPotentialScore()
        {
            return ((IRankScorer)_parent).GetRollsPotentialScore();
        }

        public int Puddings()
        {
            return _parent.Puddings();
        }

        public int Rolls()
        {
            return _parent.Rolls();
        }

        public void SetPuddingPotentialScore(int puddingScore)
        {
            //Read-only, only used for predicting scores
        }

        public void SetRollsPotentialScore(int rollScore)
        {
            //Read-only, only used for predicting scores
        }
    }
}

public static class ScoreCardsExtension
{
    /// <summary>
    /// Sorts the Cards by score and name, for scoreboard purposes
    /// </summary>
    /// <typeparam name="T">ScoreCard</typeparam>
    /// <param name="scoreCards">ScoreCards To sort</param>
    /// <returns>Sorted ScoreCards</returns>
    public static IOrderedEnumerable<T> SortByScoreAndName<T>(this IEnumerable<T> scoreCards) where T: ScoreCard
    {
        return scoreCards.OrderByDescending(r => r.Score()).ThenBy(r => r.Name());
    }
}
