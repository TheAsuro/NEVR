using System.Collections;
using UnityEngine;
using Api;

public class Test : MonoBehaviour
{
    bool done = false;

    void Start()
    {
        EveApi.PlayerKills.Update();
    }

    void Update()
    {
        if (Time.time > 1f && !done)
        {
            DoStuff();
            done = true;
        }
    }

    void DoStuff()
    {
        foreach (PlayerKill kill in EveApi.PlayerKills.LastHourKills)
        {
            GetComponent<GalaxyRenderer>().AddNameField(kill.characterName, kill.systemID);
        }
    }
}
