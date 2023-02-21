using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : Singleton<EventManager>
{
    public delegate void PlayerFall(Vector3 fallpos, float falltime);
    public static event PlayerFall OnPlayerFall;

    public delegate void PlayerDeath();
    public static event PlayerDeath OnPlayerDeath;


}
