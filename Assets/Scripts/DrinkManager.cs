using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrinkManager : MonoBehaviour
{
    public List<DrinkData> drinkList = new List<DrinkData>();

    void Awake()
    {
        // 음료와 가격 추가
        drinkList.Add(new DrinkData("Drink1", new List<KeyCode> { KeyCode.A, KeyCode.S, KeyCode.D, KeyCode.W, KeyCode.W, KeyCode.D }, 50));
        drinkList.Add(new DrinkData("Drink2", new List<KeyCode> { KeyCode.W, KeyCode.D, KeyCode.A, KeyCode.S, KeyCode.D }, 80));
    }

    public DrinkData GetDrinkByName(string name)
    {
        return drinkList.Find(c => c.drinkName == name);
    }
}
