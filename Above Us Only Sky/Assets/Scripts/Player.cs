using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Tooltip("The number of hitpoints the player has.")]
    public int playerHP = 0;

    [Tooltip("The speed at which the player moves.")]
    public float moveSpeed = 0f;

    [Tooltip("The radius of the player's shooting range in tiles.")]
    public float range = 0f;

    [Tooltip("The amount of damage each projectile deals.")]
    public int playerDamage = 0;

    [Tooltip("The fire rate of the player in rounds per minute.")]
    public float fireRate = 0f;

    [Tooltip("The amount of healing each instance provides.")]
    public int towerHealingAmount = 0;

    [Tooltip("The radius of the player's healing range in tiles.")]
    public int towerHealingRange = 0;


    //The speed at which the player's bullets travel in tiles per second
    private float bulletSpeed = 20f;
    //The layer that the player's targets are on. Should be 'Enemies'
    private LayerMask targetLayer;
    // The time that the shooting debug line appears on the screen
    private float lineDuration = 0.1f;
    private Rigidbody2D _rb;
    public GameObject bulletPrefab;
    private Transform firePoint;
    private LineRenderer lineRenderer;
    // For use in calculating player movement. MUST be set to Vector2.zero in this line.
    private Vector2 _moveDir = Vector2.zero; 
    // For use in fire rate cooldown. MUST be set to true in this line.
    private bool canShoot = true;
    private int currentHealth;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        bulletPrefab = GameObject.Find("Player Bullet");
        firePoint = transform.Find("Fire Point").GetComponent<Transform>();
        lineRenderer = GetComponent<LineRenderer>();
        targetLayer = LayerMask.GetMask("Enemy");
    }

    private void Start()
    {
        currentHealth = playerHP;
    }

    private void Update()
    {
        _moveDir.x = Input.GetAxisRaw("Horizontal");
        _moveDir.y = Input.GetAxisRaw("Vertical");
        if (canShoot && Input.GetMouseButtonDown(0))
        {
            Shoot();
            StartCoroutine(ShootingCooldown());
        }
    }

    private void FixedUpdate()
    {
        _rb.velocity = _moveDir.normalized * moveSpeed * Time.fixedDeltaTime;
    }

    private void Shoot()
    {
        Vector2 mousePosition2D = (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 firePoint2D = firePoint.position;
        Vector2 direction = (mousePosition2D - firePoint2D).normalized;

        RaycastHit2D hit = Physics2D.Raycast(firePoint2D, direction, range, targetLayer);
        Vector2 endPoint = mousePosition2D;

        if (hit.collider != null)
        {
            Debug.Log("hit");
            hit.collider.gameObject.GetComponent<Enemy>().TakeDamage(playerDamage);
            endPoint = hit.point;
        }

        StartCoroutine(DrawRay(firePoint2D, endPoint));

        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
        Renderer bulletRenderer = bullet.GetComponent<Renderer>();
        bulletRenderer.transform.Rotate(Vector3.forward * Mathf.Atan2(direction.y, direction.x) * 180 / Mathf.PI); // Rotates the bullet sprite in the direction of shooting
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        rb.velocity = direction * bulletSpeed;

        float bulletTime = range / bulletSpeed;
        Destroy(bullet, bulletTime);
    }

    private IEnumerator ShootingCooldown()
    {
        canShoot = false;
        yield return new WaitForSeconds(60f / fireRate);
        canShoot = true;
    }

    IEnumerator DrawRay(Vector3 start, Vector3 end)
    {
        lineRenderer.SetPosition(0, start);
        lineRenderer.SetPosition(1, end);

        lineRenderer.enabled = true;
        yield return new WaitForSeconds(lineDuration);
        lineRenderer.enabled = false;
    }
    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        Debug.Log(gameObject.name + " : " + damage + " damage : " + currentHealth + " health");

        if (currentHealth <= 0)
        {
            Destroy(gameObject);
        }
    }
}
