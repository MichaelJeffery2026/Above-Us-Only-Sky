using System.Collections;
using UnityEngine;

public class Health : MonoBehaviour
{
    public int hp;
    public int currencyReward;
    private int currentHealth;

    private SpriteRenderer objectRenderer;
    private GameManager gm;

    public GameObject healthBarInstance;
    private SpriteRenderer healthBarFillRenderer;
    private float originalWidth;

    private float hpTimeLeft = 0f;

    private void Awake()
    {
        gm = FindFirstObjectByType<GameManager>();
    }

    void Start()
    {
        objectRenderer = GetComponent<SpriteRenderer>();
        currentHealth = hp;

        if (healthBarInstance) {
            healthBarFillRenderer = healthBarInstance.GetComponent<SpriteRenderer>();
            healthBarFillRenderer.color = new Color(1f, 0f, 0f, 0f);
            originalWidth = healthBarFillRenderer.size.x;
        }
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;

        if (healthBarInstance)
        {
            float ratio = (float)currentHealth / hp;  // Assuming the X axis represents the health bar width
            healthBarFillRenderer.size = new Vector2(originalWidth * ratio, healthBarFillRenderer.size.y);

            // Make the health bar visible and start fading it
            healthBarFillRenderer.color = new Color(1f, 0f, 0f, 1f);  // Show health bar
            StopCoroutine(FadeHealthBar());
            StartCoroutine(FadeHealthBar());
        }

        if (currentHealth <= 0)
        {
            gm.AddToCurrency(currencyReward);
            gm.Kill(this.gameObject);
            return;
        }

        if (damage < 0)
        {
            StartCoroutine(PaintGreen());
        }
        else
        {
            StartCoroutine(PaintRed());
        }
    }

    private IEnumerator FadeHealthBar()
    {
        yield return new WaitForSeconds(2f); // Keep it visible for 2 seconds

        float fadeDuration = 1f;
        float elapsedTime = 0f;
        Color currentColor = healthBarFillRenderer.color;

        //while (elapsedTime < fadeDuration)
        //{
        //    currentColor.a = Mathf.Lerp(1f, 0f, elapsedTime / fadeDuration);
        //    healthBarFillRenderer.color = currentColor;
        //    elapsedTime += Time.deltaTime;
        //    yield return null;
        //}

        currentColor.a = 0f;
        healthBarFillRenderer.color = currentColor;
    }

    private IEnumerator PaintRed()
    {
        objectRenderer.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        objectRenderer.color = Color.white;
    }

    private IEnumerator PaintGreen()
    {
        objectRenderer.color = Color.green;
        yield return new WaitForSeconds(0.1f);
        objectRenderer.color = Color.white;
    }

    public bool IsFull()
    {
        return currentHealth >= hp;
    }
}
