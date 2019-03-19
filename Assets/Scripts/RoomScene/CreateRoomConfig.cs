using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class CreateRoomConfig : MonoBehaviour
{
    public Slider slider;
    public Button createRoom;
    public Button cancel;
    public List<long> sliderValues;
    // Use this for initialization
    void Start()
    {
        slider.onValueChanged.AddListener(OnSliderUpdate);
    }

    private void OnApplicationQuit()
    {
        slider.onValueChanged.RemoveAllListeners();
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void SetSliderValue(List<long> values)
    {
        sliderValues = values;
        slider.minValue = 0;
        slider.maxValue = sliderValues.Count - 1;
        slider.wholeNumbers = true;
    }

    public void OnSliderUpdate(float value)
    {
        Debug.Log((int)value);
    }
}
