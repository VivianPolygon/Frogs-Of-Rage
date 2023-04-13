using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

//retreives copies of the data from the database
public class HatDatabaseRetreiver
{
    public List<HatData> RetreiveDatabaseCopy()
    {
        HatDatabase databaseInstance = ScriptableObject.CreateInstance<HatDatabase>();
        databaseInstance = (HatDatabase)Resources.Load<HatDatabase>(HatDatabase.ResourcesFilePath);

        if (databaseInstance)
        {
            return databaseInstance.GetDatabaseCopy();
        }
        else
        {
            Debug.LogWarning("No Database to Get");
            return new List<HatData>();
        }

    }
}







