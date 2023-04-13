using System.Collections.Generic;
using UnityEngine;

public class HatDatabase : ScriptableObject
{
    private static string _resourcesFilePath = "HatDatabase";
    private static string _filePath = "Assets/Resources/" + _resourcesFilePath + ".asset"; //used to acsess the asset's file path
    public static string FilePath
    {
        get
        {
            return _filePath;
        }
    } //filepath from the project folder.
    public static string ResourcesFilePath
    {
        get
        {
            return _resourcesFilePath;
        }
    } //filepath from the resources folder (for Resources.Load)

    [SerializeField] private List<HatData> _hatDatalist;
    /// <summary>
    /// Property for Hatdatalist. Should only be set from the editor window
    /// </summary>
    public List<HatData> HatDatalist
    {
        get 
        {
            if(_hatDatalist != null) //returns a copy
            {
                return GetDatabaseCopy();
            }
            else
            {
                Debug.LogWarning("Hat Database is empty");
                return new List<HatData>();
            }
        } 
        
        set
        {
            if(HatsEditor.WindowOpen) //can only write if the hats editor window is open. its not foolproof, but should prevent unauthorized writing
            {
                _hatDatalist = value;
            }
            else
            {
                Debug.LogWarning("The Hat database should only be modified through the editor window. nothing was written");
            }
        }
    }


    /// <summary>
    /// Clone the hat database as a list of HatData
    /// </summary>
    /// <returns></returns>
    public List<HatData> GetDatabaseCopy()
    {
        if(_hatDatalist == null)
        {
            _hatDatalist = new List<HatData>();
        }

        if(_hatDatalist.Count < 1)
        {
            //Debug.LogWarning("Hat Database is empty");
        } //warning that database is empty if count is 0

        List<HatData> copy = new List<HatData>();

        for (int i = 0; i < _hatDatalist.Count; i++)
        {
            copy.Add(_hatDatalist[i].GetCopy());
        }

        return copy;
    }


}
