using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DrinkData
{
    public string drinkName;
    public Sprite drinkSprite;
    public List<KeyCode> sequence;
    public int drinkPrice;

    public DrinkData(string name, Sprite sprite, List<KeyCode> keys, int price)
    {
        drinkName = name;
        drinkSprite = sprite;
        sequence = keys;
        drinkPrice = price;
    }
}
