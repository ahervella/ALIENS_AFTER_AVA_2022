using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunnerDevTools : MonoBehaviour
{
    [SerializeField]
    private bool enableDevTools = false, enableInvincibility = false;

    [SerializeField]
    private BoolPropertySO invincibleSO = null;

    private RunnerPlayer player;

    private void Awake()
    {
        invincibleSO.ModifyValue(enableInvincibility);
    }

    void Update()
    {
        if (!enableDevTools)
        {
            return;
        }
        if (Input.GetKeyDown(KeyCode.X))
        {
            player.HasRock = true;
        }
        if (Input.GetKeyDown(KeyCode.Z))
        {
            RunnerPlayer.GunBullets = RunnerPlayer.AMO_SIZE;
        }
    }
}
