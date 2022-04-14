using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

using static HelperUtil;
using Random = UnityEngine.Random;

[RequireComponent(typeof(MeshRenderer))]
public class EnvTreadmill : MonoBehaviour
{
    [SerializeField]
    private EnvNodeGenerator generator = null;

    [SerializeField]
    private SO_TerrSettings settings = null;

    [SerializeField]
    private Transform terrNodesTransform = null;

    //TODO: find a way around not having to expose these transforms?

    /// <summary>
    /// The horizontal component of this treadmill
    /// which is a child of the treadmill transform
    /// </summary>
    [SerializeField]
    private Transform horizTransform = null;
    public Transform HorizTransform => horizTransform;

    /// <summary>
    /// the vertical component of this treadmill, which is
    /// a child of the horizontal transform
    /// </summary>
    [SerializeField]
    private Transform vertTransform = null;
    public Transform VertTransform => vertTransform;

    [SerializeField]
    private List<SO_TerrZoneWrapper> zoneWrappers;

    private SO_TerrZoneWrapper currZoneWrapper;
    private float cachedDefaultZoneSpeed;

    [SerializeField]
    private PSO_CurrentGameMode currGameMode = null;

    [SerializeField]
    private IntPropertySO currZone = null;

    [SerializeField]
    private DSO_LaneChange laneChangeDelegate = null;

    [SerializeField]
    private BoolPropertySO spawnOnlyFoleyPSO = null;

    [SerializeField]
    private DSO_TreadmillSpeedChange treadmillToggleDelegate = null;

    [SerializeField]
    private PSO_CurrentZonePhase currZonePhase = null;

    private Coroutine treadmillToggleCR = null;

    private Data2D<TerrAddon> generatedTerrAddons;

    private Data2D<float> renderedGroundPoints;

    private Data2D<float> renderedInterPoints;

    private Mesh mesh;
    Vector3[] meshVerticies = null;

    [SerializeField]
    private FloatPropertySO currTargetSpeed = null;
    private float currSpeed = 0;

    private float newRowThreshold;

    private LaneChange targetLaneChange;
    private float colShiftPerc;

    private float totalZoneDistTraveled = 0;
    private bool bossSpawned = false;

    private bool gamePaused => currGameMode.Value == GameModeEnum.PAUSE;

    //TODO: HACK for now to get the transforms to shift properly
    //for the horiz transform children without having to cache their initial
    //positions
    float prevHorizWrapPos = 0;

    private void Start()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        currZone.RegisterForPropertyChanged(OnZoneWrapperChange);
        laneChangeDelegate.RegisterForDelegateInvoked(OnLaneChange);
        treadmillToggleDelegate.RegisterForDelegateInvoked(OnTreadmillSpeedChange);

        InitData2D();

        transform.position = Vector3.zero;
        terrNodesTransform.localPosition = Vector3.zero;
        horizTransform.localPosition = Vector3.zero;
        vertTransform.localPosition = Vector3.zero;

        newRowThreshold = settings.TileDims.y;

