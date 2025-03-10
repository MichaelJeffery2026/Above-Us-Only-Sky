using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;
using UnityEditor.Timeline;
using System.Collections;
using TMPro;

public class GameManager : MonoBehaviour
{
    public Tilemap groundTilemap;
    public int currencyCount = 0;
    public float currencyCooldown = 3;

    private float cooldownLeft;
    private Dictionary<Vector3Int, GameObject> placedTowers = new Dictionary<Vector3Int, GameObject>();
    private TowerManager towerManager;
    private TextMeshProUGUI currencyCountText;

    private void Start()
    {
        cooldownLeft = currencyCooldown;
        StartCoroutine(CooldownTimer());
        currencyCountText = GameObject.FindGameObjectWithTag("Cost").GetComponent<TextMeshProUGUI>();
        currencyCountText.SetText("" + currencyCount);
    }

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

    private IEnumerator CooldownTimer()
    {
        while (cooldownLeft > 0)
        {
            cooldownLeft--;
            yield return new WaitForSeconds(1);
        }
        cooldownLeft = currencyCooldown;
        currencyCount += 1;
        currencyCountText.SetText("" + currencyCount);
        StartCoroutine(CooldownTimer());
    }

    public void AddToCurrency(int add)
    {
        currencyCount += add;
        currencyCountText.SetText("" + currencyCount);
    }
}
