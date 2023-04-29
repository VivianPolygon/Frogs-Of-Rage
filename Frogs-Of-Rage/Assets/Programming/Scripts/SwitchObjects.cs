using UnityEngine;

public class SwitchObjects : MonoBehaviour
{
    public GameObject objectToDisable;
    public GameObject objectToEnable;

    public void Switch()
    {
        if (objectToDisable != null)
        {
            objectToDisable.SetActive(false);
        }

        if (objectToEnable != null)
        {
            objectToEnable.SetActive(true);
        }
    }
}
