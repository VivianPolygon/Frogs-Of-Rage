using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Rendering;

public class PlayerEmissionVFX : MonoBehaviour
{
    [SerializeField] private Renderer _meshRenderer;

    #region "Variables"
    [SerializeField] float _effectDuration = 2;
    [SerializeField] float _matFlashDuration = 1.25f;

    [SerializeField] float _particleQuantity = 25;
    [SerializeField] float _particleSpeed = 5;


    [Header("Damage Sprite")]
    [Space(5)]
    [SerializeField] private Sprite _damageSprite;
    [SerializeField] private Color _damageMatColor = Color.red;
    [SerializeField] private float _damageSpriteScale = 0.15f;
    [Header("Electrocuted Sprite")]
    [Space(5)]
    [SerializeField] private Sprite _electrocutedSprite;
    [SerializeField] private Color _electrocutedMatColor = Color.black;
    [SerializeField] private float _electrocutedSpriteScale =0.1f;
    [Header("Heal Sprite")]
    [Space(5)]
    [SerializeField] private Sprite _healSprite;
    [SerializeField] private Color _healMatColor = Color.green;
    [SerializeField] private float _healSpriteScale = 0.15f;

    #endregion

    private Transform _particlePool;
    private SpriteRenderer[] _particleRenderers;
    private Coroutine _particleCoroutine;

    private Material[] _playerMaterials;
    private Color[] _initialColors;

    public enum EmitPlayerParticlesMode
    {
        Damage,
        Electrocuted,
        Heal,
    }

    private void Start()
    {
        GetPlayerMat();
        SetUpParticles();
    }

    /* for debugging/examples
    private void Update()
    {
        if(Input.GetKeyDown("1"))
        {
            EmitPlayerParticles(EmitPlayerParticlesMode.Damage);
        }
        if (Input.GetKeyDown("2"))
        {
            EmitPlayerParticles(EmitPlayerParticlesMode.Electrocuted);
        }
        if (Input.GetKeyDown("3"))
        {
            EmitPlayerParticles(EmitPlayerParticlesMode.Heal);
        }
    }
    */

    public void EmitPlayerParticles(EmitPlayerParticlesMode mode)
    {
        switch (mode)
        {
            case EmitPlayerParticlesMode.Damage:
                SpawnParticles(_damageSpriteScale, _damageSprite, _damageMatColor);
                break;
            case EmitPlayerParticlesMode.Electrocuted:
                SpawnParticles(_electrocutedSpriteScale, _electrocutedSprite, _electrocutedMatColor);
                break;
            case EmitPlayerParticlesMode.Heal:
                SpawnParticles(_healSpriteScale, _healSprite, _healMatColor);
                break;
            default:
                break;
        }
    }


    private void SpawnParticles(float particleScale, Sprite sprite, Color matColor)
    {
        if (!_meshRenderer || !_particlePool || _particleCoroutine != null) return;

        Mesh objectMesh = null;
        List<Vector3> vertexPositions = new List<Vector3>();
        List<Vector3> vertexNormals = new List<Vector3>();

        SetParticleSprites(sprite);

        if(_meshRenderer as SkinnedMeshRenderer) // gets skinned renderer if the render is that type so it can use a baked mesh for proper information
        {
            SkinnedMeshRenderer skinnedRenderer = _meshRenderer as SkinnedMeshRenderer;
            objectMesh = skinnedRenderer.sharedMesh;

            skinnedRenderer.BakeMesh(objectMesh, true);
            objectMesh.GetVertices(vertexPositions);
            objectMesh.GetNormals(vertexNormals);
        }    
        else if (_meshRenderer.gameObject.TryGetComponent(out MeshFilter mFilter)) //if its not a skinned mesh, finds the filter and used that mesh
        {
            objectMesh = mFilter.mesh;
            objectMesh.GetVertices(vertexPositions);
            objectMesh.GetNormals(vertexNormals);
        }
        else //if there is no filter, returns as theres no mesh
        {
            return;
        }


        Vector3 particleSpawnPos;
        Vector3 particleMoveDirection;

        int indexer = 0;
        GameObject currentObject;

        for (int i = 0; i < _particlePool.childCount; i++)
        {
            indexer = Random.Range(0, vertexPositions.Count);

            particleSpawnPos = _meshRenderer.transform.localToWorldMatrix.MultiplyPoint3x4(vertexPositions[indexer]);
            particleMoveDirection = Vector3.Normalize(vertexNormals[indexer]);

            currentObject = _particlePool.GetChild(i).gameObject;
            currentObject.SetActive(true);

            currentObject.transform.forward = Camera.main.transform.forward;
            currentObject.transform.position = particleSpawnPos;
            currentObject.transform.localScale = Vector3.one * particleScale;

            if(!currentObject.TryGetComponent(out Rigidbody rBody))
            {
                rBody = currentObject.AddComponent<Rigidbody>();
            }

            if(rBody)
            {
                rBody.velocity = Vector3.zero;
                rBody.AddForce(particleMoveDirection + (Vector3.up * _particleSpeed), ForceMode.Impulse);
            }
        }


        if(_particleCoroutine == null)
        {
            for (int i = 0; i < _playerMaterials.Length; i++)
            {
                _playerMaterials[i].color = _initialColors[i];
            }

            _particleCoroutine = StartCoroutine(VFXTimer(matColor));
        }

    }

