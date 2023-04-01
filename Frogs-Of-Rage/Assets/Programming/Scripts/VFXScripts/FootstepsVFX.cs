using System.Collections;
using UnityEngine;

public class FootstepsVFX : MonoBehaviour
{
    [SerializeField] [Min(2)] private int _poolSize = 10;
    [SerializeField] [Min(0.25f)] private float _footstepFadeTime = 1.5f;
    [SerializeField] [Min(0.05f)] private float _footstepScale = 0.5f;

    [SerializeField] private Shader _footStepshader;
    [SerializeField] private Color _footstepColor;

    private Transform _footstepPool;

    public enum FootstepFoot
    {
        Left,
        Right
    }


    private void Start()
    {
        InvokeRepeating("TestSpawnFootprint", 1, 0.5f);
    }

    private void OnEnable()
    {
        CreateQuadPool(_poolSize, transform);
    } 
    private void OnDisable()
    {
        if (_footstepPool)
            Destroy(_footstepPool.gameObject);
        _footstepPool = null;
    }


    private struct FootstepData
    {
        public GameObject footstepObject;
        public MeshRenderer footstepRenderer;
        public Transform footstepParent;
    }

    private GameObject SetUpFootstepParticle(Material footstepMat, int particleNumber)
    {
        GameObject newFootstepParticle = GameObject.CreatePrimitive(PrimitiveType.Quad);


        newFootstepParticle.name = "Footstep Quad " + particleNumber;
        newFootstepParticle.transform.parent = _footstepPool;

        if (newFootstepParticle.TryGetComponent(out MeshRenderer renderer)) //sets up the mesh renderer
        {
            renderer.material = footstepMat;
            renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        }

        if (newFootstepParticle.gameObject.TryGetComponent(out Collider collider)) // removes the collider thats on the default quad
        {
            collider.enabled = false;
            Destroy(collider);
        }

        newFootstepParticle.SetActive(false);
        return newFootstepParticle;
    }

    private GameObject GetParticleFromPool()
    {
        if(!_footstepPool)
        {
            CreateQuadPool(_poolSize, transform);
        }

        if(_footstepPool)
        {
            for (int i = 0; i < _footstepPool.childCount; i++)
            {
                if (!_footstepPool.GetChild(i).gameObject.activeSelf)
                {
                    return _footstepPool.GetChild(i).gameObject;
                }
            }
        }

        return null;
    }

    private void ScaleFootstep(Transform footstepTransform, float scale, FootstepFoot foot)
    {
        switch (foot)
        {
            case FootstepFoot.Left:
                footstepTransform.localScale = Vector3.one * scale;
                break;
            case FootstepFoot.Right:
                footstepTransform.localScale = Vector3.one * scale + (Vector3.left * scale * 2);
                break;
            default:
                break;
        }
    }

    private void CreateQuadPool(int size, Transform poolParent)
    {
        if(!poolParent)
        {
            Debug.LogWarning("Couldn't create a footstep pool due to the pool parent inputted being null");
            return;
        }
        if(size < 2)
        {
            Debug.LogWarning("Couldn't create a footstep pool due to the inputed size being to small. pool should have a size of atleast 2");
            return;
        }

        _footstepPool = new GameObject().transform;
        _footstepPool.gameObject.name = "Player Footstep Pool";
        _footstepPool.transform.parent = poolParent;

        Material footstepMat = new Material(_footStepshader);
        footstepMat.SetColor("_Color", _footstepColor);

        for (int i = 0; i < size; i++)
        {
            SetUpFootstepParticle(footstepMat, i + 1);
        }
    }

    /// <summary>
    /// Spawns a footstep particle
    /// </summary>
    /// <param name="position"> Position to spawn the particle at </param>
    /// <param name="upDirectionNormal"> Direction for the particle to be facing verticaly. should be alligned with the floor </param>
    /// <param name="forwardDirectionNormal"> The direction the footstep is facing lateraly. should be alligned with the player's forward </param>
    /// <param name="foot"> which foot made the imprint? Left or Right? </param>
    public void SpawnFootstep(Vector3 position, Vector3 upDirectionNormal, Vector3 forwardDirectionNormal, FootstepFoot foot)
    {
        if(!_footstepPool)
        {
            CreateQuadPool(_poolSize, transform);
        }

        if(_footstepPool)
        {
            GameObject footstep = GetParticleFromPool();
            if(footstep)
            {
                footstep.transform.position = position;
                footstep.transform.forward = -upDirectionNormal;
                footstep.transform.RotateAround(footstep.transform.position, upDirectionNormal, 180 + Vector3.SignedAngle(footstep.transform.up, forwardDirectionNormal, upDirectionNormal));

                FootstepData footstepData;
                footstepData.footstepObject = footstep;
                footstep.TryGetComponent(out footstepData.footstepRenderer);
                footstepData.footstepParent = footstep.transform.parent;

                ScaleFootstep(footstep.transform, _footstepScale, foot);

                StartCoroutine(FadeFootstep(footstepData, _footstepFadeTime));
            }
        }
    }

    private IEnumerator FadeFootstep(FootstepData footstep, float fadeTime)
    {
        footstep.footstepObject.SetActive(true);
        footstep.footstepObject.transform.parent = null;
        for (float t = 0; t < fadeTime; t += Time.deltaTime)
        {
            footstep.footstepRenderer.material.SetFloat("_Fade", t / fadeTime);
            yield return null;
        }
        footstep.footstepObject.transform.parent = footstep.footstepParent;
        footstep.footstepObject.SetActive(false);
    }


    //for testing

    bool left = true;
    private void TestSpawnFootprint()
    {
        left = !left;
        if(left)
        {
            SpawnFootstep(transform.position + (Vector3.up * 0.05f), Vector3.up, transform.forward, FootstepFoot.Left);
        }
        else
        {
            SpawnFootstep(transform.position + (Vector3.up * 0.05f), Vector3.up, transform.forward, FootstepFoot.Right);
        }

    }

}

