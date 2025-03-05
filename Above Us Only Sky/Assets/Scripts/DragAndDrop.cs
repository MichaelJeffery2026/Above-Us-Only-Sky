using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;

public class DragAndDrop : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public GameObject towerPrefab;  
    public bool isUnique = false;
    private GameObject currentTower;
    private SpriteRenderer currentSpriteRenderer;
    private Camera mainCamera;       
    private GameManager gameManager;
    private Tower towerScript;

    private void Start()
    {
        mainCamera = Camera.main;
        gameManager = FindObjectOfType<GameManager>();
        towerScript = towerPrefab.GetComponent<Tower>();
        isUnique = towerScript.IsUnique;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
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

    public void OnDrag(PointerEventData eventData)
    {
        // Update the position of the tower sprite as the mouse moves (during drag)
        if (currentTower != null)
        {
            Vector3 mouseWorldPosition = mainCamera.ScreenToWorldPoint(eventData.position);
            Vector3 towerPosition = new Vector3(mouseWorldPosition.x, mouseWorldPosition.y, 0);

            currentTower.transform.position = towerPosition;
        }
    }
    

    public void OnEndDrag(PointerEventData eventData)
    {
        if (currentTower != null)
        {
            Vector3 mouseWorldPosition = mainCamera.ScreenToWorldPoint(eventData.position);
            Vector3Int tilePosition = gameManager.groundTilemap.WorldToCell(mouseWorldPosition);

            bool placedSuccessfully = gameManager.TryPlaceTower(tilePosition, towerPrefab);
            Destroy(currentTower);
            currentTower = null;

            if (placedSuccessfully && isUnique)
            {
                this.gameObject.SetActive(false);
            }
        }
    }
}
