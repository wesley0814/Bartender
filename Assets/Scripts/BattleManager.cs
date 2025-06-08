using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleManager : MonoBehaviour
{
    public Transform canvasTransform;
    public Transform guardSpawnPoint;
    public Transform enemySpawnPoint;
    public Transform wavePopupPoint;
    public GameObject wavePopupPrefab;
    public GameObject guardPrefab;
    public List<EnemyData> enemyDataList;
    public Text stageText;
    public Text WaveText;

    private List<List<EnemyData>> stages = new List<List<EnemyData>>();

    private Guard guard;
    private Enemy enemy;

    private int currentStageIndex = 0;
    private int currentWaveIndex = 0;

    private Coroutine enemyAttackRoutine;

    void Start()
    {
        GameObject guardGO = Instantiate(guardPrefab, canvasTransform);
        guard = guardGO.GetComponent<Guard>();
        guardGO.GetComponent<RectTransform>().anchoredPosition = (Vector2)guardSpawnPoint.localPosition;

        InitializeStages();
        StartStage(0);
    }

    void InitializeStages()
    {
        stages.Add(new List<EnemyData>
        {
            enemyDataList[1],
            enemyDataList[1],
            enemyDataList[2],
        });

        stages.Add(new List<EnemyData>
        {
            enemyDataList[1],
            enemyDataList[2],
            enemyDataList[3],
        });

        stages.Add(new List<EnemyData>
        {
            enemyDataList[1],
            enemyDataList[2],
            enemyDataList[3],
            enemyDataList[0],
        });

    }

    void StartStage(int stageIndex)
    {
        if (stageIndex >= stages.Count)
        {
            Debug.Log("🏁 모든 스테이지 완료!");
            return;
        }

        guard.Init();

        currentStageIndex = stageIndex;
        currentWaveIndex = 0;

        Debug.Log($"📘 스테이지 {stageIndex + 1} 시작!");
        UpdateStageUI($"Stage {currentStageIndex + 1}");
        NextWave();
    }

    public void NextWave()
    {
        if (enemy != null)
        {
            Destroy(enemy.gameObject);
            if (enemyAttackRoutine != null) StopCoroutine(enemyAttackRoutine);
        }

        var currentStage = stages[currentStageIndex];

        if (currentWaveIndex >= currentStage.Count)
        {
            Debug.Log($"✅ 스테이지 {currentStageIndex + 1} 완료!");
            currentStageIndex++;
            StartStage(currentStageIndex);
            return;
        }

        var data = currentStage[currentWaveIndex];
        ShowWavePopup(currentWaveIndex + 1);
        GameObject enemyGO = Instantiate(data.prefab, canvasTransform);
        enemy = enemyGO.GetComponent<Enemy>();
        enemy.Init(data);
        enemyGO.GetComponent<RectTransform>().anchoredPosition = (Vector2)enemySpawnPoint.localPosition;

        enemy.OnEnemyDefeated += NextWave;

        Debug.Log($"🚨 웨이브 {currentWaveIndex + 1} 시작!");
        UpdateWaveUI($"Wave {currentWaveIndex + 1}");

        enemyAttackRoutine = StartCoroutine(EnemyAttackLoop(data));
        currentWaveIndex++;
    }

    IEnumerator EnemyAttackLoop(EnemyData data)
    {
        while (enemy != null)
        {
            int ran = Random.Range(0, data.skills.Count);
            float cooldown = data.skills[ran].cooldown;

            float elapsed = 0f;
            while (elapsed < cooldown)
            {
                elapsed += Time.deltaTime;
                if (enemy != null && enemy.cooldownBarImage != null)
                {
                    enemy.cooldownBarImage.fillAmount = elapsed / cooldown;
                }
                yield return null;
            }

            enemy.UseSkill(ran);
            Debug.Log($"Enemy가 {ran}번 스킬 사용 - 데미지: {data.skills[ran].damage}");
            guard.TakeDamage(data.skills[ran].damage);

            if (enemy != null && enemy.cooldownBarImage != null)
            {
                enemy.cooldownBarImage.fillAmount = 0f;
            }
        }
    }


    public void DrinkBehavior(List<Drink.DrinkQueue> drinkList)
    {
        string name1 = drinkList[0].drinkData.drinkName;
        string name2 = drinkList[1].drinkData.drinkName;
        string name3 = drinkList[2].drinkData.drinkName;

        int damage = 0;
        string effect = "";

        if (name1 == name2 && name2 == name3)
        {
            guard.UseSkill(1);
            damage = 50;
            effect = "🔥 강한 공격!";
        }
        else if (name1 == name2 || name1 == name3 || name2 == name3)
        {
            guard.UseSkill(2);
            damage = 20;
            effect = "⚡ 중간 공격!";
        }
        else
        {
            guard.UseSkill(3);
            damage = 10;
            effect = "💨 약한 공격!";
        }

        Debug.Log(effect + " " + damage);

        if (enemy != null)
        {
            enemy.TakeDamage(damage);
        }
        else
        {
            Debug.LogWarning("❗ 현재 공격할 Enemy가 없습니다.");
        }
    }

    public void ShowWavePopup(int waveNumber)
    {
        GameObject popup = Instantiate(wavePopupPrefab, wavePopupPoint);

        Text text = popup.GetComponentInChildren<Text>();
        if (text != null)
            text.text = $"Wave {waveNumber}";

        StartCoroutine(FadeAndDestroy(popup, 2f));
    }

    IEnumerator FadeAndDestroy(GameObject popup, float duration)
    {
        CanvasGroup cg = popup.GetComponent<CanvasGroup>();
        if (cg == null)
        {
            Destroy(popup, duration);
            yield break;
        }

        yield return new WaitForSeconds(1f);

        float fadeTime = 1f;
        float t = 0;
        while (t < fadeTime)
        {
            t += Time.deltaTime;
            cg.alpha = Mathf.Lerp(1, 0, t / fadeTime);
            yield return null;
        }

        Destroy(popup);
    }

    void UpdateStageUI(string text)
    {
        if (stageText != null)
        {
            stageText.text = text;
        }
    }

    void UpdateWaveUI(string text)
    {
        if (WaveText != null)
        {
            WaveText.text = text;
        }
    }

}
