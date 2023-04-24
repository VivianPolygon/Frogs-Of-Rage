using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHatManager : MonoBehaviour
{
    public Transform headJoint;
    private GameObject _hatObject;

    //offsets off of the base head joint. gets added to rotation and position when creating hat object.
    private Vector3 _rotationOffset;
    private Vector3 _positionOffset;

    public PlayerHatsInventory Inventory
    {
        get;
        private set;
    }


    [SerializeField] private HatDatabase _hatDatabase;
    public List<HatData> HatDatabase
    {
        get
        {
            if(_hatDatabase != null)
            {
                return _hatDatabase.GetDatabaseCopy();
            }
            return null;
        }
    }

    public int CurrentHatID
    {
        get;
        private set;
    }

    private void Awake()
    {
        Inventory = new PlayerHatsInventory(_hatDatabase);

        Inventory.LoadHatInventory();

        _positionOffset = new Vector3(0.00017f, -0.00038f, -0.00028f);
        _rotationOffset = new Vector3(18.357f, -160.113f, -41.072f);
    }



    public void UnlockHat(int hatID)
    {
        Inventory.UnlockHat(hatID);
        Inventory.SaveHatInventory();
    }

    public void EmptyPlayerInventory()
    {
        Inventory._playerHats = new Dictionary<int, bool>();
        Inventory.SaveHatInventory();
        Inventory.LoadHatInventory();
    }

    public void EquiptHat(int hatID)
    {
        //an ID of 0 hides hats
        if(hatID == 0)
        {
            if(_hatObject)
            {
                _hatObject.SetActive(false);
            }

            CurrentHatID = hatID;
            return;
        }

        if(Inventory._playerHats.TryGetValue(hatID, out bool hasHat))
        {
            if(hasHat)
            {
                if (headJoint)
                {
                    HatData data = Inventory.TryGetHat(hatID);
                    if (data != null)
                    {
                        if (!_hatObject)
                        {
                            _hatObject = new GameObject();
                            _hatObject.name = "Player Hat";
                            _hatObject.AddComponent<MeshFilter>();
                            _hatObject.AddComponent<MeshRenderer>();
                            _hatObject.transform.parent = headJoint;

                            _hatObject.transform.position = headJoint.position;
                            _hatObject.transform.position += _positionOffset;

                            _hatObject.transform.rotation = headJoint.rotation;
                            _hatObject.transform.Rotate(_rotationOffset);

                            _hatObject.transform.localScale = Vector3.one;
                        }
                        _hatObject.SetActive(true);

                        _hatObject.GetComponent<MeshFilter>().mesh = data.hatMesh;
                        _hatObject.GetComponent<MeshRenderer>().material = data.hatMaterial;
                        _hatObject.name += ": " + data.hatName;

                        CurrentHatID = hatID;
                        return;
                    }
                }
                else
                {
                    Debug.LogWarning("No Headjoint assigned for displaying hats");
                }
            }

        }


    }

}
