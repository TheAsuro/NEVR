using UnityEngine;
using System.Collections.Generic;
using Api;

public class GalaxyRenderer : MonoBehaviour
{
    public int lineParticleCount = 3;
    public int clusterSize = 1000;
    public float scale = 1e-16f;
    public float particleSize = 1f;
    public GameObject systemClusterPrefab;
    public GameObject connectionClusterPrefab;

    private Galaxy galaxy;

    void Start()
    {
        galaxy = new Galaxy();
        GameObject currentObj = Instantiate(systemClusterPrefab);
        currentObj.transform.parent = transform;
        int counter = 0;

        foreach (SolarSystem system in galaxy.Systems.Values)
        {
            if (counter <= 0)
            {
                counter = clusterSize;
                currentObj = Instantiate(systemClusterPrefab);
                currentObj.transform.parent = transform;
            }

            SpawnSystem(system, currentObj.GetComponent<ParticleSystem>());
            counter--;
        }

        counter = 0;

        foreach (JumpConnection connection in galaxy.Jumps)
        {
            if (counter <= 0)
            {
                counter = clusterSize;
                currentObj = Instantiate(connectionClusterPrefab);
                currentObj.transform.parent = transform;
            }

            SpawnConnection(connection, currentObj.GetComponent<ParticleSystem>());
            counter--;
        }
	}

    private void SpawnSystem(SolarSystem system, ParticleSystem PS)
    {
        PS.Emit(system.Position * scale + transform.position, Vector3.zero, particleSize, float.MaxValue, SecurityColor(system.Security));
    }

    private void SpawnConnection(JumpConnection connection, ParticleSystem PS)
    {
        Vector3 startPos = galaxy.Systems[connection.StartID].Position;
        Vector3 endPos = galaxy.Systems[connection.EndID].Position;

        for (int i = 0; i < lineParticleCount; i++)
            PS.Emit(Vector3.Lerp(startPos * scale + transform.position, endPos * scale + transform.position, (float)((float)(i + 1f) / (float)(lineParticleCount + 1f))), Vector3.zero, particleSize * 0.33f, float.MaxValue, Color.white);
    }

    private Color SecurityColor(decimal security)
    {
        if (security <= 0M)
            return Color.red;

        return Color.Lerp(Color.green, Color.red, (float)security);
    }
}
