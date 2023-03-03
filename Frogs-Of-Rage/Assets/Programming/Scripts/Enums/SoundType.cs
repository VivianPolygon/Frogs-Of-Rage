using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SoundType
{
    LowPrioritySoundEffect,     //has a pooling system on the audio manager. 
    HighPrioritySoundEffect,    //plays no matter what, for key things like scripted events. 
    PlayerSoundEffect,          //has its own pooling system, for sounds the player makes. 
    Music                      //can only ever have one thing assigned, is the overall level music. other music will be considered sound effects
}
