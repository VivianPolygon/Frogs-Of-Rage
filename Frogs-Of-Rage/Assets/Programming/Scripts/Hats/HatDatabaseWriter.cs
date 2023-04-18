using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

#if UNITY_EDITOR 

//writes to the database
public class HatDatabaseWriter
{
    /// <summary>
    /// Adds or Overwrites data in the database
    /// </summary>
    /// <param name="newData"> new data to add or overwrite </param>
    public void AddToDataBase(HatData newData)
    {
        bool overwritten = false;

        if(newData != null)
        {
            HatDatabaseRetreiver retreiver = new HatDatabaseRetreiver();
            List<HatData> copiedData = retreiver.RetreiveDatabaseCopy();
            retreiver = null;


            if(copiedData != null)
            {
                for (int i = 0; i < copiedData.Count; i++)
                {
                    if(copiedData[i].hatID == newData.hatID)
                    {
                        if(copiedData[i].hatName == newData.hatName && !overwritten)
                        {
                            //overwrite
                            copiedData[i] = newData.GetCopy();
                            overwritten = true;
                            break;
                        }
                    }
                }

                if(!overwritten)
                {
                    //appends
                    copiedData.Add(newData);
                }

                //sorts list by ID before saving
                copiedData = UtilityFunctions.SortByHatID(copiedData);

                //writes to the database
                HatDatabase hatDatabase = ScriptableObject.CreateInstance<HatDatabase>();
                hatDatabase.HatDatalist = copiedData;

                AssetDatabase.CreateAsset(hatDatabase, HatDatabase.FilePath);

                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
        }
    }

    public void EraseHat(int idToErase)
    {
        HatDatabaseRetreiver retreiver = new HatDatabaseRetreiver();
        List<HatData> copiedData = retreiver.RetreiveDatabaseCopy();
        retreiver = null;

        bool changesToWrite = false;

        for (int i = 0; i < copiedData.Count; i++)
        {
            if(copiedData[i].hatID == idToErase)
            {
                copiedData.RemoveAt(i);
                changesToWrite = true;
                break;
            }
        }

        if(changesToWrite)
        {
            //writes to the database
            HatDatabase hatDatabase = ScriptableObject.CreateInstance<HatDatabase>();
            hatDatabase.HatDatalist = copiedData;

            AssetDatabase.CreateAsset(hatDatabase, HatDatabase.FilePath);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }
}

#endif
