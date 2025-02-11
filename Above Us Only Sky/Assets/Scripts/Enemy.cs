using UnityEngine;

public class Enemy : MonoBehaviour
{
    public int enemyHP = 100;

    private int currentHealth;

    private void Awake()
    {
        currentHealth = enemyHP;
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0) Destroy(gameObject);
    }
}