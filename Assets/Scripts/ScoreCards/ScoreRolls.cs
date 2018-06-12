using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public interface IRollScorer
{
    /// <summary>
    /// The count of rolls for this scorer
    /// </summary>
    /// <returns>roll count</returns>
    int Rolls();

    /// <summary>
    /// Sets the calculated score of these rolls for end of round
    /// Used by UpdateRollScores() to update the individual scorer's calculated value
    /// </summary>
    /// <param name="rollScore">the calulated score</param>
    void SetRollsPotentialScore(int rollScore);

    /// <summary>
    /// Sets the calculated score of these rolls for end of round
    /// </summary>
    /// <returns>The calculated score</returns>
    int GetRollsPotentialScore();
}

public static class ScoreRolls
{
    /// <summary>
    /// Sorts by roll count
    /// </summary>
    /// <typeparam name="T">IRollScorer</typeparam>
    /// <param name="rollScorers">collection of scorers</param>
    /// <returns>sorted roll scorers</returns>
    public static IOrderedEnumerable<T> SortByRolls<T>(this IEnumerable<T> rollScorers) where T: IRollScorer
    {
        return rollScorers.OrderByDescending(r => r.Rolls());
    }

    /// <summary>
    /// Updates the roll calculated scores
    /// Roll point values are relative to other players so this must be calculated as a group
    /// Used for score calculations and AI prediction calculations
    /// </summary>
    /// <typeparam name="T">IRollScorer</typeparam>
    /// <param name="rollScorers">the list of scorers</param>
    public static void UpdateRollScores<T>(this IEnumerable<T> rollScorers) where T: IRollScorer
    {
        // Roll rules:
        // The player with the most rolls scores 6
        // If multiple players tie for most, split points evenly (ignoring remainder) and NO SECOND PLACE POINTS AWARDED
        // The player with second most rolls scores 3
        // If multiple players tie for second, split points evenly (ignoring remainder)
        // Players must have at least 1 sushi roll to be counted

        //Zero out the values
        foreach(IRollScorer rollScorer in rollScorers)
        {
            rollScorer.SetRollsPotentialScore(0);
        }

        var rollGroupEnumerator = rollScorers.SortByRolls().GroupBy(r => r.Rolls()).GetEnumerator();

        //Check First
        if (rollGroupEnumerator.MoveNext())
        {
            if (rollGroupEnumerator.Current.Key == 0) { return; } //only score rolls > 0
            foreach (IRollScorer card in rollGroupEnumerator.Current)
            {
                //Integer divide throws away remainder
                card.SetRollsPotentialScore(6 / rollGroupEnumerator.Current.Count());
            }
            //Check Second only if exists, no tie for first, rolls > 0
            if (rollGroupEnumerator.Current.Count() == 1
                && rollGroupEnumerator.MoveNext()
                && rollGroupEnumerator.Current.Key > 0)
            {
                foreach (IRollScorer card in rollGroupEnumerator.Current)
                {
                    card.SetRollsPotentialScore(3 / rollGroupEnumerator.Current.Count());
                }
            }
        }
    }

}