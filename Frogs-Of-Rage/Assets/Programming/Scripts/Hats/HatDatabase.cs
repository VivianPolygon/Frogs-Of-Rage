using System.Collections.Generic;
using UnityEngine;

public class HatDatabase : ScriptableObject
{

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

#if UNITY_EDITOR 
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
#endif
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
