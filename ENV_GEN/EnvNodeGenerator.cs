using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static HelperUtil;

public class EnvNodeGenerator : MonoBehaviour
{
    [SerializeField]
    private SO_TerrSettings settings = null;

    [SerializeField]
    private SO_DeveloperToolsSettings devTools = null;

    [SerializeField]
    private IntPropertySO currZone = null;

    [SerializeField]
    private PSO_CurrentZonePhase currZonePhase = null;

    [SerializeField]
    private DSO_TerrainChange terrainChangeDelegate = null;

    [SerializeField]
    private BoolPropertySO spawnOnlyFoleyPSO = null;

    [SerializeField]
    private SO_TerrZoneWrapperSettings zoneWrapperSettings = null;

    private SO_TerrZoneWrapper cachedZoneWrapper;

    private TerrainChangeWrapper queuedTerrainChangeWrapper = null;
    private float terrainChangeDistTraveled;

    private void Awake()
    {
        currZone.RegisterForPropertyChanged(OnZoneChange);
        currZonePhase.RegisterForPropertyChanged(OnZonePhaseChange);
    }

    private void Start()
    {
        if (devTools.SpawnBossOnStart)
        {
            currZonePhase.ModifyValueNoInvoke(ZonePhaseEnum.BOSS_SPAWN);
            OnZoneChange(currZone.Value, currZone.Value);
            currZonePhase.ModifyValue(ZonePhaseEnum.BOSS_SPAWN);

            terrainChangeDelegate.InvokeDelegateMethod(
                queuedTerrainChangeWrapper);

            queuedTerrainChangeWrapper = null;
        }
    }

    private void OnZoneChange(int prevZone, int newZone)
    {
        QueueTerrainChangeDelegate(new TerrainChangeWrapper(true));
        cachedZoneWrapper = zoneWrapperSettings.GetZoneWrapper(newZone);
        cachedZoneWrapper.InitAndCacheTerrAddonData();
    }

    private void OnZonePhaseChange(ZonePhaseEnum oldPhase, ZonePhaseEnum newPhase)
    {
        QueueTerrainChangeDelegate(new TerrainChangeWrapper(false));
        cachedZoneWrapper.InitAndCacheTerrAddonData();
    }

    private void QueueTerrainChangeDelegate(TerrainChangeWrapper tcw)
    {
        queuedTerrainChangeWrapper = tcw;
        terrainChangeDistTraveled = 0;
    }

    public void TerrainChangeDelegateTick(float posVertDelta)
    {
        if (queuedTerrainChangeWrapper == null) { return; }
        if (terrainChangeDistTraveled < settings.TileRows * settings.TileDims.y)
        {
            terrainChangeDistTraveled += -posVertDelta;
            return;
        }

        terrainChangeDelegate.InvokeDelegateMethod(queuedTerrainChangeWrapper);
        queuedTerrainChangeWrapper = null;
    }

    public void TriggerEarlyBossSpawnTerrain()
    {
        cachedZoneWrapper.InitAndCacheTerrAddonData(ZonePhaseEnum.BOSS_SPAWN);
    }

    public TerrAddon GetNewAddon(int colIndex, int rowIndex, Data2D<TerrAddon> currAddons)
    {
        if (cachedZoneWrapper == null)
        {
            OnZoneChange(-1, currZone.Value);
        }

        return TryGetNewViolationFreeAddon(colIndex, rowIndex, currAddons);
    }

    private TerrAddon TryGetNewViolationFreeAddon(int colIndex, int rowIndex, Data2D<TerrAddon> currAddons)
    {
        if (devTools.SpawnNoTerrAddons) { return null; }

        TerrAddon newAddon = cachedZoneWrapper.GenerateRandomNewAddon(spawnOnlyFoleyPSO.Value || devTools.SpawnOnlyFoley);
        if (newAddon == null) { return null; }

        //have to account for wrapping effect, such that there may be violations in wrapped space
        int wrappedColIndex = colIndex > currAddons.Cols / 2 ? colIndex - currAddons.Cols : colIndex + currAddons.Cols;

        Vector2Int dist2Center;
        Vector2Int wrappedDist2Center;

        for (int x = 0; x < currAddons.Cols; x++)
        {
            for (int y = 0; y < currAddons.Rows; y++)
            {
                TerrAddon sourceAddon = currAddons.GetElement(x, y);
                if (sourceAddon == null) { continue; }

                dist2Center = new Vector2Int(colIndex - x, rowIndex - y);
                wrappedDist2Center = new Vector2Int(wrappedColIndex - x, rowIndex - y);

                if (!FreeOfViolations(sourceAddon, newAddon, dist2Center, wrappedDist2Center))
                {
                    return null;
                }
            }
        }

        return newAddon;
    }



    private bool FreeOfViolations(TerrAddon source, TerrAddon other,
        Vector2Int relativePos2Source, Vector2Int wrappedRelativePos2Source)
    {
        if (source == null || other == null) { return true; }

        //TODO: just make the cached stuff a getter to bring all the functionality into TerrAddon
        return !source.IsViolation(other, relativePos2Source)
            && !other.IsViolation(source, -relativePos2Source)
            && !source.IsViolation(other, wrappedRelativePos2Source)
            && !other.IsViolation(source, -wrappedRelativePos2Source);
    }
}