        targetLaneChange = null;

    }

    private void OnZoneWrapperChange(int oldValue, int newValue)
    {
        currZoneWrapper = zoneWrappers.Find(x => x.Zone == newValue);
        cachedDefaultZoneSpeed = GetDefaultZoneSpeed();
        OnTreadmillSpeedChange(new TreadmillSpeedChange(1, 0));
    }


    private float GetDefaultZoneSpeed()
    {
        return -1 * currZoneWrapper.StartSpeed * settings.TileDims.y;
    }

    private int OnLaneChange(LaneChange lc)
    {
        //-1 because direction is the player movement direction,
        //environment shifts in opposite direction
        ShiftTerrainColumns(-1 * lc.Dir);

        //immediately offset the x delta of the grid
        float deltaX = lc.Dir * settings.TileDims.x;
        LocalPositionChange(terrNodesTransform, deltaX, 0, 0);
        //TODO move all children instead of transform going in crazy values space wise
        //LocalPositionChange(horizTransform, deltaX, 0, 0);

        //Start the treadmill horizontal tween to default x position
        colShiftPerc = 0;
        targetLaneChange = lc;
        prevHorizWrapPos = targetLaneChange.Dir * settings.TileDims.x;

        return 0;
    }

    private int OnTreadmillSpeedChange(TreadmillSpeedChange tsc)
    {
        if (treadmillToggleCR != null)
        {
            StopCoroutine(treadmillToggleCR);
        }

        currTargetSpeed.DirectlySetValue(cachedDefaultZoneSpeed * tsc.SpeedPerc);

        if (tsc.TransitionTime == 0)
        {
            currSpeed = currTargetSpeed.Value;
            return 0;
        }

        float deltaSpeed = currTargetSpeed.Value - currSpeed;
        float acceleration = deltaSpeed / tsc.TransitionTime;

        StartCoroutine(TreadmillSpeedChangeCoroutine(acceleration));
        return 0;
    }

    private IEnumerator TreadmillSpeedChangeCoroutine(float acceleration)
    {
        //< b/c normal speed is negative (negative z direction)
        bool speedingUp = currTargetSpeed.Value < currSpeed;

        //< and > respectively here because treadmill
        //normal speed is negative (negative z axis)
        bool tweening = true;
        while (tweening)//true if 
        {
            yield return null;
            currSpeed += acceleration * Time.deltaTime;
            tweening = speedingUp ?
                currTargetSpeed.Value < currSpeed :
                currTargetSpeed.Value > currSpeed;
        }

        currSpeed = currTargetSpeed.Value;
        treadmillToggleCR = null;
    }

    private void InitData2D()
    {
        OnZoneWrapperChange(0, currZone.Value);

        //since we also pass in the spawn method, this populates the initial grid for us
        //with the node generator!

        generatedTerrAddons = new Data2D<TerrAddon>(
            settings.TileCols, settings.TileRows,
            ResetAddonSpace,
            SpawnNewAddon,
            DestroyAddon,
            WrapAddon,
            ShiftVertAddon);

        float newFloat(int col) => 0f;
        renderedGroundPoints = new Data2D<float>(settings.PointCols, settings.PointRows, newFloat);
        renderedInterPoints = new Data2D<float>(settings.InterCols, settings.InterRows, newFloat);
        UpdateMeshRender();

        //Setup populated grid:

        spawnOnlyFoleyPSO.ModifyValue(true);

        for (int i = 0; i < settings.TileRows; i++)
        {
            if (i == settings.StartHazardFreeRowsFromPlayer)
            {
                spawnOnlyFoleyPSO.ModifyValue(false);
            }
            ShiftTerrainRowsDown();
        }
    }


    /// <summary>
    /// Need this to reset the new rows so that when we spawn the new rows
    /// they do not have any old data (so that the generator works properly
    /// by not comparing rules to old data locations)
    /// </summary>
    /// <param name="colIndex"></param>
    /// <returns></returns>
    private TerrAddon ResetAddonSpace(int colIndex)
    {
        return null;
    }

    /// <summary>
    /// Spawns a new TerrAddon at the given column
    /// </summary>
    private TerrAddon SpawnNewAddon(int colIndex)
    {
        //if the generator returned null, means it wasn't able to generate anything
        //new here due to rules or other restrictions
        TerrAddon taPrefab = generator.GetNewAddon(colIndex, 0, generatedTerrAddons);

        if (taPrefab == null)
        {
            return null;
        }


        TerrAddon taInstance = taPrefab.InstantiateAddon(terrNodesTransform);
        if (taInstance.RandomYTileOffset)
        {
            taInstance.transform.localPosition += new Vector3(0, 0, settings.TileDims.y * Random.Range(-0.5f, 0.5f));
        }

        //line up in grid local position
        taInstance.transform.localPosition += new Vector3
            ((colIndex + 0.5f) * settings.TileDims.x, 0, settings.TileRows * settings.TileDims.y);
        return taInstance;
    }

    //In the following addon call back methods,
    //we need to check for null because the Data2D element
    //may be null (empty space), or may be populated

    private void DestroyAddon(TerrAddon ta)
    {
        if (ta == null) { return; }
        SafeDestroy(ta.gameObject);
    }

    private void WrapAddon(TerrAddon ta, int newHorizPosDiff)
    {
        if (ta == null) { return; }
        ta.transform.position += new Vector3(newHorizPosDiff * settings.TileDims.x, 0, 0);
    }

    private void ShiftVertAddon(TerrAddon ta, int newVertPosDiff)
    {
        if (ta == null) { return; }
        ta.transform.localPosition += new Vector3(0, 0, -newVertPosDiff * settings.TileDims.y);
    }

    private void UpdateMeshRender()
    {
        meshVerticies = ConvertToMeshArray(renderedGroundPoints, settings.PointCols, settings.PointRows, settings.TileDims);
        mesh.vertices = meshVerticies;
        mesh.triangles = CreateMeshTriangles();
        mesh.RecalculateNormals();
    }

    //private void OnDrawGizmos()
    //{
    //    if (meshVerticies == null) { return; }
    //    foreach(Vector3 vertex in meshVerticies)
    //    {
    //        Gizmos.DrawSphere(vertex + transform.position, 0.5f);
    //    }
    //}


    /// <summary>
    /// Convert a 2D float dataa to a 1D Vector3 array
    /// </summary>
    /// <param name="data2D"></param>
    /// <param name="cols"></param>
    /// <param name="rows"></param>
    /// <param name="units"></param>
    /// <returns></returns>
    private Vector3[] ConvertToMeshArray(Data2D<float> data2D, int cols, int rows, Vector2 units)
    {
        Vector3[] meshData = new Vector3[cols * rows];

        int meshIndex = 0;

        for (int r = 0; r < settings.PointRows; r++)
        {
            for (int c = 0; c < settings.PointCols; c++)
            {
                //since Data2Ds have row index 0 at the top, and if we want the bottom row to be at unity
                //location Vector3(x, elevation, 0), we need to get the difference of rows and r

                //NOTE: in unity, selecting the mesh (which is attached to the envTreadmill node) has
                //the selection point in the middle of the mesh, but the actual transformation point of reference
                //will be zero zero if we do the follow.
                meshData[meshIndex++] = new Vector3(c * units.x, data2D.GetElement(c, r), (rows - r - 1) * units.y);
            }
        }

        return meshData;
    }

    private int[] CreateMeshTriangles()
    {
        int totalTileCount = settings.TileCols * settings.TileRows;
        int totalTriangleVertCount = totalTileCount * 6;
        int[] meshTriangles = new int[totalTriangleVertCount];

        for (int r = 0, t = 0; r < settings.TileRows; r++)
        {
            for (int c = 0; c < settings.TileCols; c++)
            {
                int topRowOffset = settings.PointCols * r;
                int bottomRowOffset = topRowOffset + settings.PointCols;

                meshTriangles[t++] = bottomRowOffset + c;
                meshTriangles[t++] = topRowOffset + c;
                meshTriangles[t++] = topRowOffset + c + 1;

                meshTriangles[t++] = topRowOffset + c + 1;
                meshTriangles[t++] = bottomRowOffset + c + 1;
                meshTriangles[t++] = bottomRowOffset + c;
            }
        }
        return meshTriangles;
    }

    public void AddTransformAsHorizChild(Transform newChild)
    {
        newChild.parent = horizTransform;
    }

    public void AddTransformAsVertChild(Transform newChild, bool destroyIfOutOfRange)
    {
        //TODO: would we need to implement destroyIfOutOfRange?
        newChild.parent = vertTransform;
    }

    private void Update()
    {
        if (gamePaused) { return; }
        TickTreadmillVertMove();
        TickTreadmillHorzWrap();
    }

    private void TickTreadmillVertMove()
    {
        float posVert = currSpeed * Time.deltaTime;

        totalZoneDistTraveled += Mathf.Abs(posVert);

        if (posVert + terrNodesTransform.localPosition.z <= -newRowThreshold)
        {
            posVert += settings.TileDims.y;

            ShiftTerrainRowsDown();
        }

        LocalPositionChange(terrNodesTransform, 0, 0, posVert);
        ForEachTransformChild(vertTransform, child =>
        {
            LocalPositionChange(child.transform, 0, 0, posVert);
        });

        if (bossSpawned) { return; }

        if (totalZoneDistTraveled >= currZoneWrapper.TileDistance2Boss * settings.TileDims.y)
        {
            SpawnZoneBoss();
        }
    }

    private void ShiftTerrainRowsDown()
    {
        generatedTerrAddons.ShiftRows(1);
        renderedGroundPoints.ShiftRows(1);
        renderedInterPoints.ShiftRows((1 + settings.InterCount));

        //TODO: uncomment if we decide to alter terrain mesh
        //UpdateMeshRender();

        //TODO: make sure we some how compensate for objects that are
        //multiple tiles long (whether we place the player the same
        //number of tiles ahead as the maximum terrAddon length or
        //do a custom per terrAddon check
    }

    private void ForEachTransformChild(Transform trans, Action<Transform> method)
    {
        for (int i = 0; i < trans.childCount; i++)
        {
            method(trans.GetChild(i));
        }
    }

    private void SpawnZoneBoss()
    {
        bossSpawned = true;
        currZonePhase.ModifyValue(ZonePhaseEnum.BOSS);
        currZoneWrapper.BossPrefab.InstantiateBoss(this);
    }


    private Vector3 GetLocalTileCenter(int colIndex, int rowIndex, int floorIndex)
    {
        float x = (colIndex + 0.5f) * settings.TileDims.x;
        float y = (settings.TileRows - rowIndex - 0.5f) * settings.TileDims.y;
        float elevation = floorIndex * settings.FloorHeight;
        return new Vector3(x, elevation, y);
    }


    private void ShiftTerrainColumns(int dir)
    {
        generatedTerrAddons.ShiftColsWrapped(dir);
        renderedGroundPoints.ShiftColsWrapped(dir);
        renderedInterPoints.ShiftColsWrapped(dir * (1 + settings.InterCount));

        //TODO: uncomment if we decide to alter terrain mesh
        //UpdateMeshRender();

        //TODO: make sure we compensate for objects wider than 1 tile such that we
        //have a wide enough environment (relative to the maximum width of a terrAddon)
        //that the player doesn't notice, or we
        //duplicate the terrAddon onto the other side
    }

    private void TickTreadmillHorzWrap()
    {
        if (targetLaneChange == null) { return; }

        colShiftPerc += Time.deltaTime / targetLaneChange.Time;

        float easedColShiftPerc = EasedPercent(colShiftPerc);
        float startXPos = targetLaneChange.Dir * settings.TileDims.x;
        float horizPos = Mathf.Lerp(startXPos, 0, easedColShiftPerc);

        LocalPositionChange(terrNodesTransform, horizPos - terrNodesTransform.localPosition.x, 0, 0);
        
        ForEachTransformChild(horizTransform, child =>
        {
            PositionChange(child, horizPos - prevHorizWrapPos, 0, 0);
        });

        ForEachTransformChild(vertTransform, child =>
        {
            PositionChange(child, horizPos - prevHorizWrapPos, 0, 0);
        });

        prevHorizWrapPos = horizPos;

        if (horizPos == 0)
        {
            targetLaneChange = null;
        }
    }
}
