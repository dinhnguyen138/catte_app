using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WifiController : MonoBehaviour
{
    public Sprite[] wifiStatus;
    public Image wifiImage;
    int counter = 0;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (counter != 0 && counter < wifiStatus.Length)
        {
            wifiImage.gameObject.SetActive(true);
            wifiImage.sprite = wifiStatus[counter];
        }
        if (counter == 0)
        {
            wifiImage.gameObject.SetActive(false);
        }
    }

    public void Connected()
    {
        counter = 0;
    }

    public void Disconnected()
    {
        counter++;
        if (counter >= wifiStatus.Length)
        {
            counter = wifiStatus.Length - 1;
        }
    }
}
