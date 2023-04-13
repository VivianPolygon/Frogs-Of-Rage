using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//functions as a list of bools mapped to IDs from the hat database. 
//if a bool is true, the player can equipt the hat that coresponds to the ID
public class PlayerHatsInventory
{
    private List<HatData> _databaseCopy;

    //controls which hats the player has.
    public Dictionary<int, bool> _playerHats;

    private List<HatData> GetDatabase()
    {
        HatDatabaseRetreiver retreiver = new HatDatabaseRetreiver();
        _databaseCopy = retreiver.RetreiveDatabaseCopy();
        return _databaseCopy;
    }

    /// <summary>
    /// Search and return a copy of the hat data by the inputted ID. returns null if the id isint present.
    /// </summary>
    /// <param name="ID"> Hat ID </param>
    /// <returns></returns>
    private HatData TryGetHat(int ID)
    {
        if(_databaseCopy == null)
        {
            GetDatabase();
        }
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
    private HatData TryGetHat(string hatName)
    {
        if (_databaseCopy == null)
        {
            GetDatabase();
        }
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


        //player hat inventory is initilized. adds any new hats from the database and sets them to false.
        if(_playerHats != null)
        {
            GetDatabase();

            for (int i = 0; i < _databaseCopy.Count; i++)
            {
                if(!_playerHats.TryGetValue(_databaseCopy[i].hatID, out bool state))
                {
                    _playerHats.Add(_databaseCopy[i].hatID, false);
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

}
