using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.IO;


public class Terrainnnn : MonoBehaviour
{
    const string terrainFolderPath = "Assets/RESOURCES/TERRAINS/";

    public TextAsset terrainFile;

    List<Vector3> tempTerrainData = new List<Vector3>();

    public Vector3[] terrainData;

    public int width = 0;
    public int height = 0;

    private void Start()
    {
        parseTerrainFile();
    }

    public void parseTerrainFile()
    {
        if (terrainFile == null) { return; }

        StreamReader sr = new StreamReader(terrainFolderPath + terrainFile.name + ".txt");

        //List<<List<string>> lines = new
        List<string[]> lines = new List<string[]>();

        string currLine = "";

        while ((currLine = sr.ReadLine()) != null)
        {
            lines.Add(currLine.Split(' '));
            
        }


        height = lines.Count;
        width = lines[0].Length;
        terrainData = new Vector3[width * height];

        for (int i = 0; i < height; i++)
        {
            
            for (int k = 0; k < width; k++)
            {
                int heightVal = (height - i - 1);
                int depthVal = int.Parse(lines[i][k]);

                terrainData[i * width + k] = new Vector3(k, depthVal, heightVal);
            }
        }

    }


}
