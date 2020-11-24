using System;
using System.Collections.Generic;
using System.Linq;

public static class DiceValue
{
    public static Dictionary<int, int> diceValueDict = new Dictionary<int, int>()
    {
        {1, 0}, {2, 0}, {3, 0}, {4, 0}, {5, 0}, {6, 0}
    };

    public static List<int> diceImageList = new List<int>();

    public static void AddValue(int key)
    {
        diceValueDict[key]++;
    }

    public static void RemoveValue(int key)
    {
        diceValueDict[key]--;
    }

    public static List<int> GetValues()
    {
        return diceValueDict.Where(item => item.Value != 0).Select(item => item.Key).ToList();
    }

    public static void ResetValue()
    {
        for (int i = 0; i < diceValueDict.Count; i++)
        {
            diceValueDict[diceValueDict.Keys.ElementAt(i)] = 0;
        }
    }
}
