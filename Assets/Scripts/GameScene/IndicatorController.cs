using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IndicatorController : MonoBehaviour
{
    TMPro.TextMeshProUGUI indicator;
    public bool isWinner = false;
    public bool isAmount = false;
    public string text = "";
    // Start is called before the first frame update
    void Start()
    {
        indicator = GetComponent<TMPro.TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        indicator.text = text;
        if (isAmount)
        {
            indicator.fontSize = 35;
            indicator.outlineWidth = 0.12f;
            if (!isWinner)
            {
                indicator.faceColor = new Color32(0xF0, 0xFA, 0x4F, 0xFF);
                indicator.outlineColor = new Color32(0xE1, 0xFA, 0x70, 0xFF);
            }
            else
            {
                indicator.faceColor = new Color32(0xA8, 0xA8, 0xA6, 0xFF);
                indicator.outlineColor = new Color32(0xC5, 0xC5, 0xC4, 0xFF);
            }
        }
        else
        {
            indicator.fontSize = 60;
            indicator.outlineWidth = 0.25f;
            if (!isWinner)
            {
                indicator.faceColor = new Color32(0xA2, 0xA2, 0xA2, 0xFF);
                indicator.outlineColor = new Color32(0xC5, 0xC5, 0xC4, 0xFF);
            }
            else
            {
                indicator.faceColor = new Color32(0x39, 0xCC, 0xCC, 0xFF);
                indicator.outlineColor = new Color32(0x7F, 0xFD, 0xF9, 0xFF);
            }
        }
    }
}
