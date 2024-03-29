﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AAlienBossBase : MonoBehaviour
{
    public abstract AAlienBossBase InstantiateBoss(EnvTreadmill treadmill);

    public abstract void TakeDamage(int damage);

    public abstract BoxColliderSP HitBox();

    public abstract void Stun();
}
