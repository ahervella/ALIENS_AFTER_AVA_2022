using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunnerDevTools : MonoBehaviour
{
    [SerializeField]
    private bool enableDevTools = false, enableInvincibility = false;

    private RunnerPlayer player;

    private void Awake()
    {
        player = GetComponent<RunnerPlayer>();
        if (enableInvincibility)
        {
            player.IsInvincible = true;
        }
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
        if (Input.GetKeyDown(KeyCode.C))
        {
            player.GunBullets = RunnerPlayer.AMO_SIZE;
        }
    }
}
