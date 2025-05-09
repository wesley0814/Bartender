using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrinkManager : MonoBehaviour
{
    public List<DrinkData> drinkList = new List<DrinkData>();

    void Awake()
    {
        drinkList.Add(new DrinkData("Drink1", new List<KeyCode> { KeyCode.A, KeyCode.S, KeyCode.D, KeyCode.W, KeyCode.W, KeyCode.D }));
        drinkList.Add(new DrinkData("Drink2", new List<KeyCode> { KeyCode.W, KeyCode.D, KeyCode.A, KeyCode.S, KeyCode.D }));
    }

    public DrinkData GetDrinkByName(string name)
    {
        return drinkList.Find(c => c.drinkName == name);
    }
}
