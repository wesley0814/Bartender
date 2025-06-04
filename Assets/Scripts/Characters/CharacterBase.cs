using UnityEngine;
using UnityEngine.UI;

public class CharacterBase : MonoBehaviour
{
    public int maxHealth = 100;
    public int currentHealth;

    public Image healthBarImage;  // Fill 이미지
    public Transform healthBarCanvas; // 전체 체력바 UI (부모)

    protected virtual void Awake()
    {
        currentHealth = maxHealth;
        UpdateHealthUI();
    }

    public virtual void TakeDamage(int damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Max(currentHealth, 0);
        UpdateHealthUI();

        Debug.Log($"{gameObject.name} 체력: {currentHealth}");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public virtual void Die()
    {
        Debug.Log($"{gameObject.name} 사망!");
        // 이펙트나 애니메이션 등 추가 가능
        Destroy(gameObject);
    }

    public void UpdateHealthUI()
    {
        if (healthBarImage != null)
        {
            float ratio = (float)currentHealth / maxHealth;
            healthBarImage.fillAmount = ratio;
        }
    }
}
