using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections;
using System.Collections.Generic;

public class CustomTileRange : MonoBehaviour
{
    [Header("Final Properties")]
    public int towerCost = 0;
    public int towerHP = 0;
    public int towerDamage = 0;
    public float fireRate = 0.0f;

    [Header("Range Properties")]
    public Tilemap tilemap; // The Tilemap where tiles exist
    public Vector3Int centerTilePosition; // The tile position of the object
    public Vector3Int[] customOffsets; // Custom shape offsets
    public Color gizmoColor = Color.green;

    [Header("Debug Properties")]
    public Transform firePoint;
    public GameObject bulletPrefab;
    public string target;
    public float bulletSpeed = 10f;
    public float bulletRange = 10f;
    public LayerMask targetLayer;
    public LineRenderer lineRenderer;
    public float lineDuration = 0.2f;
    private bool canShoot = true; // Cooldown flag

    public float gridSize = 1f;
    public Tilemap groundTilemap;
    public LayerMask objectLayer; // Layer mask for checking placed objects
    private bool placingTower = false; // Toggle for placement state


    private void Update()
    {
        UpdateCenterTile();
        CheckAndShoot();
        if (placingTower && Input.GetMouseButtonDown(0)) // Left Click
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePos.z = 0f;

            float snappedX = Mathf.Round((mousePos.x - gridSize / 2f) / gridSize) * gridSize + gridSize / 2f;
            float snappedY = Mathf.Round((mousePos.y - gridSize / 2f) / gridSize) * gridSize + gridSize / 2f;
            Vector3Int tilePosition = groundTilemap.WorldToCell(new Vector3(snappedX, snappedY, 0f));

            if (groundTilemap.HasTile(tilePosition))
            {
                Collider2D hit = Physics2D.OverlapPoint(new Vector2(snappedX, snappedY), objectLayer);
                if (hit == null)
                {
                    transform.position = new Vector3(snappedX, snappedY, 0f);
                }
                else
                {
                    Debug.Log("Cannot place here! Another object is already present.");
                }
            }
        }
    }

    public void ChangePlacingState()
    {
        placingTower = !placingTower;
    }

    private void UpdateCenterTile()
    {
        centerTilePosition = tilemap.WorldToCell(transform.position);
    }

    private void CheckAndShoot()
    {
        if (!canShoot) return; // Prevent shooting if cooldown is active

        List<Vector3Int> tilesInRange = GetTilesInRange();

        foreach (Vector3Int tilePos in tilesInRange)
        {
            Vector3 worldPos = tilemap.GetCellCenterWorld(tilePos);
            Vector2 tileSize = tilemap.cellSize;

            Collider2D collider = Physics2D.OverlapBox(worldPos, tileSize, 0f);

            if (collider != null && collider.CompareTag(target))
            {
                StartCoroutine(DrawRay((Vector2)firePoint.position, (Vector2)collider.transform.position));
                if (!canShoot) return;
                Shoot(collider.gameObject);
                StartCoroutine(ShootingCooldown()); // Start cooldown timer
                break;
            }
        }
    }

    private IEnumerator ShootingCooldown()
    {
        canShoot = false; // Disable shooting
        yield return new WaitForSeconds(60f / fireRate); // Wait for 0.5 seconds
        canShoot = true; // Enable shooting again
    }


    public List<Vector3Int> GetTilesInRange()
    {
        List<Vector3Int> tilesInRange = new List<Vector3Int>();

        foreach (Vector3Int offset in customOffsets)
        {
            Vector3Int tilePos = centerTilePosition + offset;
            if (tilemap.HasTile(tilePos)) // Only add valid tiles
            {
                tilesInRange.Add(tilePos);
            }
        }

        return tilesInRange;
    }

    private void OnDrawGizmos()
    {
        if (tilemap == null) return;

        Gizmos.color = gizmoColor;
        List<Vector3Int> tiles = GetTilesInRange();

        foreach (Vector3Int tile in tiles)
        {
            Vector3 tileWorldPos = tilemap.GetCellCenterWorld(tile);
            Gizmos.DrawWireCube(tileWorldPos, tilemap.cellSize);
        }
    }

    private void Shoot(GameObject target)
{
    if (target == null) return; // Prevent errors if target is null

    Vector2 targetPosition = target.transform.position; // Get detected player's position
    Vector2 firePoint2D = firePoint.position;
    Vector2 direction = (targetPosition - firePoint2D).normalized; // Aim at the detected player

    RaycastHit2D hit = Physics2D.Raycast(firePoint2D, direction, bulletRange, targetLayer);
    Vector2 endPoint = targetPosition; // Set the endPoint to the player's position

    if (hit.collider != null) // If we hit something before reaching the player
    {
        Debug.Log("hit");
        endPoint = hit.point; // Update endpoint to hit location
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
