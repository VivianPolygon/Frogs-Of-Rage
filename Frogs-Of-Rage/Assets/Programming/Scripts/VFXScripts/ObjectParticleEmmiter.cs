using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class ObjectParticleEmmiter : MonoBehaviour 
{
    [SerializeField] protected ShadowCastingMode shadowMode;

    [SerializeField] protected int particleMax = 20;
    [SerializeField] protected bool hardCapParticles = true;

    [SerializeField] protected Mesh particleMesh;
    [SerializeField] protected Material particleMaterial;

    [SerializeField] protected Vector3 particleScale = Vector3.one;
    [SerializeField] protected Vector3 emmitterOffset = Vector3.zero;
    public Vector3 EmitterOffset
    {
        get { return emmitterOffset; }
    }
    private bool _particlesActive = true;

    private Transform _particlePool;
    protected Transform ParticlePool
    {
        get {return _particlePool; }
    }

    /// <summary>
    /// Initilizes the particle system from the settings
    /// </summary>
    protected void InitilizePool()
    {
        if(_particlePool != null)
        {
            Destroy(_particlePool);
            _particlePool = null;
        }

        _particlePool = new GameObject().transform;
        _particlePool.name = gameObject.name + " Particle Pool";
        _particlePool.transform.parent = gameObject.transform;
        _particlePool.transform.position = gameObject.transform.position + emmitterOffset;

        GameObject newParticle;
        for (int i = 0; i < particleMax; i++)
        {
            newParticle = new GameObject();

            newParticle.AddComponent<MeshFilter>().mesh = particleMesh;

            MeshRenderer newRenderer = newParticle.AddComponent<MeshRenderer>();
            newRenderer.material = particleMaterial;
            newRenderer.shadowCastingMode = shadowMode;

            newParticle.name = gameObject.name + " Particle " + i;
            newParticle.transform.parent = _particlePool.transform;

            ResetParticle(newParticle);
        }
    }

    /// <summary>
    /// Returns the first available particle object. returns null if none are available and system is hardcapped to the max. Also returns null if there is no pool. 
    /// </summary>
    /// <returns></returns>
    protected virtual GameObject GetParticleFromPool()
    {
        if (!_particlesActive)
            return null;

        if(_particlePool == null)
        {
            Debug.LogWarning("Could not spawn particle from gameobject: " + name + "because the pool wasne't initilized on " + this);
            return null;
        }

        for (int i = 0; i < _particlePool.childCount; i++)
        {
            if(!_particlePool.GetChild(i).gameObject.activeSelf)
            {
                ResetParticle(_particlePool.GetChild(i).gameObject);
                return _particlePool.GetChild(i).gameObject;
            }
        }
        //whole pool looped through
        if (hardCapParticles)
            return null;
        else
        {
            GameObject newParticle = new GameObject();

            newParticle.AddComponent<MeshFilter>().mesh = particleMesh;

            MeshRenderer newRenderer = newParticle.AddComponent<MeshRenderer>();
            newRenderer.material = particleMaterial;
            newRenderer.shadowCastingMode = shadowMode;

            newParticle.name = gameObject.name + " Particle " + _particlePool.childCount;
            newParticle.transform.parent = _particlePool.transform;

            ResetParticle(newParticle);
            return newParticle;
        }
    }

    protected void ResetParticle(GameObject particleObject)
    {
        if (particleObject == null || ParticlePool == null)
            return;

        particleObject.transform.localScale = particleScale;
        particleObject.transform.position = _particlePool.transform.position;
        particleObject.SetActive(false);
    }

    /// <summary>
    /// Activates or disactivates the particle system. 
    /// </summary>
    public void ToggleParticles(bool enabled)
    {
        _particlesActive = enabled;
    }

    protected virtual void Awake()
    {
        InitilizePool();
    }
}
