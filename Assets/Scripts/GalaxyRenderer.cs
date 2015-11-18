using UnityEngine;
using System.Collections.Generic;
using Api;

public class GalaxyRenderer : MonoBehaviour
{
    public int lineParticleCount = 3;
    public float scale;

    private Galaxy galaxy;
    private ParticleSystem PS { get { return GetComponent<ParticleSystem>(); } }

    void Start()
    {
        galaxy = new Galaxy();
        foreach (SolarSystem system in galaxy.Systems.Values)
        {
            SpawnSystem(system);
        }

        foreach (JumpConnection connection in galaxy.Jumps)
        {
            SpawnConnection(connection);
        }
	}

    private void SpawnSystem(SolarSystem system)
    {
        PS.Emit(system.Position * scale, Vector3.zero, 1f, float.MaxValue, SecurityColor(system.Security));
    }

    private void SpawnConnection(JumpConnection connection)
    {
        Vector3 startPos = galaxy.Systems[connection.StartID].Position;
        Vector3 endPos = galaxy.Systems[connection.EndID].Position;

        for (int i = 0; i < lineParticleCount; i++)
            PS.Emit(Vector3.Lerp(startPos * scale, endPos * scale, (float)((float)(i + 1f) / (float)(lineParticleCount + 1f))), Vector3.zero, 0.3f, float.MaxValue, Color.white);
    }

    private Color SecurityColor(decimal security)
    {
        if (security <= 0M)
            return Color.red;

        return Color.Lerp(Color.green, Color.red, (float)security);
    }
}
