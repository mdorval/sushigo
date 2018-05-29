using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ScoreCards: List<ScoreCard>
{
    private Dictionary<ScoreCard,int> rollScore = new Dictionary<ScoreCard,int>();
    private Dictionary<ScoreCard,int> puddingScore = new Dictionary<ScoreCard,int>();
    public void UpdateRankScores()
    {
        UpdateRollScores();
        UpdatePuddingScores();
    }

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

    public void scorePuddings()
    {
        UpdatePuddingScores();
        foreach (ScoreCard scoreCard in this)
        {
            scoreCard.addToPuddings(PuddingScore(scoreCard));
        }
    }

    public void scoreRolls()
    {
        UpdateRollScores();
        //Add Scores
        foreach (ScoreCard scoreCard in this)
        {
            scoreCard.addToScore(RollScore(scoreCard));
            scoreCard.clearRolls();
            rollScore[scoreCard] = 0;
        }
    }

    public IOrderedEnumerable<ScoreCard> SortByScore()
    {
        return this.OrderByDescending(r => r.Score()).ThenBy(r => r.Name());
    }
    public IOrderedEnumerable<ScoreCard> SortByRolls()
    {
        return this.OrderByDescending(r => r.Rolls()).ThenBy(r => r.Name());
    }
    public IOrderedEnumerable<ScoreCard> SortByPudding()
    {
        return this.OrderByDescending(r => r.Puddings()).ThenBy(r => r.Name());
    }
    public int RollScore(ScoreCard cardToCheck)
    {
        return rollScore[cardToCheck];
    }
    public int PuddingScore(ScoreCard cardToCheck)
    {
        return puddingScore[cardToCheck];
    }

}

public class ScoreCard
{
    public ScoreCard(string userName, Color textColor)
    {
        _htmlColor = ColorUtility.ToHtmlStringRGB(textColor);
        _userName = userName;
    }
    private string _htmlColor;
    private string _userName;
    private int _score = 0;
    private int _rolls = 0;
    private int _puddings = 0;
    public void addToScore(int scoreCount)
    {
        _score += scoreCount;
    }
    public void addToPuddings(int puddingCount)
    {
        _puddings += puddingCount;
    }
    public void addToRolls(int rollCount)
    {
        _rolls += rollCount;
    }
    public void clearRolls()
    {
        _rolls = 0;
    }
    public void clear()
    {
        _score = 0;
        _puddings = 0;
        _rolls = 0;
    }

    public string Name() { return _userName; }
    public string HtmlColor() { return _htmlColor; }
    public int Score() { return _score;  }
    public int Rolls() { return _rolls; }
    public int Puddings() { return _puddings; }
}
