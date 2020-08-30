using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.IO;


public class Terrainnnn : MonoBehaviour
{
    const string terrainFolderPath = "Assets/RESOURCES/TERRAINS/";

    public TextAsset terrainFile;

    public float appearanceLikelihood;

    public Vector3[] terrainData;

    //control point vertecie Heights and Widths
    public int vertWidth = 0;
    public int vertHeight = 0;

    //multiplyer for text file floats (so user can use large numbers
    //for precision in text file)
    public float elevationMultiplyer = 0.05f;




    private void OnEnable()
    {
        parseTerrainFile();
    }


    public void parseTerrainFile()
    {
        //the direct pased data from the text file in the readable order
        List<float[]> parsedData = new List<float[]>();


        if (terrainFile == null) { return; }

        StreamReader sr = new StreamReader(terrainFolderPath + terrainFile.name + ".txt");


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


        vertHeight = parsedData.Count;
        vertWidth = parsedData[0].Length;
        terrainData = new Vector3[vertWidth * vertHeight];


        //convert depth string vals and location to Vector3 val
        for (int i = 0; i < vertHeight; i++)
        {

            for (int k = 0; k < vertWidth; k++)
            {
                float vertHeightVal = vertHeight - i - 1;
                float depthVal = parsedData[i][k] * elevationMultiplyer;

                terrainData[i * vertWidth + k] = new Vector3(k, depthVal, vertHeightVal);
            }
        }

        //terrain data indecies are stored from left to right, going down


    }




    //move the terrain shape up and down the plane
    public Vector3[] offSetData(float offset)
    {
        Vector3[] newTerrainData = (Vector3[])terrainData.Clone();

        for (int i = 0; i < terrainData.Length; i++)
        {
            newTerrainData[i] = new Vector3(newTerrainData[i].x, newTerrainData[i].y, newTerrainData[i].z + offset);
        }

        return newTerrainData;
    }


}
