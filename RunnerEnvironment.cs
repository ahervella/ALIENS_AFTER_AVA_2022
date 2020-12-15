using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//requires mesh
[RequireComponent(typeof(Mesh))]
public class RunnerEnvironment : MonoBehaviour
{
    private Mesh mesh;
    private static bool V2_RIPCORD = false;

    [SerializeField]
    private bool drawShapes;
    [SerializeField]
    private bool drawPoints;
    [SerializeField]
    private bool drawOnlyControlPoints;
    [SerializeField]
    private bool hideTerrObjs;
    [SerializeField]
    private bool doNotGenerateShit;

    //control point height and width
    [SerializeField]
    private int height;
    [SerializeField]
    private int width;

    private int rvHeight;
    private int rvWidth;
    //number of points to fill between control points (can be zero too)
    [SerializeField]
    private int fillPointCount;

    [SerializeField]
    private int topMargin;
    [SerializeField]
    private int bottomMargin;
    [SerializeField]
    private int leftMargin;
    [SerializeField]
    private int rightMargin;

    [SerializeField]
    private float treadmillSpeed;
    private float startingTreadmillSpeed;

    [SerializeField]
    private float treadmillAccel;
    private float TMSlowDownTime = 0;
    private float currTMSpeed;
    private float targetTMSpeed;
    private float TMTimeTotal = 0;

    [SerializeField]
    private float horizontalWrapSpeed;

    private int[] triangleRenIndices;


    //actual render height and width with repsect to a unit
    //of 1 fill shape length or width
    private int rendHeight;
    private int rendWidth;

    //length between two control points (fill points will adjust to this)
    [SerializeField]
    private float heightUnit;
    [SerializeField]
    private float widthUnit;


    private bool canGenerateOthers = true;

    private int changeLaneDir = 0;
    [SerializeField]
    private float changeLaneTime = 1.5f;
    private float currChangeLaneTime = 0f;
    private float xLaneChangePositionProgress = 0f;


    [SerializeField]
    private float distBehindPlayer2ZeroAlpha;
    //[SerializeField]
    private Vector3[] vertices;
    //[SerializeField]
    private Vector3[] rendVertices;

    private List<List<Vector3>> verticesV2 = new List<List<Vector3>>();
    private List<List<Vector3>> rendVerticesV2 = new List<List<Vector3>>();

    [SerializeField]
    private TerrTile[] terrains;
    //TODO: make these read onlys
    [SerializeField]
    private List<TerrObject> terrObjects;
    [SerializeField]
    private List<TerrObject> attachmentTerrObjects = new List<TerrObject>();
    private List<TerrObject> attachmentTerrObjs = new List<TerrObject>();
    List<RunnerThrowable> throwables = new List<RunnerThrowable>();

    [SerializeField]
    private RunnerPlayer player;
    private int playerLane;


    //list for value because could generate more than one instance before control point
    //index applies treadmill effect
    private Dictionary<int, List<TerrObject>> currTerrObjsDict = new Dictionary<int, List<TerrObject>>();
    private Dictionary<int, Dictionary<int, List<TerrObject>>> currTerrObjsDictV2 = new Dictionary<int, Dictionary<int, List<TerrObject>>>();

    private void Start()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        rendHeight = height * (1 + fillPointCount);
        rendWidth = width * (1 + fillPointCount);

        if (!V2_RIPCORD)
        {
            //don't think this is necessary but leave in the old for now
            for (int i = 0; i < terrains.Length; i++)
            {
                terrains[i] = Instantiate(terrains[i]);
            }
        }

        

        player.transform.position = new Vector3((width / 2) * widthUnit, player.hitBox.size.y / 2f * player.transform.localScale.y, player.transform.position.z);//bottomMargin * heightUnit);
        playerLane = width / 2;


        currTMSpeed = treadmillSpeed;
        targetTMSpeed = treadmillSpeed;
        startingTreadmillSpeed = treadmillSpeed;

        if (V2_RIPCORD)
        {
            CreateControlVerticesV2();
            RefreshRenderedPointsV2();
        }
        else
        {
            CreateControlVertices();
            RefreshRenderedPoints();
        }
        
        
        RunnerControls.OnInputAction += InputUpdate;
        RunnerPlayer.changeTreamillSpeed += ChangeTMSpeed;
        RunnerThrowable.throwableGenerated += AddThrowable;
        RunnerThrowable.throwableDestroyed += RemoveThrowable;
        TerrObject.terrObjDestroyed += OnDestroyedTerrObj;

