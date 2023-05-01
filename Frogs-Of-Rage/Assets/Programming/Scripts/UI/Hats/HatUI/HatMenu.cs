using UnityEngine;
using System.Collections.Generic;

public class HatMenu : MonoBehaviour
{
    private Canvas _canvas;
    [SerializeField] private RectTransform _hatGrid;

    [SerializeField] private GameObject _hatRow;
    [SerializeField] private GameObject _hatSlot;

    private PlayerHatManager HatManager
    {
        get { return PlayerHatManager.Instance; }
    }

    [SerializeField] private Material _unavailableMat;

    [SerializeField] private int itemSize = 325;

    private List<HatUIDisplayItem> _displayItems;

    //function used on the dehatting button to unequipt hat
    public void DeHatButton()
    {
        //equipting hat of ID 0 sets the hat mesh to inactive
        HatManager.EquiptHat(0);
    }

    private void Start()
    {
        if (gameObject.TryGetComponent(out _canvas))
        {
            _canvas.worldCamera = Camera.main;
            _canvas.planeDistance = 1f;
        }
    }

    private void OnEnable()
    {
        if (HatManager != null && HatManager.Inventory != null)
        {
            if (_displayItems == null)
                CreateHatMenu(HatManager.Inventory.GetDatabaseIDs(), 3);
            else
                RefreshHatMenu();
        }
    }

    private void CreateHatMenu(List<int> IDs, int rowQuantity)
    {
        _displayItems = new List<HatUIDisplayItem>();

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
                if (remainingHats > 0)
                {
                    currentSlot = Instantiate(_hatSlot, currentRow).GetComponent<HatUIDisplayItem>();
                    currentSlot.SetDisplay(currentRow.rect, HatManager.Inventory.TryGetHat(IDs[i]), HatManager, _unavailableMat);


                    remainingHats--;
                    i++;
                }
                else
                {
                    currentSlot = Instantiate(_hatSlot, currentRow).GetComponent<HatUIDisplayItem>();
                    currentSlot.SetEmpty();

                    i++;
                }

                //adds refresh function to equipt buttons
                currentSlot.EquiptButton.onClick.AddListener(RefreshHatEquiptTexts);

                //adds to tracked list
                _displayItems.Add(currentSlot);
            }
        }
    }

    private void RefreshHatMenu()
    {
        for (int i = 0; i < _displayItems.Count; i++)
        {
            _displayItems[i].UpdateDisplay(HatManager, _unavailableMat);
        }
    }

    private void RefreshHatEquiptTexts()
    {
        if(_displayItems != null)
        {
            for (int i = 0; i < _displayItems.Count; i++)
            {
                _displayItems[i].UpdateEquiptText();
            }
        }
    }

}