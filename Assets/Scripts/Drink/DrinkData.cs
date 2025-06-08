using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DrinkData
{
    public string drinkName;
    public GameObject drinkPrefab;
    public List<KeyCode> sequence;
    public List<KeyCode> selectKey;
    public int drinkPrice;

    public DrinkData(string name, GameObject prefab, List<KeyCode> keys, List<KeyCode> select, int price)
    {
        drinkName = name;
        drinkPrefab = prefab;
        sequence = keys;
        selectKey = select;
        drinkPrice = price;
    }
}
