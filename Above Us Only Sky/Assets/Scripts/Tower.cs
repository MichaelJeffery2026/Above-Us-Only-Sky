using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;

public class Tower : MonoBehaviour
{
    public int towerCost = 0;
    public int towerHP = 0;
    public int towerDamage = 0;
    public float fireRate = 0.0f;
    public Vector3Int[] customOffsets; // Custom shape offsets

    private Tilemap tilemap; // The Tilemap where tiles exist
    public Vector3Int centerTilePosition; // The tile position of the object
    private Color gizmoColor = Color.red;
    private Transform firePoint;
    [Tooltip("Assign bullet object.")]
    public GameObject bulletPrefab;
    private string target = "Enemy";
    private float bulletSpeed = 20f;
    private LayerMask targetLayer;
    private LineRenderer lineRenderer;
    private float lineDuration = 0.2f;
    private bool canShoot = true; // Cooldown flag

    private bool isShooting = false;
    private Animator mAnimator;

    private float gridSize = 1f;
    private Tilemap groundTilemap;
    private LayerMask objectLayer; // Layer mask for checking placed objects
    private bool placingTower = false; // Toggle for placement state
    private int currentHealth;
    private GameManager gameManager;

    private void Awake()
    {
        tilemap = GameObject.Find("Ground").GetComponent<Tilemap>();
        firePoint = transform.Find("Fire Point").GetComponent<Transform>();
        lineRenderer = GetComponent<LineRenderer>();
        //bulletPrefab = GameObject.Find("Tower Bullet");
        targetLayer = LayerMask.GetMask("Enemy");
        objectLayer = LayerMask.GetMask("Tower");
        gameManager = FindObjectOfType<GameManager>();
    }

    private void Start()
    {
        currentHealth = towerHP;
        mAnimator = GetComponent<Animator>();
    }

    private void Update()
    {
        UpdateCenterTile();
        CheckAndShoot();
        PlaceTowerCheck();
    }

    private void PlaceTowerCheck()
    {
        if (placingTower && Input.GetMouseButtonDown(0)) // Left Click
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePos.z = 0f;

            float snappedX = Mathf.Round((mousePos.x - gridSize / 2f) / gridSize) * gridSize + gridSize / 2f;
            float snappedY = Mathf.Round((mousePos.y - gridSize / 2f) / gridSize) * gridSize + gridSize / 2f;
            Vector3Int tilePosition = tilemap.WorldToCell(new Vector3(snappedX, snappedY, 0f));

            if (tilemap.HasTile(tilePosition))
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
        List<Vector3Int> tilesInRange = GetTilesInRange();

        foreach (Vector3Int tilePos in tilesInRange)
        {
            Vector3 worldPos = tilemap.GetCellCenterWorld(tilePos);
            Vector2 tileSize = tilemap.cellSize;

            Collider2D collider = Physics2D.OverlapBox(worldPos, tileSize, 0f);

            if (collider != null && collider.CompareTag(target))
            {

                StartCoroutine(DrawRay((Vector2)firePoint.position, (Vector2)collider.transform.position));

                if (!isShooting) //Activate shooting animation
                {
                    isShooting = true;
                    mAnimator.SetTrigger("Firing");
                }

                if (!canShoot) return; // Prevent shooting if cooldown is active

                Shoot(collider.gameObject);
                StartCoroutine(ShootingCooldown()); // Start cooldown timer
                return;
            }
        }
        if (isShooting) //Deactive shooting animation if no target found
        {
            isShooting = false;
            mAnimator.SetTrigger("Stop Firing");
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

    RaycastHit2D hit = Physics2D.Raycast(firePoint2D, direction, 100f, targetLayer);
    Vector2 endPoint = targetPosition; // Set the endPoint to the player's position

    if (hit.collider != null) // If we hit something before reaching the player
    {
        endPoint = hit.point; // Update endpoint to hit location
    }

    StartCoroutine(DrawRay(firePoint2D, endPoint));

    // Instantiate Bullet
    GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
    bullet.SetActive(true);
    Renderer bulletRenderer = bullet.GetComponent<Renderer>();
        bulletRenderer.transform.Rotate(Vector3.forward * Mathf.Atan2(direction.y, direction.x) * 180 / Mathf.PI); // Rotates the bullet sprite in the direction of shooting
    Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
    rb.velocity = direction * bulletSpeed;

    StartCoroutine(DestroyBulletAfterTravel(bullet, firePoint2D, endPoint, target));
    }

    private IEnumerator DestroyBulletAfterTravel(GameObject bullet, Vector2 start, Vector2 end, GameObject target)
    {
        // Continuously check the distance between the bullet's position and the target position
        while (Vector2.Distance(bullet.transform.position, start) < Vector2.Distance(start, end))
        {
            yield return null; // Wait for the next frame
        }

        // Destroy the bullet once it has traveled the full distance to the target
        Destroy(bullet);
        target.GetComponent<Enemy>().TakeDamage(towerDamage);
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
            gameManager.RemoveTower(centerTilePosition);
        }
    }
}
