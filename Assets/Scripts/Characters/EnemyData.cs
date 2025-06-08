using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EnemyData
{
    public GameObject prefab;
    public int maxHealth;
    public List<EnemySkill> skills;
}
