using UnityEngine;
using UnityEngine.Tilemaps;

public class DragAndSnap : MonoBehaviour
{
    public float gridSize = 1f;
    public Tilemap groundTilemap; // Reference to the "Ground" tilemap
    public LayerMask objectLayer; // Layer mask for checking placed objects
    private bool canPlace = false; // Toggle for placement state

    void Update()
    {
        // Toggle placement mode with the "1" key
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            canPlace = !canPlace;
            Debug.Log("Placement Mode: " + (canPlace ? "Enabled" : "Disabled"));
        }

        if (canPlace && Input.GetMouseButtonDown(0)) // Left Click
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePos.z = 0f;

            // **Correct snapping calculation (ensures the center of the tile)**
            float snappedX = Mathf.Round((mousePos.x - gridSize / 2f) / gridSize) * gridSize + gridSize / 2f;
            float snappedY = Mathf.Round((mousePos.y - gridSize / 2f) / gridSize) * gridSize + gridSize / 2f;
            Vector3Int tilePosition = groundTilemap.WorldToCell(new Vector3(snappedX, snappedY, 0f));

            // **Check if there is a tile in the Ground tilemap at this position**
            if (groundTilemap.HasTile(tilePosition))
            {
                // **Check if there's already an object at this snapped position**
                Collider2D hit = Physics2D.OverlapPoint(new Vector2(snappedX, snappedY), objectLayer);
                if (hit == null) // No object is blocking the tile
                {
                    transform.position = new Vector3(snappedX, snappedY, 0f);
                    Debug.Log("Placed at: " + transform.position);
                }
                else
                {
                    Debug.Log("Cannot place here! Another object is already present.");
                }
            }
            else
            {
                Debug.Log("Cannot place here! No Ground tile.");
            }
        }
    }
}
