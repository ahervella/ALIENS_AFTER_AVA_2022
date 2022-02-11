using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

using static HelperUtil;

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

    [SerializeField]
    private PSO_CurrentGameMode currGameMode = null;

    [SerializeField]
    private IntPropertySO currZone = null;


    [SerializeField]
    private PSO_LaneChange laneChange = null;

    //[SerializeField]
    //private PSO_CurrentTerrBossNode currTerrBossNode;

    private Data2D<TerrAddonFloorWrapper> generatedTerrAddons;

    private Data2D<float> renderedGroundPoints;

    private Data2D<float> renderedInterPoints;

    private int tileDistanceTraveled;

    private Mesh mesh;
    Vector3[] meshVerticies = null;

    private float currSpeed;

    private float newRowThreshold;

    private LaneChange targetLaneChange;
    private float colShiftPerc;

    private bool data2DsInitialized = false;
    

    private void Start()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        currZone.RegisterForPropertyChanged(OnZoneWrapperChange);
        laneChange.RegisterForPropertyChanged(OnLaneChange);

        InitData2D();

        transform.position = Vector3.zero;

        currSpeed = -1 * currZoneWrapper.StartSpeed * settings.TileDims.y;

        newRowThreshold = settings.TileDims.y;

        targetLaneChange = null;

    }

    private void OnZoneWrapperChange(int oldValue, int newValue)
    {
        currZoneWrapper = zoneWrappers.Find(x => x.Zone == newValue);
    }


    private void InitData2D()
    {
        data2DsInitialized = false;

        OnZoneWrapperChange(0, currZone.Value);

        //since we also pass in the spawn method, this populates the initial grid for us
        //with the node generator!

        generatedTerrAddons = new Data2D<TerrAddonFloorWrapper>(
            settings.TileCols, settings.TileRows,
            SpawnNewAddonFloorWrapper,
            DestroyAddonFloorWrapper,
            WrapAddonFloorWrapper,
            ShiftVertAddonFloorWrapper);

        Func<int, float> newFloat = col => 0f;
        renderedGroundPoints = new Data2D<float>(settings.PointCols, settings.PointRows, newFloat, null, null, null);
        renderedInterPoints = new Data2D<float>(settings.InterCols, settings.InterRows, newFloat, null, null, null);
        UpdateMeshRender();

        data2DsInitialized = true;
    }



    //TODO: ALSO ADD A CALL BACK FOR NEW ROWS TO MOVE TRANSFORM


    /// <summary>
    /// Spawns a new TerrAddonFloorWrapper at the given column
    /// </summary>
    private TerrAddonFloorWrapper SpawnNewAddonFloorWrapper(int colIndex)
    {
        if (!data2DsInitialized) { return null; }

        //if the generator returned null, means it wasn't able to generate anything
        //new here due to rules or other restrictions
        TerrAddonFloorWrapper tafw = generator.GetNewAddonFloorWrapper(colIndex, 0, generatedTerrAddons);

        if (tafw == null)
        {
            return null;
        }

        tafw.InstantiateFromPrefab(transform);

        //line up in grid local position
        tafw.AddonInst.transform.localPosition =
            new Vector3(colIndex * settings.TileDims.x, settings.FloorHeight * tafw.FloorIndex, settings.TileRows * settings.TileDims.y);
        return tafw;
    }

    private void DestroyAddonFloorWrapper(TerrAddonFloorWrapper tafw)
    {
        if (tafw == null)
        {
            return;
        }

        tafw.DestroyInstance();
    }

    private void WrapAddonFloorWrapper(TerrAddonFloorWrapper tafw, int newHorizPosDiff)
    {
        if (tafw == null) { return; }
        tafw.AddonInst.transform.position += new Vector3(newHorizPosDiff * settings.TileDims.x, 0, 0);
    }

    private void ShiftVertAddonFloorWrapper(TerrAddonFloorWrapper tafw, int newVertPosDiff)
    {
        if (tafw == null) { return; }
        if(tafw.AddonInst == null)
        {
            return;
        }
        tafw.AddonInst.transform.localPosition += new Vector3(0, 0, -newVertPosDiff * settings.TileDims.y);
    }

    private void UpdateMeshRender()
    {
        //renderedGroundPoints.PrintData("renderedGroundPoints");

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


    private void OnLaneChange(LaneChange prevLC, LaneChange newLC)
    {
        //-1 because direction is the player movement direction,
        //environment shifts in opposite direction
        ShiftTerrainColumns(-1 * newLC.Dir);

        //immediately offset the x delta of the grid
        float deltaX = newLC.Dir * settings.TileDims.x;
        PositionChange(transform, deltaX, 0, 0);

        //Start the treadmill horizontal tween to default x position
        colShiftPerc = 0;
        targetLaneChange = newLC;
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
