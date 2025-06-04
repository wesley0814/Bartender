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
}
