using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthBarManager : MonoBehaviour
{
    [SerializeField]
    private IntPropertySO currLives = null;

    [SerializeField]
    private GameObject healthChunkPrefab = null;

    [SerializeField]
    private float minAlpha = 1f;

    [SerializeField]
    private FloatPropertySO playerHealthBarAlphaPerc = null;

    private List<Image> healthChunks = new List<Image>();

    private void Start()
    {
        InstanceHealthBarChunks();
        currLives.RegisterForPropertyChanged(OnCurrLivesChange);
        playerHealthBarAlphaPerc.RegisterForPropertyChanged(OnHealthBarAlphaPercChange);
    }

    private void InstanceHealthBarChunks()
    {
        //In case we have dumby ones setup in the editor just for visual aid when working on them
        for (int i = 0; i < transform.childCount; i++)
        {
            Destroy(transform.GetChild(i).gameObject);
        }

        healthChunks.Clear();
        for (int i = 0; i < currLives.MaxValue(); i++)
        {
            GameObject inst = Instantiate(healthChunkPrefab, transform);
            healthChunks.Add(inst.GetComponent<Image>());
        }
    }
    
    private void OnCurrLivesChange(int prevLives, int newLives)
    {
        for (int i = 0; i < currLives.MaxValue(); i++)
        {
            if (i < newLives)
            {
                healthChunks[i].enabled = true;
            }
            else
            {
                healthChunks[i].enabled = false;
            }
        }
    }

    private void OnHealthBarAlphaPercChange(float oldPerc, float newPerc)
    {
        float alpha = Mathf.Lerp(minAlpha, 1, newPerc);
        foreach(Image chunk in healthChunks)
        {
            chunk.color = new Color(chunk.color.r, chunk.color.g, chunk.color.b, alpha);
        }
    }
}
