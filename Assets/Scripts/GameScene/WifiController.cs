using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WifiController : MonoBehaviour
{
    public Sprite[] wifiStatus;
    Image wifiImage;
    int counter = 0;
    // Start is called before the first frame update
    void Start()
    {
        wifiImage = GetComponent<Image>();    
    }

    // Update is called once per frame
    void Update()
    {
        if (counter < wifiStatus.Length)
        {
            wifiImage.sprite = wifiStatus[counter];
        }
    }

    public void setRetry(int retry)
    {
        counter = retry;
    }
}
