using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;
using UnityEditor.Timeline;
using System.Collections;
using TMPro;

public class GameManager : MonoBehaviour
{
    public Tilemap groundTilemap;
    public int currencyCount;

    private Dictionary<Vector3Int, GameObject> placedTowers = new Dictionary<Vector3Int, GameObject>();
    private TowerManager towerManager;
    private TextMeshProUGUI currencyCountText;
    private WaveSpawner waveSpawner;

    private void Start()
    {
        currencyCountText = GameObject.FindGameObjectWithTag("Cost").GetComponent<TextMeshProUGUI>();
        currencyCountText.SetText("" + currencyCount);

        towerManager = FindAnyObjectByType<TowerManager>();
        waveSpawner = FindAnyObjectByType<WaveSpawner>();
    }

    private void Update()
    {
        if (towerManager == null)
        {
            towerManager = FindAnyObjectByType<TowerManager>();
        }
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
        if (obj.CompareTag("Tower") && obj.GetComponent<Tower>() != null)
        {
            Tower tower = obj.GetComponent<Tower>();
            Vector3Int tilePosition = groundTilemap.WorldToCell(obj.transform.position);
            RemoveTower(tilePosition);
            towerManager.TowerDied(tower);
        }
        else if (obj.CompareTag("Enemy") && obj.GetComponent<Pathfinding>() != null)
        {
            waveSpawner.EnemyDeath();
        }
        Destroy(obj);
    }

    public void AddToCurrency(int add)
    {
        currencyCount += add;
        currencyCountText.SetText("" + currencyCount);
    }
}
