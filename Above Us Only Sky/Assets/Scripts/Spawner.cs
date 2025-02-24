using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    // Start is called before the first frame update
    private float timer = 0f;
    public float period = 4f;
    public GameObject enemy;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;

        // Check if the timer has passed the interval
        if (timer >= period)
        {
            // Perform the operation
            GameObject _enemy = Instantiate(enemy, transform.position, transform.rotation);

            // Reset the timer
            timer = 0f;
        }
    }
}
