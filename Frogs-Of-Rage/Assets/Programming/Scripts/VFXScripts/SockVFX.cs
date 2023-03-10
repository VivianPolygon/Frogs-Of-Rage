using UnityEngine;

public class SockVFX : MonoBehaviour
{
    [SerializeField] private ParticleSystem VFX1;
    [SerializeField] private ParticleSystem VFX2;
    [SerializeField] private StinkySock stinkySock;

    private bool _variablesInitilized = true;

    private void Awake()
    {
        InitilizeVariables();
        SetVFXFromDamageRadius(VFX1);
        SetVFXFromDamageRadius(VFX2);
    }

    private void InitilizeVariables()
    {
        if (VFX1 && VFX2 && stinkySock)
            return;

        for (int i = 0; i < 2; i++)
        {
            if(transform.childCount > i)
            {
                if(VFX1 == null)
                transform.GetChild(i).TryGetComponent(out VFX1);
                else if(VFX2 == null)
                {
                    transform.GetChild(i).TryGetComponent(out VFX2);
                }
            }
        }

        if (stinkySock == null)
            transform.parent.TryGetComponent(out stinkySock);

        if(!VFX1 || !VFX2 || stinkySock)
        {
            _variablesInitilized = false;
            Debug.LogWarning("Stinky sock: " + name + " Could not set up its VFX, make sure the fields are set up in the inspector");
        }
    }
    private void SetVFXFromDamageRadius(ParticleSystem system)
    {
        if(_variablesInitilized)
        {
            ParticleSystem.ShapeModule shapeModule = system.shape;
            shapeModule.radius = stinkySock.range;

            ParticleSystem.MainModule moduleMain = system.main;
            moduleMain.prewarm = true;  
        }
    }

}