        attachmentTerrObjs.AddRange(attachmentTerrObjects);
    }

    private void CreateControlVerticesV2()
    {
        verticesV2.Clear();

        for (int h = 0; h < height + 1; h++)
        {
            verticesV2.Add(new List<Vector3>());

            for (int w = 0; w < width + 1; w++)
            {
                //reverse the height order with height-h, where rows
                //farthest away from our player is first
                verticesV2[h].Add(new Vector3(w * widthUnit, 0, (height - h) * heightUnit));
            }
        }
    }

    private void CreateControlVertices()
    {
        vertices = new Vector3[(height + 1) * (width + 1)];

        for (int i = 0, vertIndex = 0; i <= height; i++)
        {
            for (int k = 0; k <= width; k++)
            {
                vertices[vertIndex] = new Vector3(k * widthUnit, 0, (height - i) * heightUnit);
                vertIndex++;
            }
        }

    }

    void RefreshRenderedPointsV2()
    {
        //in case we change in inspector druing runtime,
        //could later set turn this off if exported
        rvHeight = height * (fillPointCount + 1);
        rvWidth = width * (fillPointCount + 1);

        rendVerticesV2.Clear();



        //for (int )
        for (int rvH = 0; rvH < rvHeight + 1; rvH++)
        {
            rendVerticesV2.Add(new List<Vector3>());
        }


        for (int h = 0, rvH = 0; h < height + 1; h++, rvH += fillPointCount + 1)
        {
            for (int w = 0, rvW = 0; w < width + 1; w++, rvW += fillPointCount + 1)
            {
                //first point of inbetweens overlaps with non inbetweens
                Vector3 refPos1 = verticesV2[h][w];
                rendVerticesV2[rvH][rvW] = refPos1;

                if (w == width)
                {
                    //no w + 1 exists, let's not try to fill that in
                    break;
                }
                
                Vector3 refPos2 = verticesV2[h][w + 1];

                //start at 1 because we already did the index = 0 one,
                //save computatino time by doing it manually
                for (int i = 1; i < fillPointCount + 1; i++)
                {
                    rendVerticesV2[rvH][rvW + i] = smoothERPV2(refPos1, refPos2, i);
                }
            }
        }

        for (int rvW = 0; rvW < rvWidth + 1; rvW++)
        {
            //here not rvHright + 1 because we don't need to do anything
            //on the end edge case because those points are already filled
            //(for the i = 0 case), we would just break anyways
            for (int rvH = 0; rvH < rvHeight; rvH += fillPointCount + 1)
            {
                //this tiem we don't add the i = 0 case because it already exists

                Vector3 refPos1 = rendVerticesV2[rvH][rvW];
                Vector3 refPos2 = rendVerticesV2[rvH + fillPointCount + 1][rvW];

                for (int i = 1; i < fillPointCount + 1; i++)
                {
                    rendVerticesV2[rvH + i][rvW] = smoothERPV2(refPos1, refPos2, i);
                }
            }
        }
    }

    void RefreshRenderedPoints()
    {
        int rvHeight = height + fillPointCount * (height);
        int rvWidth = width + fillPointCount * (width);

        Vector3[] renderedVerts = new Vector3[(rvHeight + 1) * (rvWidth + 1)];

        //build list with full rows where control rows would be
        for (int i = 0, rowOffset = 0; i <= height; i++)
        {
            for (int k = 0, colOffset = 0; k <= width; k++)
            {
                int rvStartIndex = (i + rowOffset) * (rvWidth + 1) + k + colOffset;

                Vector3 firstRefPoint = vertices[i * (width + 1) + k];

                renderedVerts[rvStartIndex] = firstRefPoint;

                //no in between points on last vertecie
                if (k == width) { continue; }

                //fill in between points
                Vector3 secondRefPoint = vertices[i * (width + 1) + k + 1];

                for (int j = 0; j < fillPointCount; j++)
                {
                    renderedVerts[rvStartIndex + j + 1] = smoothERP(firstRefPoint, secondRefPoint, j);
                }


                colOffset += fillPointCount;
            }

            rowOffset += fillPointCount;
        }

        //now fill the rest of the empty rows
        for (int g = 0, rowOffset = 0; g < height; g++)
        {

            for (int w = 0; w <= rvWidth; w++)
            {
                int rvStartIndex = (g + rowOffset + 1) * (rvWidth + 1) + w;
                Vector3 firstRefPoint = renderedVerts[(g + rowOffset) * (rvWidth + 1) + w];
                Vector3 secondRefPoint = renderedVerts[(g + rowOffset + fillPointCount + 1) * (rvWidth + 1) + w];

                for (int q = 0; q < fillPointCount; q++)
                {
                    int index = rvStartIndex + q * (rvWidth + 1);
                    renderedVerts[index] = smoothERP(firstRefPoint, secondRefPoint, q);
                }

            }

            rowOffset += fillPointCount;
        }


        rendVertices = (Vector3[])renderedVerts.Clone();
    }



    Vector3 smoothERPV2(Vector3 refPos1, Vector3 refPos2, int inBetweenIndex)
    {
        Vector3 diff = refPos2 - refPos1;
        float sinRadAng = (Mathf.PI) / (float)(fillPointCount + 1);
        float theta = sinRadAng * inBetweenIndex;

        float curveMultiplyer = RunnerGameObject.easingFunction(theta);
        float lerpMultiplyer = (float)inBetweenIndex / (float)(fillPointCount + 1);

        return (new Vector3(diff.x * lerpMultiplyer, diff.y * curveMultiplyer, diff.z * lerpMultiplyer) + refPos1);
    }

    Vector3 smoothERP(Vector3 firstRefPoint, Vector3 secondRefPoint, int index)
    {
        Vector3 diff = secondRefPoint - firstRefPoint;
        float sinRadAng = (Mathf.PI) / (float)(fillPointCount + 1);
        float theta = sinRadAng * (index + 1);

        float curveMultiplyer = RunnerGameObject.easingFunction(theta);
        float lerpMultiplyer = (float)(index + 1) / (float)(fillPointCount + 1);

        return (new Vector3(diff.x * lerpMultiplyer, diff.y * curveMultiplyer, diff.z * lerpMultiplyer) + firstRefPoint);

    }

    private void OnDrawGizmosV2()
    {
        if (!drawPoints) { return; }

        List<List<Vector3>> points = drawOnlyControlPoints? verticesV2 : rendVerticesV2;


        foreach (List<Vector3> posRow in points)
        {
            foreach (Vector3 pos in posRow)
            {
                Gizmos.DrawSphere(pos, 0.2f);
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (V2_RIPCORD)
        {
            OnDrawGizmos();
            return;
        }


        if (/*rendVertices == null || */!drawPoints) { return; }

        Vector3[] points = drawOnlyControlPoints ? vertices : rendVertices;

        //Gizmos.DrawSphere(Vector3.zero, 0.2f);

        for (int i = 0; i < points.Length; i++)
        {
            Vector3 point = points[i];
            Gizmos.DrawSphere(point, 0.2f);
            /*
            continue;
            if (point.z <= (height - topMargin) * heightUnit
                && point.z >= bottomMargin * heightUnit
                && point.x <= (width - rightMargin) * widthUnit
                && point.x >= leftMargin * widthUnit)
            {
                Gizmos.DrawSphere(point, 0.2f);
            }
            */
        }
    }

    private void Update()
    {
        updateTreadmillSpeed();
        generateTerrain();
        generateTerrObjs();
        
        moveMesh();
        updateTerrObjAlpha();
        UpdateAliens();
        updateMesh();

        destroyOverlappingBullet();
    }


    void generateTerrain()
    {
        if (doNotGenerateShit) { return; }

        if (!canGenerateOthers) { return; }
        foreach (TerrTile terr in terrains)
        {

            if (trueFromLikelihood(terr.appearanceLikelihood) && terr.canGen())
            {
                terr.StartGenerateDelay();
                addTerrain(terr);
            }
        }
        
    }

    void generateTerrObjs()
    {
        if (hideTerrObjs) { return; }

        foreach (TerrObject terrObj in terrObjects)
        {
            if (trueFromLikelihood(terrObj.appearanceLikelihood))
            {
                if (!terrObj.canHaveAttachments)
                {
                    if (V2_RIPCORD)
                    {
                        AddTerrObjV2(terrObj);
                        continue;
                    }

                    AddTerrObj(terrObj);
                    continue;
                }

                TerrObject attachment = null;

                foreach (TerrObject attachmentTerrObj in attachmentTerrObjs)
                {
                    if (trueFromLikelihood(attachmentTerrObj.appearanceLikelihood))
                    {
                        attachment = attachmentTerrObj;
                        break;
                    }
                }

                if (V2_RIPCORD)
                {
                    AddTerrObjV2(terrObj, attachment);
                    continue;
                }
                AddTerrObj(terrObj, attachment);
            }
        }
    }


    bool trueFromLikelihood(float appearanceLikelihood)
    {
        float likelihoodSelector = Random.Range(0.00001f, 1f);
        float speedFactor = currTMSpeed / treadmillSpeed;
        float accelFactor = treadmillSpeed / startingTreadmillSpeed;
        speedFactor = currTMSpeed < 0.01f ? -1 : speedFactor;

        return likelihoodSelector < appearanceLikelihood * speedFactor * accelFactor;
    }


    IEnumerator generateOthersDelay(float delay)
    {
        canGenerateOthers = false;
        yield return new WaitForSecondsRealtime(delay);
        canGenerateOthers = true;
    }


    //TODO: once in use rename to addTerrTile
    void addTerrrainV2(TerrTile terrTile)
    {
        int offset = 0;

        //Vector3 posOffset = //new Vector3(verticesV//new Vector3(vertices[0].x, 0, vertices[0].z - (terrain.vertH() * heightUnit) + heightUnit);
    }


    void addTerrain(TerrTile terrain)
    {
        int offset = 0;

        Vector3 posOffset = new Vector3(vertices[0].x, 0, vertices[0].z - (terrain.vertH() * heightUnit) + heightUnit);

        //here not <= because terrain heights are the vertecie count

        //todo: wierd shit with 30 exact? bigger nums?
        int startOffset = Random.Range(0, width + 1);

        for (int i = 0; i < terrain.vertH(); i++)
        {
            for (int k = 0; k < terrain.vertW(); k++)
            {
                int kOffset = (startOffset + k) % (width + 1);
                int horzDiff = kOffset - k;

                int vertIndex = i * width + kOffset + offset;
                int terrainIndex = i * terrain.vertW() + k;

                Vector3 terrPnt = terrain.terrainData[terrainIndex];

                float actualElevation = terrain.canGenerateOthers && terrPnt.y >= 0f? Mathf.Max(vertices[vertIndex].y, terrPnt.y) : terrPnt.y;

                vertices[vertIndex] = new Vector3((terrPnt.x + horzDiff) * widthUnit, actualElevation, terrPnt.z * heightUnit) + posOffset;
            }
            offset++;
        }

        refreshTerrObjDict();
        RefreshRenderedPoints();
    }


    void refreshTerrObjDictV2()
    {
        foreach (var row in currTerrObjsDictV2.Values)
        {
            foreach(var col in row.Values)
            {
                foreach(TerrObject terrObject in col)
                {
                    Vector3 terrObjPos = terrObject.transform.position;
                    float elevVal = getElevationAtPos(terrObjPos.x, terrObjPos.z);
                    terrObject.PlaceOnEnvironmentCoord(new Vector3(terrObjPos.x, elevVal, terrObjPos.z));
                    //terrObject.transform.position = new Vector3(terrObjPos.x, hitBoxElevOffset(terrObject) + elevVal, terrObjPos.z);
                }
            }
        }
    }

    void refreshTerrObjDict()
    {
        for (int i = 0; i < vertices.Length; i++)
        {
            if (currTerrObjsDict.ContainsKey(i))
            {
                for (int k = 0; k < currTerrObjsDict[i].Count; k++)
                {
                    TerrObject terrObject = currTerrObjsDict[i][k];
                    
                    Vector3 terrObjPos = terrObject.transform.position;

                    //TODO: can optimize this method if provide indices as prev done below
                    float elevVal = getElevationAtPos(terrObjPos.x, terrObjPos.z);

                    currTerrObjsDict[i][k].transform.position = new Vector3(terrObjPos.x, hitBoxElevOffset(terrObject) + elevVal, terrObjPos.z);

                    

                    //old way:

                    /*
                    Vector3 ogPos = terrObject.transform.position;
                    Vector3 firstRef = vertices[i];
                    Vector3 secondRef = firstRef;

                    float zPosDiff = ogPos.z - firstRef.z;


                    //behind vertices[i].z point
                    if (zPosDiff > 0 && (i - width - 1) >= 0)
                    {
                        //get ref point behind
                        secondRef = vertices[i - width - 1];
                    }
                    //in front
                    else if (zPosDiff < 0 && (i + width + 1) < vertices.Length )
                    {
                        secondRef = vertices[i + width + 1];
                    }

                    float actualYPosDiff = 0;
                    if (secondRef.y != firstRef.y)
                    {
                        float theta = (zPosDiff / (secondRef.z - firstRef.z)) * Mathf.PI;
                        float yDiff = secondRef.y - firstRef.y;
                        actualYPosDiff = yDiff * easingFunction(theta);
                    }

                    currTerrObjsDict[i][k].transform.position = new Vector3(ogPos.x, firstRef.y + actualYPosDiff + hitBoxElevOffset(terrObject), ogPos.z);
                    */

                }
            }
        }
    }




    float getElevationAtPos(float xVal, float zVal)
    {
        //xVal = xVal / widthUnit;
        //zVal = zVal / heightUnit;

        Vector3 refNE = Vector3.zero;
        Vector3 refSE = Vector3.zero;
        Vector3 refSW = Vector3.zero;
        Vector3 refNW = Vector3.zero;

        int indexN = 0;
        int indexS = 0;
        
        for (int i = 0; i < height; i++)
        {
            Vector3 currFirstRef = vertices[i * (width + 1)];
            Vector3 currSecondRef = vertices[(i + 1) * (width + 1)];

            if (zVal <= currFirstRef.z && zVal > currSecondRef.z)
            {
                indexN = i * (width + 1);
                indexS = (i + 1) * (width + 1);

                break;
            }
        }


        for (int k = 0; k < width; k++)
        {
            Vector3 currFirstRef = vertices[k];
            Vector3 currSecondRef = vertices[k+1];
            if (xVal >= currFirstRef.x && xVal < currSecondRef.x)
            {
                refNW = vertices[indexN + k];
                refNE = vertices[indexN + k + 1];
                refSW = vertices[indexS + k];
                refSE = vertices[indexS + k + 1];
                break;
            }
        }


        float diffNWlev = refNE.y - refNW.y;
        float diffSWlev = refSE.y - refSW.y;
        float diffX = (xVal - refNW.x) / widthUnit ;
        float diffY = (refNW.z - zVal) / heightUnit;

        float thetaX = diffX * Mathf.PI;
        float thetaY = diffY * Mathf.PI;

        float midPt1Y = RunnerGameObject.easingFunction(thetaX) * diffNWlev + refNW.y;
        float midPt2Y = RunnerGameObject.easingFunction(thetaX) * diffSWlev + refSW.y;

        float diffFinal = midPt2Y - midPt1Y;

        return RunnerGameObject.easingFunction(thetaY) * diffFinal + midPt1Y;

    }



    void AddTerrObjV2(TerrObject terrObject, TerrObject attachmentObject = null, TerrObject parentInstance = null, int? parentColIndex = -1, float? parentDepthOffset = null)
    {
        int colIndex = parentColIndex ?? Random.Range(0, width + 1);
        int realColIndex;
        int flipMultiplyer;
        terrObject.GetTheoreticalSpawnInfo(colIndex, width, out realColIndex, out flipMultiplyer, parentInstance);

        float depthOffset = terrObject.GetDepthOffset(heightUnit, parentDepthOffset);

        if (!CanAddToThisLaneV2(terrObject, colIndex, depthOffset))
        {
            return;
        }

        float horizOffset = terrObject.GetHorizontalOffset(widthUnit, flipMultiplyer);

        //we don't accomodate for the terrain elevation here just yet
        //we do that later when generating new terrain, save computation time here
        float elevationOffset = terrObject.GetElevationOffset();

        Vector3 spawnPos = new Vector3(horizOffset, elevationOffset, depthOffset);

        TerrObject terrObjInst = terrObject.InitTerrObj(spawnPos, flipMultiplyer, parentInstance);

        if (!currTerrObjsDictV2.ContainsKey(0))
        {
            currTerrObjsDictV2.Add(0, new Dictionary<int, List<TerrObject>>());
        }

        currTerrObjsDictV2[0][realColIndex].Add(terrObjInst);

        //now process attachment obejct to try and generate
        if (attachmentObject != null)
        {
            AddTerrObjV2(attachmentObject, null, terrObjInst, realColIndex, depthOffset);
        }
    }


    void AddTerrObj(TerrObject terrObj, TerrObject attachment = null)
    {
        int index;
        float vertOffset;
        TerrObject terrObjInst = InitTerrObj(terrObj, out index, out vertOffset);

        if (terrObjInst == null) { return; }

        if (!currTerrObjsDict.ContainsKey(index))
        {
            currTerrObjsDict.Add(index, new List<TerrObject>());
        }

        currTerrObjsDict[index].Add(terrObjInst);

        if (attachment == null) { return; }

        int attachmentIndex;
        float attachmentVertOffset;
        int parentFlipDir = terrObjInst.IsFlipped ? -1 : 1;
        TerrObject attachmentTerrObjInst = InitTerrObj(attachment, out attachmentIndex, out attachmentVertOffset, index, vertOffset);

        if (attachmentTerrObjInst == null) { return; }

        terrObjInst.AddAlienAttachment(attachmentTerrObjInst);
        //terrObjInst.AttachmentObjects.Add(attachmentTerrObjInst);

        if (!currTerrObjsDict.ContainsKey(attachmentIndex))
        {
            currTerrObjsDict.Add(attachmentIndex, new List<TerrObject>());
        }

        currTerrObjsDict[attachmentIndex].Add(attachmentTerrObjInst);
    }

    TerrObject InitTerrObj(TerrObject terrObj, out int index, out float vertOffset, int givenIndex = -1, float? givenVertOffset = null)
    {
        //TODO: make a flip terrObject method within TerrObject
        
        int flipMultiplyer = terrObj.canFlip ? Random.Range(0, 2) * 2 - 1 : 1;

        index = givenIndex == -1? Random.Range(0, width+1) : givenIndex;

        //make sure things wrap correctly on the same row with actual offsets,
        //and so no indexes too high if on the last row
        int actualLaneOffset = terrObj.laneOffset * flipMultiplyer;

        if (index % (width + 1) == 0 && actualLaneOffset < 0)
        {
            index += (width + 1 + actualLaneOffset);
        }
        else if (index % (width + 1) == width && actualLaneOffset > 0)
        {
            index += (actualLaneOffset - (width + 1));
        }
        else
        {
            index += actualLaneOffset;
        }

        vertOffset = givenVertOffset == null? Random.Range(-(heightUnit) / 2f, heightUnit / 2f) : (float) givenVertOffset + TerrObject.ATTACHMENT_SPACING;//TerrObject.OBJ_TYPE.STATIC ? Random.Range(-(heightUnit) / 2f, heightUnit / 2f) : 0f;

        if (terrObj.objType == TerrObject.OBJ_TYPE.STATIC_HAZ && !canAddToThisLane(index, vertOffset)) { return null; }




        //if its not a hazard, meaning it doesnt have to be in a designated lane, can be anywhere in the horz.
        float horizOffset = terrObj.objType == TerrObject.OBJ_TYPE.STATIC ? Random.Range(-(widthUnit) / 2f, widthUnit / 2f) : 0f;


        if (terrObj.centerXPosWithHitBox)
        {
            horizOffset -= terrObj.GetOffsetFromHitBox().x * flipMultiplyer;
        }

        TerrObject terrObjInst = Instantiate(terrObj);

        float speedMultiplyer = treadmillSpeed / startingTreadmillSpeed;

        //TODO: just flip the whole thing including the hit box, then get this shit

        //TODO: future if using log with middle roll, sides jump, need to change this
        //for now just assuming all terrObj are 1 width in hitbox, and skinny ( /4) so that doesn't hit on changing lanes too soon
        terrObjInst.hitBox.size = new Vector3(calcHitBoxSizeX(terrObjInst.transform.localScale.x), terrObjInst.hitBox.size.y, terrObjInst.hitBox.size.z * 4 * speedMultiplyer);

        terrObjInst.hitBox.center = new Vector3(terrObjInst.hitBox.center.x * flipMultiplyer, terrObjInst.hitBox.center.y, terrObjInst.hitBox.center.z);

        //terrObj.hitBox.size = new Vector3(terrObj.hitBoxUnitWidth * widthUnit / terrObj.transform.localScale.x, terrObj.hitBox.size.y, terrObj.hitBox.size.z);

        terrObjInst.RandomizeSpriteType();

        //terrObjInst.gameObject.GetComponent<SpriteRenderer>().flipX = flipMultiplyer > 0 ? false : true;
        terrObjInst.flipTerrObj(flipMultiplyer);

        Vector3 pos = vertices[index] + new Vector3(horizOffset, hitBoxElevOffset(terrObj), vertOffset);

        terrObjInst.transform.position = pos;

        return terrObjInst;
    }

    private void OnDestroyedTerrObj(TerrObject terrObj)
    {
        foreach (int row in currTerrObjsDict.Keys)
        {
            if (currTerrObjsDict[row].Remove(terrObj))
            {
                if (currTerrObjsDict[row].Count == 0)
                {
                    currTerrObjsDict.Remove(row);
                }
                return;
            }
        }
    }

    //for each thing in 
    private bool CanAddToThisLaneV2(TerrObject terrObj, int index, float vertOffset)
    {
        foreach (var col in currTerrObjsDictV2.Keys)
        {
            foreach (var rowList in currTerrObjsDictV2[col].Values)
            {
                foreach(TerrObject terrobj in rowList)
                {
                    if (!terrobj.CanAddToLane(width, col, terrObj, index, vertOffset))
                    {
                        return false;
                    }
                }
            }
        }

        return true;
    }

    bool canAddToThisLane(int laneIndex, float vertOffset)
    {

        for (int currIndex = laneIndex; currIndex < vertices.Length; currIndex  += (width + 1))
        {
            if (currTerrObjsDict.ContainsKey(currIndex))
            {

                for (int i = 0; i < currTerrObjsDict[currIndex].Count; i++)
                {
                    TerrObject terrObj = currTerrObjsDict[currIndex][i];

                    float diffDist2TO = (vertices[laneIndex].z + vertOffset) - (terrObj.transform.position.z);

                    if (diffDist2TO < terrObj.minHeightUnitsForNextHaz * heightUnit)
                    {
                        return false;
                    } 

                }

            }
        }

        return true;
        

    }


    float calcHitBoxSizeX(float xScale = 1)
    {
        return widthUnit / 4f;
    }


    float hitBoxElevOffset(TerrObject terrObj)
    {
        return (terrObj.hitBox.size.y / 2f * terrObj.transform.localScale.y) * (1f -  terrObj.elevationOffsetPerc);
    }



    void InputUpdate(RunnerControls.InputData inputAction)
    {
        //RunnerGameObject.PLAYER_STATE action = playerControls.getAction(Time.deltaTime);

        if (!player.canChangeState()) { return; }


        float dodgeDelay = 0f;

        switch (inputAction.getState())
        {
            case RunnerGameObject.PLAYER_STATE.DODGE_L:
                dodgeDelay = player.dodge(false);
                StartCoroutine(changeLane(1, dodgeDelay));
                return;

            case RunnerGameObject.PLAYER_STATE.DODGE_R:
                dodgeDelay = player.dodge(true);
                StartCoroutine(changeLane(-1, dodgeDelay));
                return;

            case RunnerGameObject.PLAYER_STATE.JUMP:
                player.jump();
                return;

            case RunnerGameObject.PLAYER_STATE.ROLL:
                player.roll();
                return;

            case RunnerGameObject.PLAYER_STATE.SPRINT:
                player.sprint();
                return;

            case RunnerGameObject.PLAYER_STATE.THROW_R:
                player.throwRock();
                return;

            case RunnerGameObject.PLAYER_STATE.FIRE:
                player.fireGun();
                return;

            default:
                return;
        }

    }


    IEnumerator changeLane(int dir, float delayTime)
    {
        yield return new WaitForSecondsRealtime(delayTime);

        changeLaneDir = dir;
        //player will never be lane 0 or width+1 because doesn't reach ends
        playerLane = playerLane - dir;//Mathf.Clamp(playerLane - dir, 0, width);
        Debug.Log(playerLane);

    }


    void updateTreadmillSpeed()
    {
        //increase speed for game difficulty
        
        //incase of death and still
        if (targetTMSpeed > 0.0001f)
        {
            treadmillSpeed += 0;//treadmillAccel * Time.deltaTime * Time.deltaTime;
            currTMSpeed += 0;//treadmillAccel * Time.deltaTime * Time.deltaTime;
        }

        //if (treadmillSpeed % 0.01 < 0.01) { Debug.Log(currTMSpeed); }

        if (TMTimeTotal < TMSlowDownTime)
        {
            TMTimeTotal = Mathf.Min(TMSlowDownTime, TMTimeTotal + Time.deltaTime);

            float actualDelta = RunnerGameObject.easingFunction(TMTimeTotal / TMSlowDownTime * Mathf.PI);
            currTMSpeed = Mathf.Lerp(currTMSpeed, targetTMSpeed, actualDelta);

        }
    }

    void ChangeTMSpeed(bool treadmillOn, float changeTime, float speedMultiplyer)
    {
        TMTimeTotal = 0f;
        TMSlowDownTime = changeTime;
        targetTMSpeed = treadmillOn? treadmillSpeed * speedMultiplyer : 0f;
        
    }




    void AddThrowable(RunnerThrowable throwable) {
        BoxCollider hitBox = throwable.GetComponent<BoxCollider>();
        hitBox.size = new Vector3(hitBox.size.x, hitBox.size.y, hitBox.size.z);
        throwables.Add(throwable); }

    void RemoveThrowable(RunnerThrowable throwable) { throwables.Remove(throwable); }


    void moveMesh()
    {
        //for controling lane changing

        float speedToApply = currTMSpeed * Time.deltaTime;

        float change = 0f;

        if (changeLaneDir != 0)
        {
            if (currChangeLaneTime >= changeLaneTime)
            {
                changeLaneDir = 0;
                currChangeLaneTime = 0;
                xLaneChangePositionProgress = 0f;
            }

            else
            {
                currChangeLaneTime = Mathf.Min(currChangeLaneTime + Time.deltaTime, changeLaneTime);

                float theta = currChangeLaneTime / changeLaneTime * Mathf.PI;//changeLaneStep * totalFrameCounter * Mathf.PI;
                float progressVal = RunnerGameObject.easingFunction(theta);

                float currDist = Mathf.Lerp(0, widthUnit * changeLaneDir, progressVal);
                change = currDist - xLaneChangePositionProgress;
                xLaneChangePositionProgress = currDist;
            }
        }



        //applying treadmill move and lane change
        for (int i = 0; i < rendVertices.Length; i++)
        {
            rendVertices[i] += new Vector3(change, 0, -speedToApply);
        }


        for (int k = 0; k < vertices.Length; k++)
        {
            vertices[k] += new Vector3(change, 0, -speedToApply);
        }


        foreach (int key in currTerrObjsDict.Keys)
        {
            for (int t = 0; t < currTerrObjsDict[key].Count; t++)
            {
                currTerrObjsDict[key][t].transform.position += new Vector3(change, 0, -speedToApply);
            }
            
        }

        //won't need to worry about wrapping for thrown objects b/c doesn't make sense to
        //cause they moving too fast anyways
        foreach(RunnerThrowable throwable in throwables)
        {
            throwable.transform.position += new Vector3(change, 0, 0);
        }

        float playerXVal = player.transform.position.x;
        float playerZVal = player.transform.position.z;

        float elevOffset = player.hitBox.size.y / 2f * player.transform.localScale.y;
        player.transform.position = new Vector3(playerXVal, getElevationAtPos(playerXVal, playerZVal) + elevOffset, playerZVal);


        //incase points move so much that need to move rows more than once
        //fire safety here too lol
        int safety = 0;
        while (applyTreadmillEffect()) { safety++; if (safety > 10) { break; } }
        safety = 0;
        while (applyHorizontalWrap()) { safety++; if (safety > 10) { break; } }
        RefreshRenderedPoints();
    }



    bool applyTreadmillEffect()
    {
        //TODO: convert this to using the new double array
        if (vertices[0].z < (height - 1) * heightUnit)
        {
            Vector3[] shortenedList = new Vector3[vertices.Length - (width + 1)];

            Vector3[] prevVertices = (Vector3[])vertices.Clone();

            for (int i = 0; i < shortenedList.Length; i++)
            {
                shortenedList[i] = vertices[i];
            }

            for (int k = 0; k < vertices.Length; k++)
            {
                if (k <= width)
                {
                    vertices[k] = new Vector3(shortenedList[0].x + k * widthUnit, 0, shortenedList[0].z + heightUnit);
                }
                else
                {
                    vertices[k] = shortenedList[k - width - 1];
                }
            }

            if (V2_RIPCORD)
            {
                Dictionary<int, Dictionary<int, List<TerrObject>>> newGrid = new Dictionary<int, Dictionary<int, List<TerrObject>>>();
                foreach (int row in currTerrObjsDictV2.Keys)
                {
                    //if this row is at the end of the "treadmill", lets delete all the objects in that row
                    if (row >= height - 1)
                    {
                        int colCount = currTerrObjsDictV2[row].Keys.Count;
                        for (int colIndex = 0; colIndex < colCount; colIndex++)
                        {
                            int objCount = currTerrObjsDictV2[row][colIndex].Count;
                            for (int objIndex = 0; objIndex < objCount; objIndex++)
                            {
                                Destroy(currTerrObjsDictV2[row][colIndex][objIndex]);
                            }
                        }
                        continue;
                    }

                    //move all rows up by one
                    newGrid.Add(row + 1, currTerrObjsDictV2[row]);
                }

                currTerrObjsDictV2 = newGrid;
                return true;
            }

            

            //now adjust index (which are the keys for the dictionary) so that they
            //change by one row

            List<int> terrKeys2Remove = new List<int>();

            foreach (int key in currTerrObjsDict.Keys)
            {

                if (key >= vertices.Length - (width + 1))
                {
                    terrKeys2Remove.Add(key);
                    for (int t = 0; t < currTerrObjsDict[key].Count; t++)
                    {
                        Destroy(currTerrObjsDict[key][t].gameObject);
                    }

                }
            }

            foreach (int key in terrKeys2Remove)
            {
                currTerrObjsDict.Remove(key);
            }

            Dictionary<int, List<TerrObject>> copy = new Dictionary<int, List<TerrObject>>();

            foreach (int key in currTerrObjsDict.Keys)
            {
                int nextRowIndex = key + width + 1;
                copy.Add(nextRowIndex, new List<TerrObject>());
                for (int t = 0; t < currTerrObjsDict[key].Count; t++)
                {
                    copy[nextRowIndex].Add(currTerrObjsDict[key][t]);
                }
            }

            currTerrObjsDict = copy;

            return true;
            
        }





        return false;

    }


    bool applyHorizontalWrap()
    {
        if (vertices[0].x > widthUnit || vertices[0].x < -widthUnit)
        {
            bool movingRight = vertices[0].x > 1;
            int modifier = movingRight ? 1 : 0;
            int invModifier = movingRight ? 0 : 1;
            int dirModifier = movingRight ? 1 : -1;

            playerLane += dirModifier;

            Vector3[] shortenedList = new Vector3[vertices.Length - (height + 1)];


            for (int i = 0, offset = 0; i < vertices.Length; i++)
            {
                //anything on the right most column for moving right, left col for moving left

                if ((i + modifier) % (width + 1) == 0) { offset++; continue; }

                shortenedList[i - offset] = vertices[i];

            }


            Vector3[] vertCopy = (Vector3[])vertices.Clone();

            for (int k = 0, offset = 0; k < vertices.Length; k++)
            {
                //if moving right, want to catch when on the left most col
                //moving left, catch the right most col
                if ((k + invModifier) % (width + 1) == 0)
                {
                    //subtract or add the index depending on col on
                    Vector3 newPnt = vertCopy[k + width * dirModifier];

                    //move x by one control unit left or right depending on dir
                    vertices[k] = new Vector3(vertCopy[k].x - dirModifier * widthUnit, newPnt.y, newPnt.z);
                    offset++;
                }
                else
                {
                    vertices[k] = shortenedList[k - offset];
                }
            }



            Dictionary<int, List<TerrObject>> currTerrObjsCopy = new Dictionary<int, List<TerrObject>>();


            //now adjust terr objects

            foreach (int key in currTerrObjsDict.Keys)
            {
                // 0 (or multiple of width + 1) mod (width + 1) = 0, modifier == 0 when moving left
                // (width + 1) - 1 mod (width + 1) = (width + 1) - 1, modifier == 1 when moving right
                int newIndex = (key) % (width + 1) == width * modifier ? key - width * dirModifier : key + dirModifier;
                float offset = (key) % (width + 1) == width * modifier ? -(width + 1) * widthUnit * dirModifier : 0;

                currTerrObjsCopy.Add(newIndex, new List<TerrObject>());

                for (int i = 0; i < currTerrObjsDict[key].Count; i++)
                {
                    currTerrObjsDict[key][i].transform.position += new Vector3(offset, 0, 0);
                    currTerrObjsCopy[newIndex].Add(currTerrObjsDict[key][i]);
                }
            }

            currTerrObjsDict = currTerrObjsCopy;


            return true;
        }

        return false;
    }



    void updateTerrObjAlpha()
    {
        foreach (int key in currTerrObjsDict.Keys)
        {
            for (int t = 0; t < currTerrObjsDict[key].Count; t++)
            {
                TerrObject currTO = currTerrObjsDict[key][t];
                float diff = player.transform.position.z - currTO.transform.position.z;

                if (diff > 0f)
                {
                    SpriteRenderer sr = currTO.GetComponent<SpriteRenderer>();
                    float multiplyer = (distBehindPlayer2ZeroAlpha - diff) / distBehindPlayer2ZeroAlpha;
                    multiplyer = multiplyer < 0 ? 0f : multiplyer;

                    /*currTerrObjsDict[key][t].GetComponent< SpriteRenderer >()*/sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, multiplyer * sr.color.a);
                }
                //currTerrObjsDict[key][t].transform.position += new Vector3(change, 0, -currTMSpeed);
            }

        }
    }

    void UpdateAliens()
    {
        foreach (TerrObject terrObject in GetObjectsInLane(playerLane))
        {
            if (terrObject.objType == TerrObject.OBJ_TYPE.ENEMY && !terrObject.played)
            {
                float dist = terrObject.transform.position.z - player.transform.position.z;
                //quadratic formula
                float timeToClostDist = (-currTMSpeed + Mathf.Sqrt(currTMSpeed * currTMSpeed - 4 * (treadmillAccel / 2f) * (-dist)))/(treadmillAccel);
                if (timeToClostDist < terrObject.alienAttackTime && timeToClostDist > (terrObject.alienAttackTime - TerrObject.ALIEN_ATTACK_THRESHOLD ))
                {
                    terrObject.PlayAnimation();
                }
            }
        }
    }

    List<TerrObject> GetObjectsInLane(int lane)
    {
        List<TerrObject> objectsInLane = new List<TerrObject>();

        for (int i = lane; lane < (width+1) * (height+1); lane += width + 1)
        {
            if (currTerrObjsDict.ContainsKey(lane))
            {
                foreach(TerrObject terrObject in currTerrObjsDict[lane])
                {
                    objectsInLane.Add(terrObject);
                }
            }
        }

        return objectsInLane;
    }

    void updateMesh()
    {
        mesh.vertices = rendVertices;

        if (drawShapes)
        {
            createRendShapes();
            mesh.triangles = triangleRenIndices;
            mesh.RecalculateNormals();
        }
    }



    void createRendShapes()
    {
        triangleRenIndices = new int[rendHeight * rendWidth * 6];
        int vert = 0;
        int triIndex = 0;

        for (int i = 0; i < rendHeight; i++)
        {
            for (int k = 0; k < rendWidth; k++)
            {
                //verticies are from right to left, and moves a row down

                //draws from top left to bottom right, clockwise
                triangleRenIndices[triIndex] = vert;
                triangleRenIndices[triIndex + 1] = vert + 1;
                //need to add 2 because moved over a column (one extra vertice per column)
                triangleRenIndices[triIndex + 2] = vert + rendWidth + 2;


                //draws from bottom right to top left, clockwise
                triangleRenIndices[triIndex + 3] = vert + rendWidth + 2;
                triangleRenIndices[triIndex + 4] = vert + rendWidth + 1;
                triangleRenIndices[triIndex + 5] = vert;
                vert++;
                triIndex += 6;

            }
            vert++;
        }
    }

    //if the bullet goes through the mesh, don't want it to come out the end!
    private void destroyOverlappingBullet()
    {
        foreach (RunnerThrowable throwable in throwables)
        {
            if (throwable.throwType != RunnerThrowable.THROW_TYPE.BULLET)
            {
                continue;
            }

            Vector3 pos = throwable.gameObject.transform.position;
            if (getElevationAtPos(pos.x, pos.z) > pos.y)
            {
                Debug.Log("destroyed bullet from sickMesh!");
                Destroy(throwable.gameObject);
            }
        }

    }

}


