using UnityEngine;
using System.Collections.Generic;

public class HatMenu : MonoBehaviour
{
    private Canvas _canvas;
    [SerializeField]private RectTransform _hatGrid;

    [SerializeField] private GameObject _hatRow;
    [SerializeField] private GameObject _hatSlot;

    [SerializeField] private PlayerHatManager _hatManager;
    [SerializeField] private Material _unavailableMat;

    [SerializeField] private int itemSize = 325;

    private void Start()
    {
        if(gameObject.TryGetComponent(out _canvas))
        {
            _canvas.worldCamera = Camera.main;
            _canvas.planeDistance = 1f;
        }

        DisplayHatMenu(_hatManager.Inventory.GetDatabaseIDs(), 1);
    }

    private void DisplayHatMenu(List<int> IDs, int rowQuantity)
    {
        int remainingHats = IDs.Count;

        RectTransform currentRow;
        HatUIDisplayItem currentSlot;

        _hatGrid.sizeDelta = new Vector2(itemSize * rowQuantity, itemSize * Mathf.Ceil(IDs.Count / (float)rowQuantity));

        for (int i = 0; i < IDs.Count;)
        {
            currentRow = Instantiate(_hatRow, _hatGrid).GetComponent<RectTransform>();
            currentRow.sizeDelta = new Vector2(0, itemSize);

            for (int ii = 0; ii < rowQuantity; ii++)
            {
                if(remainingHats > 0)
                {
                    currentSlot = Instantiate(_hatSlot, currentRow).GetComponent<HatUIDisplayItem>();
                    currentSlot.SetDisplay(currentRow.rect, _hatManager.Inventory.TryGetHat(IDs[i]), _hatManager, _unavailableMat);


                    remainingHats--;
                    i++;
                }
                else
                {
                    currentSlot = Instantiate(_hatSlot, currentRow).GetComponent<HatUIDisplayItem>();
                    currentSlot.SetEmpty();

                    i++;
                }
            }

        }
    }
}
