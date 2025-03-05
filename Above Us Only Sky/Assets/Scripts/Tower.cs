using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;

public class Tower : MonoBehaviour
{
    public int towerCost = 0;
    public string towerName = "";
    public int cooldown = 5;
    public bool IsUnique = false;

    private Tilemap tilemap; // The Tilemap where tiles exist
    [Tooltip("Assign bullet object.")]

    private float gridSize = 1f;
    private LayerMask objectLayer; // Layer mask for checking placed objects
    private bool placingTower = false; // Toggle for placement state

    private void Awake()
    {
        tilemap = GameObject.Find("Ground").GetComponent<Tilemap>();
    }

    private void Start()
    {

    }

    private void Update()
    {
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
}
