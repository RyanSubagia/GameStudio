using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerInteractionUI : MonoBehaviour
{
    public GameObject upgradePanel;
    private Tower selectedTower;

    public void ShowPanel(Tower tower, Vector3 worldPosition)
    {
        selectedTower = tower;

        // Convert world position to screen position
        Vector3 screenPosition = Camera.main.WorldToScreenPoint(worldPosition);

        // Convert screen position to local UI space
        RectTransform canvasRect = upgradePanel.GetComponentInParent<Canvas>().GetComponent<RectTransform>();
        RectTransform panelRect = upgradePanel.GetComponent<RectTransform>();

        Vector2 uiPos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, screenPosition, Camera.main, out uiPos);
        panelRect.anchoredPosition = uiPos;

        upgradePanel.SetActive(true);
    }

    public void OnUpgradeClicked()
    {
        if (selectedTower != null)
        {
            selectedTower.Upgrade();
        }
        upgradePanel.SetActive(false);
    }

    public void OnSellClicked()
    {
        if (selectedTower != null)
        {
            selectedTower.Sell();
        }
        upgradePanel.SetActive(false);
    }

    public void HidePanel()
    {
        upgradePanel.SetActive(false);
        selectedTower = null;
    }
}
