using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour
{
    public int enemyHP; //=100
    public int hitRate;
    public int damage;
    private int currentHealth;
    private Renderer objectRenderer;
    private GameObject[] numberImages;
    private GameObject pathTarget;

    private bool canHit = true;

    private void Start()
    {
        
        objectRenderer = GetComponent<Renderer>();
        /*
        numberImages = new GameObject[10];
        for (int i = 0; i < 10; i++)
        {
            numberImages[i] = GameObject.Find("Number_" + i); 
        }
        */
    }

    private void Awake()
    {
        currentHealth = enemyHP;
    }

    private void Update()
    {
        if (this.GetComponent<Pathfinding>().hasTarget())
        {
            pathTarget = this.GetComponent<Pathfinding>().getTarget();
            if (Vector3.Distance(pathTarget.transform.position, transform.position) <= 1.0f)
            {
                if (!canHit) { return; }
                Hit();
            }
        }
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;

        DisplayDamage(damage);

        if (currentHealth <= 0)
        {
            Destroy(gameObject);
            return;
        }
        StartCoroutine(PaintRed());
    }

    private void DisplayDamage(int damage)
    {
        //GameObject digit = Instantiate(numberImages[damage % 10], transform.position, Quaternion.identity);
        //digit.
    }

    private void Hit()
    {
        pathTarget.GetComponent<Tower>().TakeDamage(damage);
        StartCoroutine(HittingCooldown());

    }

    private IEnumerator HittingCooldown()
    {
        canHit = false; // Disable shooting
        yield return new WaitForSeconds(60f / hitRate); // Wait for 0.5 seconds
        canHit = true; // Enable shooting again
    }


    private IEnumerator PaintRed()
    {
        objectRenderer.material.color = Color.red;
        yield return new WaitForSeconds(0.1f); // Wait for 0.1 seconds
        objectRenderer.material.color = Color.white;
    }
}