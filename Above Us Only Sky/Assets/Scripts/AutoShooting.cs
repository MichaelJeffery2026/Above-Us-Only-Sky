using System.Collections;
using UnityEngine.Tilemaps;
using System.Collections.Generic;
using UnityEngine;

public class AutoShooting : MonoBehaviour
{
    private Tilemap tilemap;
    public int damage = 0;
    public float fireRate = 0.0f;
    public Vector3Int centerTilePosition;
    public Vector3[] customOffsets; // Custom shape offsets

    private bool isShooting = false;
    private Animator mAnimator;
    private Color gizmoColor = Color.red;
    private Transform firePoint;
    [Tooltip("Assign bullet object.")]
    public GameObject bulletPrefab;
    public string target;
    private float bulletSpeed = 20f;
    private LayerMask targetLayer;
    public string targetLayerName;
    private LineRenderer lineRenderer;
    private float lineDuration = 0.2f;
    private bool canShoot = true; // Cooldown flag

    public Vector3 targetLocation;

    public LayerMask layerMask;

    public float shootStartDelay;

    private MultiChannelAudio multiAudio;
    public bool hasAudio;


    private void Awake()
    {
        tilemap = GameObject.Find("Ground").GetComponent<Tilemap>();
        firePoint = transform.Find("Fire Point").GetComponent<Transform>();
        lineRenderer = GetComponent<LineRenderer>();
        targetLayer = LayerMask.GetMask(targetLayerName);
    }
    // Start is called before the first frame update
    void Start()
    {
        mAnimator = GetComponent<Animator>();
        if (hasAudio)
        {
            multiAudio = GetComponent<MultiChannelAudio>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        UpdateCenterTile();
        CheckAndShoot();
    }

    private void CheckAndShoot()
    {
        List<Vector3> tilesInRange = GetTilesInRange();

        foreach (Vector3 tilePos in tilesInRange)
        {
            //Vector3 worldPos = tilemap.GetCellCenterWorld(tilePos);
            Vector2 tileSize = tilemap.cellSize;

            Collider2D collider = Physics2D.OverlapBox(tilePos, tileSize, 0f, layerMask);

            if (collider != null && collider.CompareTag(target))
            {

                targetLocation = collider.transform.position;

                StartCoroutine(DrawRay((Vector2)firePoint.position, (Vector2)collider.transform.position));

                if (!isShooting) //Activate shooting animation
                {
                    isShooting = true;
                    mAnimator.SetTrigger("Firing");
                }

                if (!canShoot) return; // Prevent shooting if cooldown is active

                StartCoroutine(ShootBeginDelay(collider.gameObject));
                //Shoot(collider.gameObject);
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


    public List<Vector3> GetTilesInRange()
    {
        List<Vector3> tilesInRange = new List<Vector3>();

        foreach (Vector3 offset in customOffsets)
        {
            Vector3 tilePos = transform.position + offset;
            tilesInRange.Add(tilePos);
        }

        return tilesInRange;
    }

    private IEnumerator ShootBeginDelay(GameObject target)
    {
        yield return new WaitForSeconds(shootStartDelay);
        Shoot(target);
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

        if (hasAudio)
        {
            multiAudio.PlayRandomSound();
        }
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
        while (target != null && Vector2.Distance(bullet.transform.position, start) < Vector2.Distance(start, end))
        {
            yield return null; // Wait for the next frame
        }

        // Destroy the bullet once it has traveled the full distance to the target
        Destroy(bullet);
        if (target != null)
        {
            target.GetComponent<Health>().TakeDamage(damage);
        }
    }

    IEnumerator DrawRay(Vector3 start, Vector3 end)
    {
        lineRenderer.SetPosition(0, start);
        lineRenderer.SetPosition(1, end);

        lineRenderer.enabled = true;
        yield return new WaitForSeconds(lineDuration);
        lineRenderer.enabled = false;
    }

    private void UpdateCenterTile()
    {
        centerTilePosition = tilemap.WorldToCell(transform.position);
    }

    private void OnDrawGizmos()
    {
        if (tilemap == null) return;

        Gizmos.color = gizmoColor;
        List<Vector3> tiles = GetTilesInRange();

        foreach (Vector3 tile in tiles)
        {
            //Vector3 tileWorldPos = tilemap.GetCellCenterWorld(tile);
            Gizmos.DrawWireCube(tile, tilemap.cellSize);
        }
    }

    public bool hasTargetLocation()
    {
        return targetLocation != null;
    }
}
