using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    public int hp;
    private int currentHealth;

    private Renderer objectRenderer;
    private GameManager gm;

    private void Awake()
    {
        gm = FindFirstObjectByType<GameManager>();
    }

    // Start is called before the first frame update
    void Start()
    {
        objectRenderer = GetComponent<Renderer>();
        currentHealth = hp;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;

        //DisplayDamage(damage);

        if (currentHealth <= 0)
        {
            gm.Kill(this.gameObject);
            return;
        }
        StartCoroutine(PaintRed());
    }


    private IEnumerator PaintRed()
    {
        objectRenderer.material.color = Color.red;
        yield return new WaitForSeconds(0.1f); // Wait for 0.1 seconds
        objectRenderer.material.color = Color.white;
    }

    private void DisplayDamage(int damage)
    {
        //GameObject digit = Instantiate(numberImages[damage % 10], transform.position, Quaternion.identity);   
        //digit.
    }
}
