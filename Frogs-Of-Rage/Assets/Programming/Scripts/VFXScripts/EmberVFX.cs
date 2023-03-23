using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmberVFX : ObjectParticleEmmiter
{
    [SerializeField] private float _emmisionForce = 12f;
    [SerializeField] private float _particleDuration = 0.75f;

    [SerializeField] [Range(0f, 5f)] private float _emberHorizontalSpread = 0.5f;

    [SerializeField] private Vector2 _particleSpawnTimeMinMax = Vector2.one;

    private Rigidbody rBody;

    protected override void Awake()
    {
        base.Awake();
        StartCoroutine(SpawnParticles(_particleSpawnTimeMinMax));
    }

    private void EmitParticle(GameObject particle)
    {
        if (!particle)
            return;

        particle.SetActive(true);

        particle.TryGetComponent(out rBody);
        if(!rBody)
            rBody = particle.AddComponent<Rigidbody>();

        rBody.velocity = Vector3.zero;

        Vector3 RandomDirection = particle.transform.position;
        RandomDirection.y += 1;
        RandomDirection.x += Random.Range(-1f, 1f) * _emberHorizontalSpread;
        RandomDirection.z += Random.Range(-1f, 1f) * _emberHorizontalSpread;
        RandomDirection = Vector3.Normalize(RandomDirection - particle.transform.position);

        rBody.AddForce((RandomDirection) * _emmisionForce, ForceMode.Impulse);
    }

    private void Emit()
    {
        GameObject particle = GetParticleFromPool();
        if(particle)
        {
            EmitParticle(particle);
            StartCoroutine(ParticleDespawnDelay(_particleDuration, particle));
        }
    }

    private IEnumerator ParticleDespawnDelay(float despawnDelay, GameObject particle)
    {
        float timePercent;
        for (float t = 0; t < despawnDelay; t += Time.deltaTime)
        {
            timePercent = t / despawnDelay;
            particle.transform.localScale = Vector3.Lerp(particleScale, Vector3.zero, timePercent);

            yield return null;
        }

        particle.SetActive(false);
    }

    private IEnumerator SpawnParticles(Vector2 spawnTimes)
    {
        if(spawnTimes.y <= 0)
        {
            spawnTimes.y = 0.1f;
        }
        WaitForSeconds delay;

        while(true)
        {
            delay = new WaitForSeconds(Random.Range(spawnTimes.x, spawnTimes.y));
            Emit();

            yield return delay;
        }
    }

}
