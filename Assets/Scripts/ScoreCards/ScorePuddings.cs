using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public interface IPuddingScorer
{
    /// <summary>
    /// Number of Puddings for this scorer
    /// </summary>
    /// <returns>Count of puddings</returns>
    int Puddings();
    
    /// <summary>
    /// Sets the calculated score of these puddings for end of game
    /// Used by UpdatePuddingScores() to update the individual scorer's calculated value
    /// </summary>
    /// <param name="puddingScore"></param>
    void SetPuddingPotentialScore(int puddingScore);

    /// <summary>
    /// Calculated score of these puddings for end of game
    /// </summary>
    /// <returns>PuddingPotentialScore</returns>
    int GetPuddingPotentialScore();
}

public static class ScorePuddings
{
    /// <summary>
    /// Sorts a collection of PuddingScorers by their count of puddings
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="puddingScorers">Pudding Scorers</param>
    /// <returns>Sorted enumerable</returns>
    public static IOrderedEnumerable<T> SortByPudding<T>(this IEnumerable<T> puddingScorers) where T: IPuddingScorer
    {
        return puddingScorers.OrderByDescending(r => r.Puddings());
    }
    /// <summary>
    /// Updates the pudding calculated scores
    /// Pudding point values are relative to other players so this must be calculated as a group
    /// Used for score calculations and AI prediction calculations
    /// </summary>
    /// <typeparam name="T">IPuddingScorer</typeparam>
    /// <param name="puddingScorers">The scorers to calculate and update</param>
    public static void UpdatePuddingScores<T>(this IEnumerable<T> puddingScorers) where T: IPuddingScorer
    {
        //Pudding Rules:
        //The Player with the most puddings scores 6
        //If multiple players tie for most, split points evenly (ignoring remainder)
        //The Player with the least puddings scores 6
        //If multiple players tie for least, split points evenly (ignoring remainder)
        //Special case: if all players have same number of pudding cards, no one scores anything

        //Zero out the values
        foreach(IPuddingScorer puddingScorer in puddingScorers)
        {
            puddingScorer.SetPuddingPotentialScore(0);
        }

        var puddingGroup = puddingScorers.SortByPudding().GroupBy(r => r.Puddings());
        if (puddingGroup.Count() <= 1) { return; } //everyone has same amount of puddings
        //score everyone in first
        foreach (IPuddingScorer puddingScorer in puddingGroup.First())
        {
            puddingScorer.SetPuddingPotentialScore(6 / puddingGroup.First().Count());
        }
        //score everyone in last
        foreach (IPuddingScorer puddingScorer in puddingGroup.Last())
        {
            puddingScorer.SetPuddingPotentialScore(-6 / puddingGroup.Last().Count());

        }
    }
}