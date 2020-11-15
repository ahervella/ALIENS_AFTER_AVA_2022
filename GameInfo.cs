using UnityEngine;
using System.Collections;

public class GameInfo : MonoBehaviour
{
    public static GameInfo Current = null;

    private void Awake()
    {
        if (Current == null)
        {
            Current = this;
        }
        else if (Current != this)
        {
            Destroy(gameObject);
        }
    }

    public enum INFO
    {
        LIVE0, LIVE1, LIVE2, LIVE3
    };

    public bool blorp(INFO info)
    {
        return false;
    }
}
