using UnityEngine;
using UnityEngine.UI;

public class HatUIDisplayItem : MonoBehaviour
{
    [SerializeField] private RectTransform _hatModelTransform;
    [SerializeField] private float _spinSpeed;

    [SerializeField] private Text _hatNameText;
    [SerializeField] private Text _playerEquiptHatText;

    [SerializeField] private MeshFilter _displayMesh;
    [SerializeField] private MeshRenderer _displayMat;

    private Vector3 _rotationVector;

    private void Awake()
    {
        _rotationVector = new Vector3(0, _spinSpeed, 0);
    }

    private void RotateModel(Vector3 rotation)
    {
        _hatModelTransform.Rotate(rotation);
    }

    private void Update()
    {
        RotateModel(_rotationVector * Time.deltaTime);
    }


    public void SetEmpty()
    {
        Mask cullingMask = gameObject.AddComponent<Mask>();
        cullingMask.rectTransform.sizeDelta = Vector2.zero;
    }
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
                    _playerEquiptHatText.text = "Hat Equipt";
                }
                else
                {
                    _playerEquiptHatText.text = "Equipt Hat";
                }
            }
            else
            {
                _playerEquiptHatText.text = "Not Found";
            }
        }

    }
}
