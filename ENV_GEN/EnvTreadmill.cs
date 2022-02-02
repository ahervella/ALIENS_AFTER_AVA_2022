﻿using System.Collections;
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

    private Data2D<TerrAddon[]> generatedTerrAddons;

    private Data2D<float> renderedGroundPoints;

    private Data2D<float> renderedInterPoints;

    private int tileDistanceTraveled;

    private Mesh mesh;
    Vector3[] meshVerticies = null;

    private float currSpeed;

    private float newRowThreshold;

    private LaneChange targetLaneChange;
    private float colShiftPerc;

    private void Start()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        currZone.RegisterForPropertyChanged(OnZoneWrapperChange);

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
        OnZoneWrapperChange(0, currZone.Value);

        //since we also pass in the spawn method, this populates the initial grid for us
        //with the node generator!
        generatedTerrAddons = new Data2D<TerrAddon[]>(settings.TileCols, settings.TileRows, SpawnNewAddonFloorGroup, DestroyTerrAddonFloorGroup);

        Func<int, float> newFloat = col => 0f;
        renderedGroundPoints = new Data2D<float>(settings.PointCols, settings.PointRows, newFloat, null);
        renderedInterPoints = new Data2D<float>(settings.InterCols, settings.InterRows, newFloat, null);
        UpdateMeshRender();
    }

    

    private void DestroyTerrAddonFloorGroup(TerrAddon[] addons)
    {
        foreach(TerrAddon addon in addons)
        {
            if (addon != null)
            {
                Destroy(addon);
            }
        }
    }

    private void UpdateMeshRender()
    {
        renderedGroundPoints.PrintData("renderedGroundPoints");

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

    private void FixedUpdate()
    {
        TickTreadmillVertMove();
        TickTreadmillHorzWrap();
    }

    private void TickTreadmillVertMove()
    {
        float posVert = currSpeed * Time.fixedDeltaTime;

        if (posVert + transform.position.z <= -newRowThreshold)
        {
            posVert += settings.TileDims.y;

            ShiftTerrainRowsDown();
        }

        PositionChange(transform, 0, 0, posVert);
    }


    private void ShiftTerrainRowsDown()
    {
        generatedTerrAddons.ShiftRows(-1);
        renderedGroundPoints.ShiftRows(-1);
        renderedInterPoints.ShiftRows(-(1 + settings.InterCount));
        UpdateMeshRender();

        //TODO: make sure we some how compensate for objects that are
        //multiple tiles long (whether we place the player the same
        //number of tiles ahead as the maximum terrAddon length or
        //do a custom per terrAddon check
    }


    /// <summary>
    /// Spawns a new TerrAddons floor group at the given column
    /// </summary>
    private TerrAddon[] SpawnNewAddonFloorGroup(int colIndex)
    {
        TerrAddon addonPrefab = generator.GetNewAddon(colIndex, 0, generatedTerrAddons);

        //if the generator returned null, means it wasn't able to generate anything
        //new here due to rules or other restrictions
        if (addonPrefab == null)
        {
        return InitEmptyAddonFloorGroup(); ;
        }

        TerrAddon[] addonFloorGroup = generatedTerrAddons.GetElement(colIndex, 0);

        //If the first floor was occupied and the generator still gave us back
        //a newAddon, then that means its for the first empty floor
        for (int f = 0; f < settings.FloorCount; f++)
        {
            if (addonFloorGroup[f] == null)
            {
                TerrAddon instance = Instantiate(addonPrefab, transform);
                Vector3 spawnLocalPos = GetLocalTileCenter(colIndex, 0, f);
                instance.transform.localPosition = spawnLocalPos;

                addonFloorGroup[f] = instance;

                break;
            }
        }

        return addonFloorGroup;
    }

    private TerrAddon[] InitEmptyAddonFloorGroup()
    {
        TerrAddon[] addons = new TerrAddon[settings.FloorCount];
        for (int i = 0; i < settings.FloorCount; i++)
        {
            addons[i] = null;
        }
        return addons;
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
        ShiftTerrainColumns(newLC.Dir);
        float deltaX = newLC.Dir * settings.TileDims.x;
        PositionChange(transform, deltaX, 0, 0);
        targetLaneChange = newLC;
        colShiftPerc = 0;
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

        colShiftPerc += Time.fixedDeltaTime / targetLaneChange.Time;

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
