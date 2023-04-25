using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.Events;

//retreives copies of the data from the database
public class HatDatabaseRetreiver
{
    private HatDatabase _loadedDatabase;

    private HatDatabaseLoadedEvent _onDatabaseLoad;
    public HatDatabaseLoadedEvent OnDatabaseLoad
    {
        get
        {
            if(_onDatabaseLoad == null)
            {
                _onDatabaseLoad = new HatDatabaseLoadedEvent();
            }
            return _onDatabaseLoad;
        }
    }

    private HatDatabaseAddEvent _onDataAdd;
    public HatDatabaseAddEvent OnDataAdd
    {
        get
        {
            if (_onDataAdd == null)
            {
                _onDataAdd = new HatDatabaseAddEvent();
            }
            return _onDataAdd;
        }
    }

    public void LoadDatabase()
    {
        if(_loadedDatabase == null)
        {
            AsyncOperationHandle<HatDatabase> handle = Addressables.LoadAssetAsync<HatDatabase>("Assets/Databases/HatDatabase.asset");

            handle.Completed += SetDatabase;
        }
        else
        {
            OnDatabaseLoad.Invoke(_loadedDatabase);
        }
    }
    private void SetDatabase(AsyncOperationHandle<HatDatabase> handle)
    {
        if(handle.Status == AsyncOperationStatus.Succeeded)
        {
            HatDatabase databaseInstance = ScriptableObject.CreateInstance<HatDatabase>();
            databaseInstance = handle.Result;
            _loadedDatabase = databaseInstance;
            OnDatabaseLoad.Invoke(_loadedDatabase);
        }
    }

#if UNITY_EDITOR


#endif
}

public class HatDatabaseLoadedEvent: UnityEvent<HatDatabase> { }
public class HatDatabaseAddEvent: UnityEvent<HatData> { }





