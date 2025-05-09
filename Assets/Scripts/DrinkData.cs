using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DrinkData
{
    public string drinkName;
    public List<KeyCode> sequence;

    public DrinkData(string name, List<KeyCode> keys)
    {
        drinkName = name;
        sequence = keys;
    }
}