    private IEnumerator VFXTimer(Color matColor)
    {
        Vector3 startingScale = Vector3.zero;

        StartCoroutine(FlashPlayerMat(matColor));

        if (_particlePool.childCount > 0)
        {
            startingScale = _particlePool.GetChild(0).transform.localScale;
        }


        for (float t = 0; t < _effectDuration; t += Time.deltaTime)
        {
            foreach (Transform child in _particlePool.transform)
            {
                child.transform.forward = Camera.main.transform.forward;
                child.transform.localScale = Vector3.Lerp(startingScale, Vector3.zero, t / _effectDuration);
            }
            yield return null;
        }

        EndDamageVFX();
        _particleCoroutine = null;
    }

    public void EndDamageVFX()
    {
        if(_particlePool)
        {
            for (int i = 0; i < _particlePool.childCount; i++)
            {
                _particlePool.GetChild(i).gameObject.SetActive(false);
            }
        }    
    }

    private void SetParticleSprites(Sprite sprite)
    {
        if(_particleRenderers != null)
        {
            for (int i = 0; i < _particleRenderers.Length; i++)
            {
                _particleRenderers[i].sprite = sprite;
            }
        }
    }

    private void SetUpParticles()
    {
        if (!_meshRenderer) return;


        _particlePool = new GameObject().transform;
        _particlePool.name = "Damage Particle Pool";
        _particlePool.parent = _meshRenderer.gameObject.transform;

        Transform particleObject;
        List<SpriteRenderer> spriteRenderers = new List<SpriteRenderer>();
        for (int i = 0; i < _particleQuantity; i++)
        {
            particleObject = new GameObject().transform;
            particleObject.name = "Damage Particle " + (i + 1).ToString();
            particleObject.transform.parent = _particlePool.transform;
            spriteRenderers.Add(particleObject.gameObject.AddComponent<SpriteRenderer>());
            particleObject.gameObject.SetActive(false);
        }

        _particleRenderers = spriteRenderers.ToArray();
    }

    private void GetPlayerMat()
    {
        if(_meshRenderer)
        {
            _playerMaterials = _meshRenderer.materials;
            _initialColors = new Color[_playerMaterials.Length];

            for (int i = 0; i < _initialColors.Length; i++)
            {
                _initialColors[i] = _playerMaterials[i].color;            }
        }
    }
    private IEnumerator FlashPlayerMat(Color flashColor)
    {
        bool color = false;
        for (float t = 0; t < _matFlashDuration; t += 0.15f)
        {
            color = !color;

            if(color)
            {
                for (int i = 0; i < _playerMaterials.Length; i++)
                {
                    _playerMaterials[i].color = flashColor;
                }
            }
            else
            {
                for (int i = 0; i < _playerMaterials.Length; i++)
                {
                    _playerMaterials[i].color = _initialColors[i];
                }
            }

            yield return new WaitForSeconds(0.15f);
        }

        for (int i = 0; i < _playerMaterials.Length; i++)
        {
            _playerMaterials[i].color = _initialColors[i];
        }
    }


    public static void AddPlayerEmissionVFX(GameObject objectToAddTo, Renderer objectRenderer, Sprite damageSprite, Sprite electrocutedSprite, Sprite healSprite)
    {
        PlayerEmissionVFX newEmissionVFX = objectToAddTo.AddComponent<PlayerEmissionVFX>();

        newEmissionVFX._damageSprite = damageSprite;
        newEmissionVFX._electrocutedSprite = electrocutedSprite;
        newEmissionVFX._healSprite = healSprite;

        newEmissionVFX._meshRenderer = objectRenderer;

        newEmissionVFX.GetPlayerMat();
        newEmissionVFX.SetUpParticles();
    }

}
