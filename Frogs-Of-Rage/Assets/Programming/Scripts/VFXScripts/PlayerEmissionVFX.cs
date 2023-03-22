using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Rendering;

public class PlayerEmissionVFX : MonoBehaviour
{
    [SerializeField] private Renderer meshRenderer;

    [SerializeField] float _effectDuration;
    [SerializeField] float _particleQuantity;
    [SerializeField] float _particleSpeed;

    [Header("Damage Sprite")]
    [Space(5)]
    [SerializeField] private Sprite _damageSprite;
    [SerializeField] private float _damageSpriteScale;
    [Header("Electrocuted Sprite")]
    [Space(5)]
    [SerializeField] private Sprite _electrocutedSprite;
    [SerializeField] private float _electrocutedSpriteScale;
    [Header("Heal Sprite")]
    [Space(5)]
    [SerializeField] private Sprite _healSprite;
    [SerializeField] private float _healSpriteScale;
    [Header("Stun Sprite")]
    [Space(5)]
    [SerializeField] private Sprite _stunSprite;
    [SerializeField] private float _stunSpriteScale;

    private Transform _particlePool;
    private SpriteRenderer[] _particleRenderers;
    private Coroutine _particleCoroutine;

    public enum EmitPlayerParticlesMode
    {
        Damage,
        Electrocuted,
        Heal,
        Stun
    }

    private void Start()
    {
        SetUpParticles();
    }

    public void EmitPlayerParticles(EmitPlayerParticlesMode mode)
    {
        switch (mode)
        {
            case EmitPlayerParticlesMode.Damage:
                SpawnParticles(_damageSpriteScale, _damageSprite);
                break;
            case EmitPlayerParticlesMode.Electrocuted:
                SpawnParticles(_electrocutedSpriteScale, _electrocutedSprite);
                break;
            case EmitPlayerParticlesMode.Heal:
                SpawnParticles(_healSpriteScale, _healSprite);
                break;
            case EmitPlayerParticlesMode.Stun:
                SpawnParticles(_stunSpriteScale, _stunSprite);
                break;
            default:
                break;
        }
    }


    private void SpawnParticles(float particleScale, Sprite sprite)
    {
        if (!meshRenderer || !_particlePool || _particleCoroutine != null) return;

        Mesh objectMesh = null;
        List<Vector3> vertexPositions = new List<Vector3>();
        List<Vector3> vertexNormals = new List<Vector3>();

        SetParticleSprites(sprite);

        if(meshRenderer as SkinnedMeshRenderer) // gets skinned renderer if the render is that type so it can use a baked mesh for proper information
        {
            SkinnedMeshRenderer skinnedRenderer = meshRenderer as SkinnedMeshRenderer;

            skinnedRenderer.BakeMesh(objectMesh, true);
            objectMesh.GetVertices(vertexPositions);
            objectMesh.GetNormals(vertexNormals);
        }    
        else if (meshRenderer.gameObject.TryGetComponent(out MeshFilter mFilter)) //if its not a skinned mesh, finds the filter and used that mesh
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

            particleSpawnPos = meshRenderer.transform.localToWorldMatrix.MultiplyPoint3x4(vertexPositions[indexer]);
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
            _particleCoroutine = StartCoroutine(VFXTimer());
        }

    }

    private IEnumerator VFXTimer()
    {
        Vector3 startingScale = Vector3.zero;
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
        if (!meshRenderer) return;


        _particlePool = new GameObject().transform;
        _particlePool.name = "Damage Particle Pool";
        _particlePool.parent = meshRenderer.gameObject.transform;

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
        if (Input.GetKeyDown("4"))
        {
            EmitPlayerParticles(EmitPlayerParticlesMode.Stun);
        }
    }
}
