using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class CreateRoomConfig : MonoBehaviour
{
    public Slider slider;
    public Dropdown dropdown;
    public GameObject slidePos;
    public Text amountText;
    public List<long> sliderValues;
    // Use this for initialization
    void Start()
    {
        slider.onValueChanged.AddListener(OnSliderUpdate);
        amountText.transform.position = new Vector3(slidePos.transform.position.x, slidePos.transform.position.y + 0.2f, slidePos.transform.position.z);
        
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
        amountText.text = Converter.ConvertToMoney(sliderValues[0]);
    }

    public void GetConfig(ref long amount, ref int number)
    {
        amount = sliderValues[(int)slider.value];
        number = int.Parse(dropdown.options[dropdown.value].text);
    }

    public void OnSliderUpdate(float value)
    {
        Debug.Log((int)value);
        amountText.transform.position = new Vector3(slidePos.transform.position.x, slidePos.transform.position.y + 0.3f, slidePos.transform.position.z);
        amountText.text = Converter.ConvertToMoney(sliderValues[(int)value]);
    }
}
