using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DrinkData
{
    public string drinkName;
    public List<KeyCode> sequence;
    public int drinkPrice;

    public DrinkData(string name, List<KeyCode> keys, int price)
    {
        drinkName = name;
        sequence = keys;
        drinkPrice = price;
    }
}
