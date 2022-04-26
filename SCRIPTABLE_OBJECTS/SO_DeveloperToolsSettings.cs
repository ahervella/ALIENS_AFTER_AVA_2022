using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SO_DeveloperToolsSettings", menuName = "ScriptableObjects/StaticData/SO_DeveloperToolsSettings")]
public class SO_DeveloperToolsSettings : ScriptableObject
{
    [SerializeField]
    private bool invincibility = false;
    public bool Invincibility => invincibility;

    [SerializeField]
    private bool instantMainMenuIntro = false;
    public bool InstantMainMenuIntro => instantMainMenuIntro;

    [SerializeField]
    private bool skipDeathSequence = false;
    public bool SkipDeathSequence => skipDeathSequence;

    [SerializeField]
    private bool startWith1Life = false;
    public bool StartWith1Life => startWith1Life;

    [SerializeField]
    private bool spawnOnlyFoley = false;
    public bool SpawnOnlyFoley => spawnOnlyFoley;

    [SerializeField]
    private bool spawnNoTerrAddons = false;
    public bool SpawnNoTerrAddons => spawnNoTerrAddons;

    [SerializeField]
    private bool spawnBossOnStart = false;
    public bool SpawnBossOnStart => spawnBossOnStart;

    [SerializeField]
    private bool demoMode = false;
    public bool DemoMode => demoMode;

    [SerializeField]
    private Loadout firstLoadout = null;
    public Loadout FirstLoadout => firstLoadout;

    //TODO: mute just music, infinite energy bar, no delay for changing moves, no timer delays, etc.
}
