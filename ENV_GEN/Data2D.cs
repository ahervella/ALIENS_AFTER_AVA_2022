using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Data2D<T>
{
    private int rows;
    private int cols;
    private T[,] data;

    /// <summary>
    /// Creates a new 2D data structure where the top row is row 0,
    /// and the left column is column 0
    /// </summary>
    /// <param name="rows"></param>
    /// <param name="cols"></param>
    public Data2D(int rows, int cols, Func<T> defaultNewVal)
    {
        data = new T[rows, cols];

        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < cols; c++)
            {
                SetElement(defaultNewVal(), r, c);
            }
        }
    }

    /// <summary>
    /// Moves all rows by a specific magnitude up or down
    /// depending on whether the number is negative(up) or positive(down), respecitively
    /// </summary>
    /// <param name="vertDir"></param>
    public void ShiftRows(int vertDir)
    {
        if (vertDir == 0) { return; }

        T[,] shiftedData = new T[rows, cols];

        int absVertDir = Mathf.Abs(vertDir);

        //move down
        if (vertDir > 0)
        {
            for (int r = absVertDir; r < rows; r++)
            {
                for (int c = 0; c < cols; c++)
                {
                    shiftedData[r, c] = data[r - absVertDir, c];
                }
            }
        }
        else
        {
            for (int r = 0; r < rows - absVertDir; r++)
            {
                for (int c = 0; c < cols; c++)
                {
                    shiftedData[r, c] = data[r + absVertDir, c];
                }
            }
        }

        data = shiftedData;
    }

    /// <summary>
    /// Moves all columns by a specific magnitude left or right
    /// depending on whether the number is negative(left) or positve(right), respectively
    /// </summary>
    /// <param name="horzDir"></param>
    public void ShiftColsWrapped(int horzDir)
    {
        if (horzDir == 0) { return; }

        T[,] shiftedData = new T[rows, cols];

        int absHorzDir = Mathf.Abs(horzDir);

        if (horzDir > 0)
        {
            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < cols; c++)
                {
                    shiftedData[r, c] = data[r, (c + absHorzDir) % cols];
                }
            }
        }
        else
        {
            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < cols; c++)
                {
                    shiftedData[r, (c + absHorzDir) % cols] = data[r, c];
                }
            }
        }

        data = shiftedData;
    }

    /// <summary>
    /// Set an element in the 2D data structure
    /// </summary>
    /// <param name="element"></param>
    /// <param name="rowIndex"></param>
    /// <param name="colIndex"></param>
    public void SetElement(T element, int rowIndex, int colIndex)
    {
        data[rowIndex, colIndex] = element;
    }

    /// <summary>
    /// Get an element from the 2D data structure
    /// </summary>
    /// <param name="rowIndex"></param>
    /// <param name="colIndex"></param>
    /// <returns></returns>
    public T GetElement(int rowIndex, int colIndex)
    {
        return data[rowIndex, colIndex];
    }

    /// <summary>
    /// Convert a 2D float dataa to a 1D Vector3 array
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    public static Vector3[] ConvertToMeshArray(float[,] data, Vector2 units)
    {
        Vector3[] meshData = new Vector3[data.Length];

        int meshIndex = 0;

        for(int r = 0; r < data.GetUpperBound(0); r++)
        {
            for (int c = 0; c < data.GetUpperBound(1); c++)
            {
                meshData[meshIndex++] = new Vector3(r * units.x, data[r, c], c * units.y);
            }
        }

        return meshData;
    }
}
