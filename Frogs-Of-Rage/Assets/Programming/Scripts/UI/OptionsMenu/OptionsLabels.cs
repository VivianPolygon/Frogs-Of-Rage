using UnityEngine;
using UnityEngine.UI;

//manages all the labels for the sliders on the options menu. Is scalable. 
public class OptionsLabels : MonoBehaviour
{
    //editable list of label classes set through the inspector
    [SerializeField] private SliderTextInfo[] _sliderLabelPairs;

    private void Start()
    {
        InitilizeLabelUpdates();
    }

    //initilizes the labels
    private void InitilizeLabelUpdates()
    {
        if(_sliderLabelPairs != null)
        {
            for (int i = 0; i < _sliderLabelPairs.Length; i++)
            {
                _sliderLabelPairs[i].InitilizeUpdates();
            }
        }
    }
}

//individual label
[System.Serializable]
public class SliderTextInfo
{
    public Slider slider;
    public Text textElement;
    public string baseText;

    public void InitilizeUpdates()
    {
        //subscribes the text update funtion to the slider
        if(slider != null)
        {
            slider.onValueChanged.AddListener(TextUpdate);
            TextUpdate(slider.value); //refreshes the text initialy
        }
    }

    //function that updates the text display
    private void TextUpdate(float value)
    {
        if(textElement != null && slider != null)
        {
            textElement.text = baseText + Mathf.Round(value).ToString();
        }
    }
}