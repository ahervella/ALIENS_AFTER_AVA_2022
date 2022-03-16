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
    private DSO_TreadmillSpeedChange treadmillToggleDelegate = null;

    private Coroutine treadmillToggleCR = null;

    //[SerializeField]
    //private PSO_CurrentTerrBossNode currTerrBossNode;

    private Data2D<TerrAddon> generatedTerrAddons;

    private Data2D<float> renderedGroundPoints;

    private Data2D<float> renderedInterPoints;

    //private int tileDistanceTraveled;

    private Mesh mesh;
    Vector3[] meshVerticies = null;

    [SerializeField]
    private FloatPropertySO currTargetSpeed = null;
    private float currSpeed = 0;

    private float newRowThreshold;

    private LaneChange targetLaneChange;
    private float colShiftPerc;
    

    private void Start()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        currZone.RegisterForPropertyChanged(OnZoneWrapperChange);
        laneChangeDelegate.SetInvokeMethod(OnLaneChange);
        treadmillToggleDelegate.SetInvokeMethod(OnTreadmillSpeedChange);

        InitData2D();

        transform.position = Vector3.zero;

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
        PositionChange(transform, deltaX, 0, 0);

        //Start the treadmill horizontal tween to default x position
        colShiftPerc = 0;
        targetLaneChange = lc;

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

        for(int i = 0; i < settings.TileRows; i++)
        {
            ShiftTerrainRowsDown();
        }

        //fully populate the grid with terr addons before beginning run
        //generatedTerrAddons.InitalizeAllValues();
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


        TerrAddon taInstance = taPrefab.InstantiateAddon(transform);
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
        Destroy(ta.gameObject);
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

    private void Update()
    {
        TickTreadmillVertMove();
        TickTreadmillHorzWrap();
    }

    private void TickTreadmillVertMove()
    {
        float posVert = currSpeed * Time.deltaTime;

        if (posVert + transform.position.z <= -newRowThreshold)
        {
            posVert += settings.TileDims.y;

            ShiftTerrainRowsDown();
        }

        PositionChange(transform, 0, 0, posVert);
    }


    private void ShiftTerrainRowsDown()
    {
        generatedTerrAddons.ShiftRows(1);
        renderedGroundPoints.ShiftRows(1);
        renderedInterPoints.ShiftRows((1 + settings.InterCount));
        UpdateMeshRender();

        //TODO: make sure we some how compensate for objects that are
        //multiple tiles long (whether we place the player the same
        //number of tiles ahead as the maximum terrAddon length or
        //do a custom per terrAddon check
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
        UpdateMeshRender();

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

        PositionChange(transform, horizPos - transform.position.x, 0, 0);

        if (horizPos == 0)
        {
            targetLaneChange = null;
        }
    }
}
