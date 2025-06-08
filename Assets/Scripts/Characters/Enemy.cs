using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class Enemy : MonoBehaviour
{
    public int maxHealth;
    public int currentHealth;
    public GameObject prefab; 
    public GameObject floatingDamageTextPrefab; 
    public List<EnemySkill> skills;
    public GameObject hitEffectPrefab;
    public Transform hitPoint;

    public Text curHpText;
    public Image healthBarImage;
    public Image cooldownBarImage;
    public Transform healthBarCanvas;

    Animator animator;

    public event Action OnEnemyDefeated;

    void Start()
    {
        
    }

    public void Init(EnemyData data)
    {
        animator = GetComponent<Animator>();

        this.maxHealth = data.maxHealth;
        this.prefab = data.prefab;
        this.skills = data.skills;

        this.currentHealth = maxHealth;
        UpdateHealthUI();
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Max(currentHealth, 0);
        ShowFloatingDamage(damage);
        UpdateHealthUI();

        Instantiate(hitEffectPrefab, hitPoint);

        Debug.Log($"🛡️ Enemy HP: {currentHealth}");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void Die()
    {
        Debug.Log("💀 Enemy 사망!");
        animator.SetTrigger("Die");
        StartCoroutine(WaitAndDestroy());
    }

    private IEnumerator WaitAndDestroy()
    {
        yield return new WaitForSeconds(1.5f);
        Destroy(gameObject);
        OnEnemyDefeated?.Invoke();
        yield return new WaitForSeconds(1.5f);
    }

    private void UpdateHealthUI()
    {
        if (healthBarImage != null)
        {
            curHpText.text = currentHealth.ToString();
            float ratio = (float)currentHealth / maxHealth;
            healthBarImage.fillAmount = ratio;
        }
    }

    public void UseSkill(int skillNumber)
    {
        switch (skillNumber)
        {
            case 0:
                animator.SetTrigger("UseSkill1");
                break;
            case 1:
                animator.SetTrigger("UseSkill2");
                break;
            case 2:
                animator.SetTrigger("UseSkill3");
                break;
            case 3:
                animator.SetTrigger("UseSkill4");
                break;
        }
    }

    private void ShowFloatingDamage(int damage)
    {
        if (floatingDamageTextPrefab != null && healthBarCanvas != null)
        {
            GameObject textGO = Instantiate(floatingDamageTextPrefab, healthBarCanvas);
            textGO.GetComponent<Text>().text = damage.ToString();

            StartCoroutine(FadeAndMove(textGO));
        }
    }

    private IEnumerator FadeAndMove(GameObject textGO)
    {
        Text text = textGO.GetComponent<Text>();
        Color originalColor = text.color;
        Vector3 startPos = textGO.transform.localPosition;
        Vector3 endPos = startPos + new Vector3(0, 30, 0);

        float duration = 1.0f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            text.color = new Color(originalColor.r, originalColor.g, originalColor.b, 1 - t);
            textGO.transform.localPosition = Vector3.Lerp(startPos, endPos, t);

            yield return null;
        }

        Destroy(textGO);
    }

}
