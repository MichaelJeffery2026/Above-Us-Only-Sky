using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour
{
    public int enemyHP = 100;
    private int currentHealth;
    private Renderer objectRenderer;

    private void Start()
    {
        objectRenderer = GetComponent<Renderer>();
    }

    private void Awake()
    {
        currentHealth = enemyHP;
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            Destroy(gameObject);
            return;
        }
        StartCoroutine(paintRed());
    }

    private IEnumerator paintRed()
    {
        objectRenderer.material.color = Color.red;
        yield return new WaitForSeconds(0.1f); // Wait for 0.1 seconds
        objectRenderer.material.color = Color.white;
    }
}