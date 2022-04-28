using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioWrapperSource))]
public class PlayerVox : MonoBehaviour
{
    [SerializeField]
    private SO_PlayerVoxSettings settings = null;

    private AudioWrapperSource audioSource = null;

    private void Awake()
    {
        audioSource = GetComponent<AudioWrapperSource>();
    }

    private void TryPlayVox(PlayerVoxEnum voxType)
    {
        AAudioWrapperV2 wrapper = settings.TryGetPlayerVox(voxType);
        if (wrapper != null)
        {
            wrapper.PlayAudioWrapper(audioSource);
        }
    }

    public void AE_Vox_RunBreath()
    {
        TryPlayVox(PlayerVoxEnum.RUN_BREATH);
    }

    public void AE_Vox_ScaredYell()
    {
        TryPlayVox(PlayerVoxEnum.SCARED_YELL);
    }

    public void AE_Vox_AngryYell()
    {
        TryPlayVox(PlayerVoxEnum.ANGRY_YELL);
    }

    public void AE_Vox_Hurt()
    {
        TryPlayVox(PlayerVoxEnum.HURT);
    }

    public void AE_Vox_Struggle()
    {
        TryPlayVox(PlayerVoxEnum.STRUGGLE);
    }
}
