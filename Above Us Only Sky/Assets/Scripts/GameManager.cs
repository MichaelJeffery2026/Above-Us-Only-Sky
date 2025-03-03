using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public Tilemap groundTilemap;
    public float GoldCooldown = 3;

    private Dictionary<Vector3Int, GameObject> placedTowers = new Dictionary<Vector3Int, GameObject>();

    private void Update()
    {
        
    }

    public bool TryPlaceTower(Vector3Int tilePosition, GameObject towerPrefab)
    {
        if (groundTilemap.HasTile(tilePosition) && !placedTowers.ContainsKey(tilePosition))
        {
            // Place the tower on the tile at the corresponding world position
            Vector3 worldPosition = groundTilemap.CellToWorld(tilePosition) + groundTilemap.tileAnchor;
            GameObject newTower = Instantiate(towerPrefab, worldPosition, Quaternion.identity);

            placedTowers[tilePosition] = newTower;  
            return true;  
        }


        return false; 
    }

    public void RemoveTower(Vector3Int tilePosition)
    {
        if (placedTowers.ContainsKey(tilePosition))
        {
            Destroy(placedTowers[tilePosition]);
            placedTowers.Remove(tilePosition);
        }
    }
}
