using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Controller : MonoBehaviour
{
    [Header("Final")]
    public int hp = 100;
    public float moveSpeed = 50f;
    public float range = 10f;
    public int damage = 25;
    public float fireRate = 120f;
    public int towerHealingAmount = 0;
    public int towerHealingRange = 0;

    [Header("Testing")]
    public float bulletSpeed = 10f;
    public LayerMask targetLayer;
    public float lineDuration = 0.1f;
    public Rigidbody2D _rb;
    public GameObject bulletPrefab;
    public Transform firePoint;
    public LineRenderer lineRenderer;

    private Vector2 _moveDir = Vector2.zero;  
    private bool canShoot = true;  

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
            endPoint = hit.point;
        }

        StartCoroutine(DrawRay(firePoint2D, endPoint));

        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
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
}
