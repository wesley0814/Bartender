using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrinkManager : MonoBehaviour
{
    public List<DrinkData> drinkList = new List<DrinkData>();

    public DrinkData GetDrinkByName(string name)
    {
        return drinkList.Find(d => d.drinkName == name);
    }

    public DrinkData FindDrinkByKeys(List<KeyCode> inputSequence)
    {
        foreach (var drink in drinkList)
        {
            if (drink.selectKey.Count != inputSequence.Count) continue;

            bool match = true;
            for (int i = 0; i < inputSequence.Count; i++)
            {
                if (drink.selectKey[i] != inputSequence[i])
                {
                    match = false;
                    break;
                }
            }

            if (match) return drink;
        }

        return null;
    }
}
