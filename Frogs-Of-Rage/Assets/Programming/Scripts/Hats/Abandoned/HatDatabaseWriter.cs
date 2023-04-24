using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.AddressableAssets;
using System.Threading.Tasks;

#if UNITY_EDITOR 

//writes to the database
public class HatDatabaseWriter
{
    private HatDatabaseRetreiver _retreiver;
    private HatDatabase _loadedDatabase;

    private void LoadDatabase()
    {
        if(_retreiver == null)
        {
            InitilizeRetreiver();
        }
        _retreiver.LoadDatabase();
    }
    private void InitilizeRetreiver()
    {
        _retreiver = new HatDatabaseRetreiver();
        _retreiver.OnDatabaseLoad.AddListener(SetLoadedDatabase);
        Debug.Log("Retreiving database on Writer");
    }
    private void SetLoadedDatabase(HatDatabase database)
    {
        _loadedDatabase = database;
        Debug.Log("Writer retreived database");
    }


    /// <summary>
    /// Adds or Overwrites data in the database
    /// </summary>
    /// <param name="newData"> new data to add or overwrite </param>
    public void AddToDataBase(HatData newData)
    {
        LoadDatabase();
        System.Threading.Thread.Sleep(2000);


        if(_loadedDatabase != null)
        {
            bool overwritten = false;
            List<HatData> copiedData = _loadedDatabase.GetDatabaseCopy();

            if (newData != null)
            {


                if (copiedData != null)
                {
                    for (int i = 0; i < copiedData.Count; i++)
                    {
                        if (copiedData[i].hatID == newData.hatID)
                        {
                            if (copiedData[i].hatName == newData.hatName && !overwritten)
                            {
                                //overwrite
                                copiedData[i] = newData.GetCopy();
                                overwritten = true;
                                break;
                            }
                        }
                    }

                    if (!overwritten)
                    {
                        //appends
                        copiedData.Add(newData);
                    }

                    //sorts list by ID before saving
                    copiedData = UtilityFunctions.SortByHatID(copiedData);

                    //writes to the database
                    HatDatabase hatDatabase = ScriptableObject.CreateInstance<HatDatabase>();
                    hatDatabase.HatDatalist = copiedData;

                    //saves
                    Debug.Log("Saving New Asset");
                    AssetDatabase.CreateAsset(hatDatabase, "Assets/Databases/HatDatabase.asset");

                }
            }
        }
        else
        {
            //wont work on the first try due to loading times. 
            Debug.Log("Please try Saving again");
        }
    }

    public void EraseHat(int idToErase)
    {
        LoadDatabase();
        System.Threading.Thread.Sleep(2000);

        if (_loadedDatabase != null)
        {
            bool changesToWrite = false;
            List<HatData> copiedData = _loadedDatabase.GetDatabaseCopy();

            for (int i = 0; i < copiedData.Count; i++)
            {
                if (copiedData[i].hatID == idToErase)
                {
                    copiedData.RemoveAt(i);
                    changesToWrite = true;
                    break;
                }
            }

            if (changesToWrite)
            {
                //writes to the database
                HatDatabase hatDatabase = ScriptableObject.CreateInstance<HatDatabase>();
                hatDatabase.HatDatalist = copiedData;

                //saves
                Debug.Log("Erasing Asset");
                AssetDatabase.CreateAsset(hatDatabase, "Assets/Databases/HatDatabase.asset");
            }
        }
    }
}
#endif
