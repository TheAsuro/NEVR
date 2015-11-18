using System.Collections.Generic;
using Api;

public class Galaxy
{
    public Dictionary<int, SolarSystem> Systems { get; private set; }
    public List<JumpConnection> Jumps { get; private set; }

    public Galaxy()
    {
        MapData data = new MapData();
        Systems = new Dictionary<int, SolarSystem>();
        Jumps = new List<JumpConnection>();
        
        foreach (SolarSystem system in data.SystemData)
        {
            Systems.Add(system.ID, system);
        }

        foreach (JumpConnection jump in data.JumpData)
        {
            Jumps.Add(jump);
        }
    }
}
