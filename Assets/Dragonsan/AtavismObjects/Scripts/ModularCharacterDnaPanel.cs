using System.Collections;
using System.Collections.Generic;
using HNGamers;
using UnityEngine;

public class ModularCharacterDnaPanel : MonoBehaviour
{
    public GameObject dnaSliderPrefab;
    public GameObject dnaColorPrefab;
    public GameObject ColorPickerPrefab;
    public RectTransform pickerLoc;
    public List<string> dnaSliders = new List<string>();
    public List<string> dnaSlidersNames = new List<string>();
    public List<string> dnaColors = new List<string>();
    public List<string> dnaColorNames = new List<string>();
    public List<ModularCharacterSlider> createdSliders = new List<ModularCharacterSlider>();
    public bool headPanel;

    public void ClearExistingSliders()
    {
        foreach (var slider in createdSliders)
        {
            if (slider != null)
                Destroy(slider.gameObject);
        }
        createdSliders.Clear();
    }

    public void CreateMorphShapeSliders(MorphShapesManager morphShapesManager)
    {
        ClearExistingSliders();

        foreach (var morphShapeGameObject in morphShapesManager.morphShapeObjects)
        {
            foreach (var morphShapeValue in morphShapeGameObject.morphShapeValues)
            {
                // Check conditions based on headPanel and isHeadItem
                if (morphShapeValue.isVisible &&
                    ((headPanel && morphShapeValue.isHeadItem) || // If this is a head panel, add only head items
                     (!headPanel && !morphShapeValue.isHeadItem))) // If this is not a head panel, ignore head items
                {
                    // Instantiate a new slider prefab
                    ModularCharacterSlider slider = GameObject.Instantiate(dnaSliderPrefab, transform).GetComponent<ModularCharacterSlider>();
                    slider.label.text = morphShapeValue.displayName;
                    slider.dna = "BlendShape"; // Or any other appropriate identifier
                    slider.morphObjName = morphShapeGameObject.objectName;
                    slider.slider.value = morphShapeValue.currentShapeValue;
                    slider.slider.minValue = morphShapeValue.minAllowedValue;
                    slider.slider.maxValue = morphShapeValue.maxAllowedValue;
                    slider.Assign();

                    createdSliders.Add(slider); // Store reference to the slider
                }
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < dnaSliders.Count; i++)
        {
            ModularCharacterSlider slider = GameObject.Instantiate(dnaSliderPrefab, transform).GetComponent<ModularCharacterSlider>();
#if AT_I2LOC_PRESET
                slider.label.text = I2.Loc.LocalizationManager.GetTranslation(dnaSlidersNames[i]);
#else
            slider.label.text = dnaSlidersNames[i];
#endif
            slider.label.text = dnaSlidersNames[i];
            slider.dna = dnaSliders[i];
            slider.Assign();
        }
        
        for (int i = 0; i < dnaColors.Count; i++)
        {
            ModularCharacterColor color = GameObject.Instantiate(dnaColorPrefab, transform).GetComponent<ModularCharacterColor>();
#if AT_I2LOC_PRESET
                color.label.text = I2.Loc.LocalizationManager.GetTranslation(dnaColorNames[i]);
#else
            color.label.text = dnaColorNames[i];
#endif
             color.dna = dnaColors[i];
            if (ColorPickerPrefab && pickerLoc)
            {
                GameObject picker = GameObject.Instantiate(ColorPickerPrefab, pickerLoc);
                color.picker = picker.GetComponent<AtavismPicker>().picker;
                color.pickerCanvas = picker.GetComponent<Canvas>();
                color.pickerCanvas.enabled = false;
            }
            color.Assign();
        }

    }

    
}
