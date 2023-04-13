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

    public PlayerHatsInventory _inventory = new PlayerHatsInventory();


    private void Awake()
    {
        _inventory.LoadHatInventory();

        _positionOffset = new Vector3(0.00017f, -0.00038f, -0.00028f);
        _rotationOffset = new Vector3(18.357f, -160.113f, -41.072f);
    }

    public void UnlockHat(int hatID)
    {
        _inventory.UnlockHat(hatID);
        _inventory.SaveHatInventory();
    }

    public void EmptyPlayerInventory()
    {
        _inventory._playerHats = new Dictionary<int, bool>();
        _inventory.SaveHatInventory();
        _inventory.LoadHatInventory();
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
        }

        if(_inventory._playerHats.TryGetValue(hatID, out bool hasHat))
        {
            if(hasHat)
            {
                if (headJoint)
                {
                    HatData data = _inventory.TryGetHat(hatID);
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
