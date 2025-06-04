using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    public Transform canvasTransform;

    public GameObject guardPrefab;
    public GameObject enemyPrefab;

    private Guard guard;
    private Enemy enemy;

    void Start()
    {
        GameObject guardGO = Instantiate(guardPrefab, canvasTransform);
        GameObject enemyGO = Instantiate(enemyPrefab, canvasTransform);

        guard = guardGO.GetComponent<Guard>();
        enemy = enemyGO.GetComponent<Enemy>();

        guardGO.GetComponent<RectTransform>().anchoredPosition = new Vector2(100, 0);
        enemyGO.GetComponent<RectTransform>().anchoredPosition = new Vector2(-100, 0);
    }

    public void DrinkBehavior(List<Drink.DrinkQueue> drinkList)
    {
        if (drinkList.Count != 3)
        {
            Debug.LogWarning("Attack requires exactly 3 drinks.");
            return;
        }

        string name1 = drinkList[0].drinkData.drinkName;
        string name2 = drinkList[1].drinkData.drinkName;
        string name3 = drinkList[2].drinkData.drinkName;

        int damage = 0;
        string effect = "";

        if (name1 == name2 && name2 == name3)
        {
            damage = 30;
            effect = "🔥 강한 공격!";
        }
        else if (name1 == name2 || name1 == name3 || name2 == name3)
        {
            damage = 20;
            effect = "⚡ 중간 공격!";
        }
        else
        {
            damage = 10;
            effect = "💨 약한 공격!";
        }

        Debug.Log(effect + damage);
        enemy.TakeDamage(damage);
        enemyAttack();
    }

    public void enemyAttack()
    {
        int damage = 20;
        guard.TakeDamage(damage);
    }
}

