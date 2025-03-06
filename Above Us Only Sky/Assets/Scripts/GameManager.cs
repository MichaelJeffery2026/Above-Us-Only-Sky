using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;
using UnityEditor.Timeline;

public class GameManager : MonoBehaviour
{
    public Tilemap groundTilemap;
    public float GoldCooldown = 3;

    private Dictionary<Vector3Int, GameObject> placedTowers = new Dictionary<Vector3Int, GameObject>();
    private TowerManager towerManager;

    private void Update()
    {
        towerManager = FindAnyObjectByType<TowerManager>();
    }

    public bool TryPlaceTower(Vector3Int tilePosition, GameObject towerPrefab)
    {
        if (groundTilemap.HasTile(tilePosition) && !placedTowers.ContainsKey(tilePosition))
        {
            // Place the tower on the tile at the corresponding world position
            Vector3 worldPosition = groundTilemap.CellToWorld(tilePosition) + groundTilemap.tileAnchor;
            GameObject newTower = Instantiate(towerPrefab, worldPosition, Quaternion.identity);

            placedTowers[tilePosition] = newTower;  

            Tower tower = newTower.GetComponent<Tower>();

            towerManager.placedTower(tower);
            return true;  
        }


        return false; 
    }

    public void RemoveTower(Vector3Int tilePosition)
    {
        if (placedTowers.ContainsKey(tilePosition))
        {
            placedTowers.Remove(tilePosition);
        }
    }

    public void Kill(GameObject obj)
    {
        if (obj.CompareTag("Tower"))
        {
            Tower tower = obj.GetComponent<Tower>();
            Vector3Int tilePosition = groundTilemap.WorldToCell(obj.transform.position);
            RemoveTower(tilePosition);
            towerManager.TowerDied(tower);
        }
        Destroy(obj);
    }
}
