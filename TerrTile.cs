using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.IO;


public class TerrTile : MonoBehaviour
{
    const string TERRAIN_FOLDER_PATH = "Assets/RESOURCES/TERRAINS/";

    public TextAsset terrainFile;
    [SerializeField]
    private float appearanceLikelihood;
    public float AppearanceLikelihood => appearanceLikelihood;

    public Vector3[] terrainData;
    public List<List<Vector3>> terrainDataV2 = new List<List<Vector3>>();

    //control point vertecie Heights and Widths
    public int VertWidth { get; private set; } = 0;
    public int VertHeight { get; private set; } = 0;

    //multiplyer for text file floats (so user can use large numbers
    //for precision in text file)
    public float elevationMultiplyer = 0.05f;

    
    public float generateDelay = 0f;
    [SerializeField]
    private bool canGenerate = true;
    public bool CanGenerate => canGenerate;

    public bool canGenerateOthers = true;

    private void OnEnable()
    {
        ParseTerrainFile();
    }


    public void ParseTerrainFileV2()
    {
        //TODO: is it faster to use arrays until the end? Does it matter?
        if (terrainFile == null) { return; }

        List<List<float>> parsedData = new List<List<float>>();

        StreamReader sr = new StreamReader(TERRAIN_FOLDER_PATH + terrainFile.name + ".txt");

        string currLine = "";
        VertWidth = 0;

        while ((currLine = sr.ReadLine()) != null)
        {
            string[] rowOfStringNums = currLine.Split(' ');
            VertWidth = Mathf.Max(rowOfStringNums.Length, VertWidth);

            List<float> rowOfNums = new List<float>();

            for (int i = 0; i < rowOfStringNums.Length; i++)
            {
                rowOfNums.Add(float.Parse(rowOfStringNums[i]));

            }

            parsedData.Add(rowOfNums);
        }

        VertHeight = parsedData.Count;

        for (int i = 0; i < VertHeight; i++)
        {
            List<Vector3> rowOfPos = new List<Vector3>();

            for (int k = 0; k < VertWidth; k++)
            {
                //here it's more of how deep, not how tall, but want to avoid
                //confusion with the word depth
                float elevation = 0f;
                float reversed_i = VertHeight - i - 1;

                if (k < parsedData.Count)
                {
                    elevation = parsedData[i][k] * elevationMultiplyer;
                }

                rowOfPos.Add(new Vector3(k, elevation, reversed_i));
            }
        }
    }

    public void ParseTerrainFile()
    {
        //the direct pased data from the text file in the readable order
        List<float[]> parsedData = new List<float[]>();


        if (terrainFile == null) { return; }

        StreamReader sr = new StreamReader(TERRAIN_FOLDER_PATH + terrainFile.name + ".txt");


        string currLine = "";

        while ((currLine = sr.ReadLine()) != null)
        {
            string[] rowOfStringNums = currLine.Split(' ');
            float[] rowOfNums = new float[rowOfStringNums.Length];

            for (int i = 0; i < rowOfStringNums.Length; i++)
            {
                rowOfNums[i] = float.Parse(rowOfStringNums[i]);
            }

            parsedData.Add(rowOfNums);

        }


        VertHeight = parsedData.Count;
        VertWidth = parsedData[0].Length;
        terrainData = new Vector3[VertWidth * VertHeight];


        //convert depth string vals and location to Vector3 val
        for (int i = 0; i < VertHeight; i++)
        {

            for (int k = 0; k < VertWidth; k++)
            {
                float vertHeightVal = VertHeight - i - 1;
                float depthVal = parsedData[i][k] * elevationMultiplyer;

                terrainData[i * VertWidth + k] = new Vector3(k, depthVal, vertHeightVal);
            }
        }

        //terrain data indecies are stored from left to right, going down


    }


    public void StartGenerateDelay()
    {
        if ( generateDelay == 0f ) { return;  }

        canGenerate = false;
        StartCoroutine(GenDelay());

    }

    IEnumerator GenDelay()
    {
        yield return new WaitForSecondsRealtime(generateDelay);
        canGenerate = true;
    }

    public List<List<Vector3>> OffSetDataV2(float offset)
    {
        for(int row = 0; row < VertHeight; row++)
        {
            for (int col = 0; col < VertWidth; col++)
            {
                Vector3 old = terrainDataV2[row][col];
                terrainDataV2[row][col] = new Vector3(old.x, old.y, old.z + offset);
            }
        }

        return terrainDataV2;
    }

    //move the terrain shape up and down the plane
    public Vector3[] OffSetData(float offset)
    {
        Vector3[] newTerrainData = (Vector3[])terrainData.Clone();

        for (int i = 0; i < terrainData.Length; i++)
        {
            newTerrainData[i] = new Vector3(newTerrainData[i].x, newTerrainData[i].y, newTerrainData[i].z + offset);
        }

        return newTerrainData;
    }


}
