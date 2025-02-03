using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Controller : MonoBehaviour
{
    public float _moveSpeed = 50f;
    public Rigidbody2D _rb;
    private Vector2 _moveDir = Vector2.zero;
    public GameObject bulletPrefab; // Prefab for the bullet
    public Transform firePoint; // Where the bullets spawn
    public float bulletSpeed = 10f; // Speed of the bullet
    public float bulletRange = 10f;
    public int playerBulletDamage = 25;
    public LineRenderer lineRenderer; // Assign a LineRenderer in the Inspector
    public float lineDuration = 0.1f; // How long the line stays visible
    public LayerMask targetLayer;

    private void Update()
    {
        GatherInput();
        if (Input.GetMouseButtonDown(0))
        {
            Shoot();
        }
    }

    private void FixedUpdate()
    {
        MovementUpdate();
    }

    private void GatherInput()
    {
        _moveDir.x = Input.GetAxisRaw("Horizontal");
        _moveDir.y = Input.GetAxisRaw("Vertical");
    }

    private void MovementUpdate()
    {
        _rb.velocity = _moveDir.normalized * _moveSpeed * Time.fixedDeltaTime;
    }

    private void Shoot()
    {
        Vector2 mousePosition2D = (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 firePoint2D = firePoint.position;
        Vector2 direction = (mousePosition2D - firePoint2D).normalized;

        RaycastHit2D hit = Physics2D.Raycast(firePoint2D, direction, bulletRange, targetLayer);
        Vector2 endPoint = mousePosition2D;

        if (hit.collider != null) // If we hit something
        {
            Debug.Log("hit");
            endPoint = hit.point; // Set the end point to the hit location
        }

        StartCoroutine(DrawRay(firePoint2D, endPoint));

        // Instantiate Bullet
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        rb.velocity = direction * bulletSpeed;

        float bulletTime = bulletRange / bulletSpeed;
        Destroy(bullet, bulletTime);
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
