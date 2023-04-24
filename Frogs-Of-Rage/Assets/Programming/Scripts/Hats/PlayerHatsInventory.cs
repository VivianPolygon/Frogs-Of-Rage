using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//functions as a list of bools mapped to IDs from the hat database. 
//if a bool is true, the player can equipt the hat that coresponds to the ID
public class PlayerHatsInventory
{
    public List<HatData> _databaseCopy;

    //controls which hats the player has.
    public Dictionary<int, bool> _playerHats;



    /// <summary>
    /// Search and return a copy of the hat data by the inputted ID. returns null if the id isint present.
    /// </summary>
    /// <param name="ID"> Hat ID </param>
    /// <returns></returns>
    public HatData TryGetHat(int ID)
    {
        if(_databaseCopy != null)
        {
            for (int i = 0; i < _databaseCopy.Count; i++)
            {
                if(ID == _databaseCopy[i].hatID)
                {
                    return _databaseCopy[i].GetCopy();
                }
            }
        }

        return null;
    }
    /// <summary>
    /// Search and return a copy of the hat data by the inputted name. returns null if the name isint present.
    /// </summary>
    /// <param name="hatName"></param>
    /// <returns></returns>
    public HatData TryGetHat(string hatName)
    {
        if (_databaseCopy != null)
        {
            for (int i = 0; i < _databaseCopy.Count; i++)
            {
                if (hatName == _databaseCopy[i].hatName)
                {
                    return _databaseCopy[i].GetCopy();
                }
            }
        }

        return null;
    }

    /// <summary>
    /// returns true if the player has unlocked the given hat, and false otherwise
    /// </summary>
    /// <param name="ID"> ID of the hat to check for </param>
    /// <returns></returns>
    public bool CheckForHatAquired(int ID)
    {
        if (_playerHats != null)
        {
            bool state;
            if (_playerHats.TryGetValue(ID, out state))
            {
                return state;
            }
            else
            {
                return false;
            }

        }
        return false;
    }

    /// <summary>
    /// returns the sorted list of IDS in the database. lowest to highest
    /// </summary>
    /// <returns></returns>
    public List<int> GetDatabaseIDs()
    {
        GetHatsFromDatabase();
        List<int> returnList = new List<int>();
        _databaseCopy = UtilityFunctions.SortByHatID(_databaseCopy);

        for (int i = 0; i < _databaseCopy.Count; i++)
        {
            returnList.Add(_databaseCopy[i].hatID);
        }

        return returnList;
    }

    public void UnlockHat(int ID)
    {
        if(_playerHats != null)
        {
            if(_playerHats.TryGetValue(ID, out bool state))
            {
                _playerHats.Remove(ID);
                _playerHats.Add(ID, true);
            }

        }
    }

    //will not overwrite hats that the player already has values for
    private void GetHatsFromDatabase()
    {
        if(_databaseCopy != null)
        {
            //player hat inventory is initilized. adds any new hats from the database and sets them to false.
            if (_playerHats != null)
            {
                for (int i = 0; i < _databaseCopy.Count; i++)
                {
                    if (!_playerHats.TryGetValue(_databaseCopy[i].hatID, out bool state))
                    {
                        _playerHats.Add(_databaseCopy[i].hatID, false);
                    }
                }
            }
        }


    }

    public void SaveHatInventory()
    {
        SaveManager.SavePlayerHats(_playerHats);
    }

    public void LoadHatInventory()
    {
       _playerHats = SaveManager.LoadPlayerHats();
        GetHatsFromDatabase();
    }



    public PlayerHatsInventory(HatDatabase database)
    {
        _databaseCopy = database.GetDatabaseCopy();
    }

}
