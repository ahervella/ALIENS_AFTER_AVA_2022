using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthDisplayManager : MonoBehaviour
{
    [SerializeField]
    private IntPropertySO livesSO = null;

    [SerializeField]
    List<GameObject> gameObjects = null;

    private void Start()
    {
        livesSO.RegisterForPropertyChanged(OnLivesChanged);
        OnLivesChanged(livesSO.Value, livesSO.Value);
    }

    void OnLivesChanged(int previous, int current)
    {
        List<int> enabled = new List<int>();
        switch (current)
        {
            case (-1):
                enabled = new List<int>() { 0, 1, 2, 3 };
                break;
            case (0):
                enabled = new List<int>() { 1, 2, 3 };
                break;
            case (1):
                enabled = new List<int>() { 2, 3 };
                break;
            case 2:
                enabled = new List<int>() { 3 };
                break;
            case (3):
                break;
            case 4:
                enabled = new List<int>(4);
                break;
            case 5:
                enabled = new List<int>() { 4, 5 };
                break;
        }
        for (int i = 0; i < gameObjects.Count; i++)
        {
            gameObjects[i].SetActive(enabled.Contains(i));
        }
    }
}
