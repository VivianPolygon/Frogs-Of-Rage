using UnityEngine;
using UnityEngine.UI;

public class HatUIDisplayItem : MonoBehaviour
{
    [SerializeField] private RectTransform _hatModelTransform;
    [SerializeField] private float _spinSpeed = 10;

    [SerializeField] private Text _hatNameText;
    [SerializeField] private Text _playerEquiptHatText;

    [SerializeField] private Button _equiptButton;
    public Button EquiptButton
    {
        get { return _equiptButton; }
    }

    [SerializeField] private MeshFilter _displayMesh;
    [SerializeField] private MeshRenderer _displayMat;

    private Vector3 _rotationVector;
    private PlayerHatManager _playerHatManager;
    private int _itemID;
    bool _isCulled = false;


    //update rotates the model
    private void Update()
    {
        _hatModelTransform.Rotate(_rotationVector * Time.fixedDeltaTime);
    }

    //function that equipts the hat to the player attatched to the button through code
    private void EquiptHatButton()
    {
        if (_playerHatManager != null)
            _playerHatManager.EquiptHat(_itemID);

    }

    //called on extra slots in a row, makes them invisible
    public void SetEmpty()
    {
        if(!_isCulled)
        {
            _itemID = -10000;
            Mask cullingMask = gameObject.AddComponent<Mask>();
            cullingMask.rectTransform.sizeDelta = Vector2.zero;

            if (gameObject.TryGetComponent(out Image image))
            {
                image.color = Color.clear;
            }

            if (_displayMesh != null)
            {
                _displayMesh.mesh = null;
            }

            _isCulled = true;
        }
    }

    //standin for awake, initilizes the hat display item and all of its data
    public void SetDisplay(Rect parentTransform, HatData data, PlayerHatManager hatsManager, Material unavailableMat)
    {
        //sets UI scale
        RectTransform rTransform = GetComponent<RectTransform>();
        rTransform.sizeDelta = new Vector2(parentTransform.height, parentTransform.height);

        bool playerHasHat = hatsManager.Inventory.CheckForHatAquired(data.hatID);

        //sets hat name text above hat
        if (_hatNameText != null)
        {
            if(playerHasHat)
            {
                _hatNameText.text = data.hatName;
            }
            else
            {
                _hatNameText.text = "???";
            }
        }

        //sets the mesh on the display hat
        _displayMesh.mesh = data.hatMesh;

        //sets material on display hat
        if (playerHasHat)
        {
            _displayMat.material = data.hatMaterial;
        }
        else
        {
            _displayMat.material = unavailableMat;
        }

        //sets the text on the equipt button
        if(_playerEquiptHatText != null)
        {
            if (playerHasHat)
            {
                if (hatsManager.CurrentHatID == data.hatID)
                {
                    _playerEquiptHatText.text = "Its on Your Head";
                }
                else
                {
                    _playerEquiptHatText.text = "Wear Hat";
                }
            }
            else
            {
                _playerEquiptHatText.text = "";
            }
        }

        //sets the equipt button's active state based on if the player has the hat or not
        if (_equiptButton != null)
        {
            if (playerHasHat)
                _equiptButton.gameObject.SetActive(true);
            else
                _equiptButton.gameObject.SetActive(false);
        }

        //sets private variables used by button function
        _playerHatManager = hatsManager;
        _itemID = data.hatID;

        //adds the button function to the button
        if (_equiptButton)
        {
            _equiptButton.onClick.AddListener(EquiptHatButton);
        }


        //initilizes the display rotation vector
        _rotationVector = new Vector3(0, _spinSpeed, 0);
    }

    public void UpdateDisplay(PlayerHatManager hatsManager, Material unavailableMaterial)
    {
        bool playerHasHat = hatsManager.Inventory.CheckForHatAquired(_itemID);
        HatData data = hatsManager.Inventory.TryGetHat(_itemID);

        if(data != null)
        {
            //sets hat name text above hat
            if (_hatNameText != null)
            {
                if (playerHasHat)
                {
                    _hatNameText.text = data.hatName;
                }
                else
                {
                    _hatNameText.text = "???";
                }

            }

            //sets the mesh on the display hat
            _displayMesh.mesh = data.hatMesh;

            //sets material on display hat
            if (playerHasHat)
            {
                _displayMat.material = data.hatMaterial;
            }
            else
            {
                _displayMat.material = unavailableMaterial;
            }

            //sets the text on the equipt button
            if (_playerEquiptHatText != null)
            {
                if (playerHasHat)
                {
                    if (hatsManager.CurrentHatID == data.hatID)
                    {
                        _playerEquiptHatText.text = "Its on Your Head";
                    }
                    else
                    {
                        _playerEquiptHatText.text = "Wear Hat";
                    }
                }
                else
                {
                    _playerEquiptHatText.text = "";
                }
            }

            //sets the equipt button's active state based on if the player has the hat or not
            if(_equiptButton != null)
            {
                if(playerHasHat)
                    _equiptButton.gameObject.SetActive(true);
                else
                    _equiptButton.gameObject.SetActive(false);
            }
        }
        else
        {
            SetEmpty();
        }
    }

    //updates just the equipt text on the hat. is used in the HatMenu script as a helper function and subscribed to all equipt buttons there.
    public void UpdateEquiptText()
    {
        if(_playerHatManager != null)
        {
            if(_playerHatManager.Inventory != null)
            {
                bool playerHasHat = _playerHatManager.Inventory.CheckForHatAquired(_itemID);
                HatData data = _playerHatManager.Inventory.TryGetHat(_itemID);

                //sets the text on the equipt button
                if (_playerEquiptHatText != null && _playerHatManager != null)
                {
                    if (playerHasHat)
                    {
                        if (_playerHatManager.CurrentHatID == data.hatID)
                        {
                            _playerEquiptHatText.text = "Its on Your Head";
                        }
                        else
                        {
                            _playerEquiptHatText.text = "Wear Hat";
                        }
                    }
                    else
                    {
                        _playerEquiptHatText.text = "";
                    }
                }
            }
        }
    }
}
