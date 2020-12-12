using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public abstract class AAudioWrapper : ScriptableObject
{

    abstract public void PlayAudioWrappers(GameObject soundObject);

    // Adds an offset to the wrapper's volume level
    abstract public void AddOffset(float offsetDb);

    // Resets the wrapper's volume level (to be used after playing all included wrappers or clips)
    //This prevents offsets from compounding
    protected abstract void ResetLevelOffset();

    protected void Initialize()
    {
        ResetLevelOffset();
    }
}