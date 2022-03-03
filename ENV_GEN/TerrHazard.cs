using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class TerrHazard : TerrAddon
{
    [SerializeField]
    private PlayerActionEnum requiredAvoidAction = PlayerActionEnum.NONE;

    private void OnTriggerEnter(Collider other)
    {
        PlayerRunner player = other.gameObject.GetComponent<PlayerRunner>();
        if (player != null)
        {
            player.OnEnterHazard(requiredAvoidAction, PlayerActionEnum.NONE, TerrAddonEnum);
            return;
        }

        Projectile projectile = other.gameObject.GetComponent<Projectile>();
        if (projectile != null)
        {
            projectile.OnEnteredHazard(this);
        }
    }
}
