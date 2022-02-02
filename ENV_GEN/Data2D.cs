﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class Data2D<T>
{
    private int cols;
    private int rows;
    private T[,] data;
    private Func<int, T> defaultNewVal;
    private Action<T> destructionCall;

    /// <summary>
    /// Creates a new 2D data structure where the top row is row 0,
    /// and the left column is column 0
    /// </summary>
    /// <param name="cols"></param>
    /// <param name="rows"></param>
    public Data2D(int cols, int rows, Func<int, T> defaultNewVal, Action<T> destructionCall)
    {
        this.cols = cols;
        this.rows = rows;
        this.data = new T[cols, rows];
        this.defaultNewVal = defaultNewVal;
        this.destructionCall = destructionCall;

        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < cols; c++)
            {
                data[c, r] = defaultNewVal(c);
            }
        }
    }

    /// <summary>
    /// Moves all rows up or down
    /// depending on whether the direction is negative(up) or positive(down), respecitively
    /// </summary>
    /// <param name="vertAmnt"></param>
    public void ShiftRows(int vertAmnt)
    {
        if (vertAmnt == 0 || vertAmnt > rows)
        {
            Debug.LogError("Error: row shift was more than the total amount of rows, or was zero!");
            return;
        }


        //move down. Two for loops instead of other operations for efficiency
        if (vertAmnt > 0)
        {
            //shifted rows
            for (int c = 0; c < cols; c++)
            {
                for (int r = rows - 1; r >= vertAmnt; r--)
                {
                    data[c, r] = data[c, r - vertAmnt];
                }
            }

            //reset the new row(s)
            for (int c = 0; c < cols; c++)
            {
                for (int r = 0; r < vertAmnt; r++)
                {
                    if (destructionCall != null)
                    {
                        destructionCall(data[c, r]);
                    }
                    data[c, r] = defaultNewVal(c);
                }
            }
            return;
        }

        //make positive so easier to work with
        vertAmnt = -vertAmnt;

        //shifted rows
        for (int c = 0; c < cols; c++)
        {
            for (int r = 0; r < rows - vertAmnt; r++)
            {
                data[c, r] = data[c, r + vertAmnt];
            }
        }

        //reset the new row(s)
        for (int c = 0; c < cols; c++)
        {
            for (int r = rows - vertAmnt; r < rows; r++)
            {
                if (destructionCall != null)
                {
                    destructionCall(data[c, r]);
                }
                data[c, r] = defaultNewVal(c);
            }
        }
    }

    /// <summary>
    /// Resets the element at the given location using the
    /// destroy and default valie methods
    /// </summary>
    /// <param name="colIndex"></param>
    /// <param name="rowIndex"></param>
    public void ResetElement(int colIndex, int rowIndex)
    {
        if (destructionCall != null)
        {
            destructionCall(data[colIndex, rowIndex]);
        }
        data[colIndex, rowIndex] = defaultNewVal(colIndex);
    }

    /// <summary>
    /// Moves all columns left or right
    /// depending on whether the number is negative(left) or positve(right), respectively
    /// </summary>
    /// <param name="horzDir"></param>
    public void ShiftColsWrapped(int horzAmnt)
    {
        if (horzAmnt == 0 || horzAmnt > cols)
        {
            Debug.LogError("Error: col shift was more than the total amount of columns, or was zero!");
            return;
        }

        //since we are wrapping, we need to cache the new data
        T[,] shiftedData = new T[horzAmnt, rows];

        if (horzAmnt > 0)
        {
            //cache wrapped col(s)
            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < horzAmnt; c++)
                {
                    shiftedData[c, r] = data[cols - horzAmnt + c, r];
                }
            }

            //rest of cols
            for (int r = 0; r < rows; r++)
            {
                for (int c = cols - 1; c > horzAmnt; c--)
                {
                    data[c, r] = data[c - horzAmnt, r];
                }
            }

            //apply cached wrapped ones
            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < horzAmnt; c++)
                {
                    data[c, r] = shiftedData[c, r];
                }
            }

            return;
        }

        //make positive so easier to work with
        horzAmnt = -horzAmnt;

        //cache wrapped col(s)
        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < horzAmnt; c++)
            {
                shiftedData[c, r] = data[c, r];
            }
                
        }

        //rest of cols
        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < cols - horzAmnt; c++)
            {
                data[c, r] = data[c + horzAmnt, r];
            }
        }

        //apply cached wrapped ones
        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < horzAmnt; c++)
            {
                data[rows - horzAmnt + c, r] = shiftedData[c, r];
            }
        }
    }

    /// <summary>
    /// Set an element in the 2D data structure
    /// </summary>
    /// <param name="element"></param>
    /// <param name="colIndex"></param>
    /// <param name="rowIndex"></param>
    public void SetElement(T element, int colIndex, int rowIndex)
    {
        data[colIndex, rowIndex] = element;
    }

    /// <summary>
    /// Get an element from the 2D data structure
    /// </summary>
    /// <param name="colIndex"></param>
    /// <param name="rowIndex"></param>
    /// <returns></returns>
    public T GetElement(int colIndex, int rowIndex)
    {
        return data[colIndex, rowIndex];
    }

    public void PrintData(string name)
    {
        Debug.LogFormat("Printing Data2D '{0}'", name);

        for (int r = 0; r < rows; r++)
        {
            StringBuilder rowString = new StringBuilder();
            rowString.AppendFormat("Row {0} contains: ", r);

            for (int c = 0; c < cols; c++)
            {
                rowString.AppendFormat("{0}, ", GetElement(c, r).ToString());
            }

            rowString.Append("ROW END");
            Debug.Log(rowString);
        }

        Debug.LogFormat("End of Data2D '{0}'", name);
    }
}
