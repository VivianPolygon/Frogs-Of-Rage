using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.Events;

//used to laod the dev times from the asset bundle.
public class DevTimesLoader
{
    public DevScoresLoad loadEvent;

    public void LoadDevTimes()
    {
        loadEvent = new DevScoresLoad();

        AsyncOperationHandle<TextAsset> handle = Addressables.LoadAssetAsync<TextAsset>("Assets/Databases/DefaultScores.json");

        handle.Completed += ProcessLoad;
    }

    private void ProcessLoad(AsyncOperationHandle<TextAsset> handle)
    {
        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            if(loadEvent != null)
                loadEvent.Invoke(handle.Result);

            Debug.Log("Sucsessfuly loaded dev times from asset bundle");
        }
        else
        {
            if (loadEvent != null)
                loadEvent.Invoke(null);

            Debug.Log("Couldnt load JSON from asset bundle");
        }
    }

}

public class DevScoresLoad : UnityEvent<TextAsset> { }


