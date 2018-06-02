using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ScoreCards: List<ScoreCard>
{
    private Dictionary<ScoreCard,int> rollScore = new Dictionary<ScoreCard,int>();
    private Dictionary<ScoreCard,int> puddingScore = new Dictionary<ScoreCard,int>();
    /// <summary>
    /// Updates the Roll and Pudding Scores
    /// Roll and Pudding Scores are relative to other players, 
    /// so they need to be kept and calculated in the collection
    /// </summary>
    public void UpdateRankScores()
    {
        UpdateRollScores();
        UpdatePuddingScores();
    }

    /// <summary>
    /// Updates the Roll Scores
    /// </summary>
    private void UpdateRollScores()
    {
        // Roll rules:
        // The player with the most rolls scores 6
        // If multiple players tie for most, split points evenly (ignoring remainder) and NO SECOND PLACE POINTS AWARDED
        // The player with second most rolls scores 3
        // If multiple players tie for second, split points evenly (ignoring remainder)
        // Players must have at least 1 sushi roll to be counted

        //Zero out the values
        this.ForEach(card => rollScore[card] = 0);

        var rollGroupEnumerator = this.SortByRolls().GroupBy(r => r.Rolls()).GetEnumerator();

        //Check First
        if (rollGroupEnumerator.MoveNext())
        {
            if (rollGroupEnumerator.Current.Key == 0) { return; } //only score rolls > 0
            foreach (ScoreCard card in rollGroupEnumerator.Current)
            {
                //Integer divide throws away remainder
                rollScore[card] = 6 / rollGroupEnumerator.Current.Count();
            }
            //Check Second only if exists, no tie for first, rolls > 0
            if (rollGroupEnumerator.Current.Count() == 1 
                && rollGroupEnumerator.MoveNext()
                && rollGroupEnumerator.Current.Key > 0)
            {
                foreach (ScoreCard card in rollGroupEnumerator.Current)
                {
                    rollScore[card] = 3 / rollGroupEnumerator.Current.Count();
                }
            }
        }
    }

    /// <summary>
    /// Updates the Pudding Scores
    /// </summary>
    private void UpdatePuddingScores()
    {
        //Pudding Rules:
        //The Player with the most puddings scores 6
        //If multiple players tie for most, split points evenly (ignoring remainder)
        //The Player with the least puddings scores 6
        //If multiple players tie for least, split points evenly (ignoring remainder)
        //Special case: if all players have same number of pudding cards, no one scores anything

        //Zero out the values
        this.ForEach(card => puddingScore[card] = 0);

        var puddingGroup = this.SortByPudding().GroupBy(r => r.Puddings());
        if (puddingGroup.Count() <= 1){ return; } //everyone has same amount of puddings
        foreach (ScoreCard card in puddingGroup.First())
        {
            puddingScore[card] = 6 / puddingGroup.First().Count();
        }
        foreach (ScoreCard card in puddingGroup.Last())
        {
            puddingScore[card] = -6 / puddingGroup.Last().Count();
        }
    }

    /// <summary>
    /// Moves the Pudding Scores to the actual ScoreCard
    /// </summary>
    public void ScorePuddings()
    {
        UpdatePuddingScores();
        foreach (ScoreCard scoreCard in this)
        {
            scoreCard.AddToScore(PuddingScore(scoreCard));
        }
    }

    /// <summary>
    /// Moves the Roll Scores to the actual Scorecard
    /// </summary>
    public void ScoreRolls()
    {
        UpdateRollScores();
        //Add Scores
        foreach (ScoreCard scoreCard in this)
        {
            scoreCard.AddToScore(RollScore(scoreCard));
            rollScore[scoreCard] = 0;
        }
    }

    /// <summary>
    /// Sorts by score, then name descending 
    /// </summary>
    /// <returns>Sorted Enumerable of Scorecards</returns>
    public IOrderedEnumerable<ScoreCard> SortByScore()
    {
        return this.OrderByDescending(r => r.Score()).ThenBy(r => r.Name());
    }

    /// <summary>
    /// Sorts by roll, then name descending 
    /// </summary>
    /// <returns>Sorted Enumerable of Scorecards</returns>
    public IOrderedEnumerable<ScoreCard> SortByRolls()
    {
        return this.OrderByDescending(r => r.Rolls()).ThenBy(r => r.Name());
    }

    /// <summary>
    /// Sorts by pudding, then name descending 
    /// </summary>
    /// <returns>Sorted Enumerable of Scorecards</returns>
    public IOrderedEnumerable<ScoreCard> SortByPudding()
    {
        return this.OrderByDescending(r => r.Puddings()).ThenBy(r => r.Name());
    }
    /// <summary>
    /// The Rolls points for the given card
    /// </summary>
    /// <param name="cardToCheck">Card to check</param>
    /// <returns>Number of rolls</returns>
    public int RollScore(ScoreCard cardToCheck)
    {
        return rollScore[cardToCheck];
    }

    /// <summary>
    /// The Puddings points for the given card
    /// </summary>
    /// <param name="cardToCheck">Card to check</param>
    /// <returns>Number of rolls</returns>
    public int PuddingScore(ScoreCard cardToCheck)
    {
        return puddingScore[cardToCheck];
    }

}
/// <summary>
/// A representation of the Score for a given player
/// </summary>
public class ScoreCard
{
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
    /// <param name="rollCount">rolls</param>
    public void AddToRolls(int rollCount)
    {
        _rolls += rollCount;
    }
    /// <summary>
    /// Clears the Roll Count
    /// </summary>
    public void ClearRolls()
    {
        _rolls = 0;
    }
    /// <summary>
    /// Clears all numbers on scorecard
    /// </summary>
    public void Clear()
    {
        _score = 0;
        _puddings = 0;
        _rolls = 0;
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
}
