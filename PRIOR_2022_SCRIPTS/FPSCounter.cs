using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FPSCounter : MonoBehaviour
{
    //text mesh pro ref for text
    [SerializeField]
    private bool useCustomRefreshTime = false;

    [SerializeField]
    private float refreshTime = 0.5f;

    [SerializeField]
    private TextMeshProUGUI textMesh;

    private void Start()
    {
        if (!textMesh)
        {
            Debug.LogError("Text mesh pro object not set in FPS Counter!");
            return;
        }

        if (useCustomRefreshTime)
        {
            StartCoroutine(tickCounter());
        }
    }

    private void Update()
    {
        if (useCustomRefreshTime)
        {
            return;
        }

        DisplayFPS(1f / Time.unscaledDeltaTime);
    }

    private IEnumerator tickCounter()
    {
        yield return new WaitForSeconds(refreshTime);
        DisplayFPS(1f / Time.unscaledDeltaTime);
        StartCoroutine(tickCounter());
    }

    private void DisplayFPS(float fpsCount)
    {
        textMesh.text = string.Format("FPS: {0}", fpsCount);
    }
}
