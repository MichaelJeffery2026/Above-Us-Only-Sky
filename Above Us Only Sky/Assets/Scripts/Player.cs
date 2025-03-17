using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Player : MonoBehaviour
{
    [Tooltip("The number of hitpoints the player has.")]
    public int playerHP = 0;

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

    private LayerMask targetLayer;
    private float lineDuration = 0.1f;
    private Rigidbody2D _rb;
    private LineRenderer lineRenderer;

    private int currentHealth;
    private float tileSize = 1f;
    public Tilemap groundTilemap;
    private bool isMoving = false;
    public float moveDuration = 0.2f;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        lineRenderer = GetComponent<LineRenderer>();
        targetLayer = LayerMask.GetMask("Enemy");
        if (groundTilemap != null)
        {
            tileSize = groundTilemap.cellSize.x;
            AlignToGrid();
        }
    }

    private void Start()
    {
        currentHealth = playerHP;
    }

    private void Update()
    {
        if (!isMoving)
        {
            float moveX = Input.GetAxisRaw("Horizontal");
            float moveY = Input.GetAxisRaw("Vertical");

            if (moveX != 0 || moveY != 0)
            {
                Vector2 moveDirection = new Vector2(moveX, moveY).normalized;
                Vector3 targetPosition = transform.position + (Vector3)(moveDirection * tileSize);
                if (IsPositionValid(targetPosition))
                {
                    StartCoroutine(Move(moveDirection));
                }
            }
        }
    }

    private IEnumerator Move(Vector2 direction)
    {
        isMoving = true;
        Vector3 startPosition = transform.position;
        Vector3 targetPosition = groundTilemap.GetCellCenterWorld(groundTilemap.WorldToCell(startPosition + (Vector3)(direction * tileSize)));
        float elapsedTime = 0f;

        while (elapsedTime < moveDuration)
        {
            transform.position = Vector3.Lerp(startPosition, targetPosition, elapsedTime / moveDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = targetPosition;
        isMoving = false;
    }

    private bool IsPositionValid(Vector3 position)
    {
        Vector3Int cellPosition = groundTilemap.WorldToCell(position);
        if (!groundTilemap.HasTile(cellPosition))
        {
            return false;
        }

        Collider2D[] colliders = Physics2D.OverlapCircleAll(position, tileSize * 0.4f);
        foreach (Collider2D collider in colliders)
        {
            if (collider.CompareTag("Tower") || collider.CompareTag("Enemy"))
            {
                return false;
            }
        }
        return true;
    }

    private void AlignToGrid()
    {
        Vector3Int cellPosition = groundTilemap.WorldToCell(transform.position);
        transform.position = groundTilemap.GetCellCenterWorld(cellPosition);
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