using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.PackageManager.Requests;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class TowerInfo
{
    public GameObject towerPrefab;
    public string towerName;
    public int towerCooldown;
    public Sprite sprite;
    public bool isDisabled = false;
    public bool isUnique = false;
    public int cooldownLeft = 0;

    public void Initialize()
    {
        if (towerPrefab != null)
        {
            GameObject tempInstance = GameObject.Instantiate(towerPrefab);
            Tower tower = tempInstance.GetComponent<Tower>();

            if (tower == null)
            {
                Debug.LogError($"Tower component missing on {towerPrefab.name}");
            }

            towerName = tower.towerName;
            towerCooldown = tower.cooldown;
            isUnique = tower.IsUnique;

            GameObject.Destroy(tempInstance);
        }
    }
}

[System.Serializable]
public class PanelInfo
{
    public DragAndDrop panel;
    public Image disabledCover;
    public Image sprite;
    public TextMeshProUGUI cooldown;

    public void Initialize()
    {
        if (panel != null)
        {
            foreach (Image i in panel.GetComponentsInChildren<Image>(true)) {
                if (i.CompareTag("DisabledCover"))
                {
                    disabledCover = i;
                    disabledCover.enabled = false;
                }

                if (i.CompareTag("Sprite"))
                {
                    sprite = i;
                }
            }

            cooldown = panel.GetComponentInChildren<TextMeshProUGUI>();
        }
    }
}

public class TowerManager : MonoBehaviour
{
    public List<TowerInfo> towers;
    public List<PanelInfo> panels;
    private int currentStartIndex = 0;

    // Start is called before the first frame update
    void Start()
    {
        foreach (PanelInfo p in panels)
        {
            p.Initialize();
        }

        foreach (TowerInfo t in towers)
        {
            t.Initialize();
        }

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
        if (currentStartIndex + panels.Count < towers.Count)
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
        //Debug.Log("Change Panels");
        for (int i = 0; i < panels.Count; i++)
        {
            panels[i].sprite.sprite = towers[currentStartIndex + i].sprite;
            panels[i].panel.towerPrefab = towers[currentStartIndex + i].towerPrefab;
            panels[i].panel.UpdateCost();

            if (towers[currentStartIndex + i].isDisabled && towers[currentStartIndex + i].cooldownLeft == int.MaxValue)
            {
                panels[i].panel.enabled = false;
                panels[i].disabledCover.enabled = true;
                panels[i].cooldown.enabled = false;
            } else if (towers[currentStartIndex + i].isDisabled)
            {
                panels[i].panel.enabled = false;
                panels[i].disabledCover.enabled = true;
                panels[i].cooldown.enabled = true;
               panels[i].cooldown.SetText("" + towers[currentStartIndex + i].cooldownLeft);
            } else
            {
                panels[i].panel.enabled = true;
                panels[i].disabledCover.enabled = false;
                panels[i].cooldown.enabled = false;
            }
        }
    }

    public void placedTower(Tower tower)
    {
        for (int i = 0; i < towers.Count; i++)
        {
            if (towers[i].towerName == tower.towerName)
            {
                towers[i].isDisabled =  true;
                towers[i].cooldownLeft = towers[i].towerCooldown;

                if (towers[i].isUnique)
                {
                    towers[i].isDisabled = true;
                    towers[i].cooldownLeft = int.MaxValue;
                    ChangePanels();
                }
                else
                {
                    StartCoroutine(CooldownTimer(towers[i]));
                }
            }
        }
    }

    public void TowerDied(Tower tower)
    {
        for (int i = 0; i < towers.Count; i++)
        {
            if (towers[i].towerName == tower.towerName)
            {
                towers[i].cooldownLeft = towers[i].towerCooldown;
                StartCoroutine(CooldownTimer(towers[i]));
            }
        }
    }

    private IEnumerator CooldownTimer(TowerInfo towerInfo)
    {
        while (towerInfo.cooldownLeft > 0)
        {
            towerInfo.cooldownLeft--;
            ChangePanels();
            yield return new WaitForSeconds(1);
        }

        towerInfo.isDisabled = false;
        ChangePanels();
    }
}
