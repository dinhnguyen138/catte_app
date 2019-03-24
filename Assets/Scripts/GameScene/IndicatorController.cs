using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IndicatorController : MonoBehaviour
{
    TMPro.TextMeshProUGUI indicator;
    public Image crown;
    public bool isWinner = false;
    public bool isAmount = false;
    public string text = "";
    Color32[] winnerGradient;
    Color32[] loserGradient;

    // Start is called before the first frame update
    void Start()
    {
        indicator = GetComponent<TMPro.TextMeshProUGUI>();
        winnerGradient = new Color32[] {
            new Color32(0xFF, 0xFA, 0xDA, 0xFF),
            new Color32(0xF1, 0xC6, 0x2A, 0xFF)
        };
        loserGradient = new Color32[]
        {
            new Color32(0xFF, 0xFF, 0xFF, 0xFF),
            new Color32(0x52, 0x51, 0x52, 0xFF)
        };
    }

    // Update is called once per frame
    void Update()
    {
        indicator.text = text;
        crown.gameObject.SetActive(false);
        if (isAmount)
        {
            indicator.fontSize = 40;
        }
        else
        {
            indicator.fontSize = 65;
        }
        if (isWinner)
        {
            if (!isAmount)
            {
                crown.gameObject.SetActive(true);
            }
            indicator.colorGradient = new TMPro.VertexGradient(winnerGradient[0], winnerGradient[0], winnerGradient[1], winnerGradient[1]);
        }
        else
        {
            indicator.colorGradient = new TMPro.VertexGradient(loserGradient[0], loserGradient[0], loserGradient[1], loserGradient[1]);
        }
    }
}
