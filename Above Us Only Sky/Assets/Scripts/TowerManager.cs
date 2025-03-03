using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public struct TowerInfo
{
    public GameObject towerPrefab;
    public Sprite sprite;
}

[System.Serializable]
public class PanelInfo
{
    public DragAndDrop panel;
    public Image disabledCover;
    public Image sprite;
}

public class TowerManager : MonoBehaviour
{
    public List<TowerInfo> towers;
    public List<PanelInfo> panels;
    private int currentStartIndex = 0;

    // Start is called before the first frame update
    void Start()
    {
        ChangePanels();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void MoveLeft()
    {
        Debug.Log("Clicked Left");
        if (currentStartIndex > 0)
        {
            currentStartIndex--;
            ChangePanels();
        }
        else
        {
            // TODO: Insert sound and visual feedback here
        }
    }

    public void MoveRight()
    {
        Debug.Log("Clicked Right");
        if (currentStartIndex + 4 < towers.Count)
        {
            currentStartIndex++;
            ChangePanels();
        }
        else
        {
            // TODO: Insert sound and visual feedback here
        }
    }

    public void ChangePanels()
    {
        Debug.Log("Change Panels");
        for (int i = 0; i < panels.Count; i++)
        {
            panels[i].sprite.sprite = towers[currentStartIndex + i].sprite;
            panels[i].panel.towerPrefab = towers[currentStartIndex + i].towerPrefab;
        }
    }
}
