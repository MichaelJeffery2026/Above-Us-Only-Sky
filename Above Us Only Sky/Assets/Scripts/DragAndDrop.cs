using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;

public class DragAndDrop : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public GameObject towerPrefab;  
    public bool isUnique = false;
    public TextMeshProUGUI costText;

    private GameObject currentTower;
    private SpriteRenderer currentSpriteRenderer;
    private Camera mainCamera;       
    private GameManager gameManager;
    private Tower towerScript;
    private int cost;

    private void Start()
    {
        mainCamera = Camera.main;
        gameManager = FindObjectOfType<GameManager>();
        towerScript = towerPrefab.GetComponent<Tower>();
        isUnique = towerScript.IsUnique;
        cost = towerScript.towerCost;
        costText.SetText("" + cost);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (gameManager.currencyCount >= cost) {
            // Create a new tower instance when dragging starts from the UI image
            Vector3 mouseWorldPosition = mainCamera.ScreenToWorldPoint(eventData.position);
            Vector3 towerPosition = new Vector3(mouseWorldPosition.x, mouseWorldPosition.y, 0);

            currentTower = new GameObject("DraggingTower");
            currentSpriteRenderer = currentTower.AddComponent<SpriteRenderer>();
            currentSpriteRenderer.sortingLayerName = "Character";

            GameObject tempInstance = GameObject.Instantiate(towerPrefab);
            Sprite sprite = tempInstance.GetComponent<SpriteRenderer>().sprite;

            GameObject.Destroy(tempInstance);

            currentSpriteRenderer.sprite = sprite;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        // Update the position of the tower sprite as the mouse moves (during drag)
        if (gameManager.currencyCount >= cost && currentTower != null)
        {
            Vector3 mouseWorldPosition = mainCamera.ScreenToWorldPoint(eventData.position);
            Vector3 towerPosition = new Vector3(mouseWorldPosition.x, mouseWorldPosition.y, 0);

            currentTower.transform.position = towerPosition;
        }
    }
    

    public void OnEndDrag(PointerEventData eventData)
    {
        if (gameManager.currencyCount >= cost && currentTower != null)
        {
            Vector3 mouseWorldPosition = mainCamera.ScreenToWorldPoint(eventData.position);
            Vector3Int tilePosition = gameManager.groundTilemap.WorldToCell(mouseWorldPosition);

            bool placedSuccessfully = gameManager.TryPlaceTower(tilePosition, towerPrefab);
            Destroy(currentTower);
            currentTower = null;

            if (placedSuccessfully)
            {
                gameManager.AddToCurrency(-cost);
            }
        }
    }

    public void UpdateCost()
    {
        towerScript = towerPrefab.GetComponent<Tower>();
        isUnique = towerScript.IsUnique;
        cost = towerScript.towerCost;
        costText.SetText("" + cost);
    }

}
