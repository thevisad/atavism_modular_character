using System;
using UnityEngine;
using System.Collections.Generic;

namespace HNGamers
{
    public class MorphShapesManager : MonoBehaviour
    {
        public List<MorphShapeGameObject> morphShapeObjects = new List<MorphShapeGameObject>();
        [HideInInspector]
        public bool limitRespect = true;
        [HideInInspector]
        [Range(0.0f, 1f)]
        public float globalMultiplier = 1f;
        [HideInInspector]
        public string blendShapePrimaryPrefix = "SFB_BS_";
        [HideInInspector]
        public string blendShapeMatchPrefix = "SFB_BSM_";
        public (string, string) plusMinus = ("Plus", "Minus");

        // Dictionary to store initial blendshape values
        private Dictionary<string, float> initialBlendShapeValues = new Dictionary<string, float>();

        [HideInInspector] public List<MorphShapeMatchList> matchList = new List<MorphShapeMatchList>();
        [HideInInspector] public string[] matchListNames;
        [HideInInspector] public int matchListIndex = 0;
        public float globalModifier = 1f;
        public MorphShapeHolder morphShapeHolder;

        private void OnEnable()
        {
            BuildMorphShapeMatchList();
            //InitializeMorphShapes();
            SaveInitialBlendShapeValues();
        }

        private void SaveInitialBlendShapeValues()
        {
            foreach (var morphShapeObject in morphShapeObjects)
            {
                foreach (var morphShapeValue in morphShapeObject.morphShapeValues)
                {
                    string key = morphShapeObject.objectName + "_" + morphShapeValue.name;
                    if (!initialBlendShapeValues.ContainsKey(key))
                    {
                        initialBlendShapeValues.Add(key, morphShapeValue.currentShapeValue);
                    }
                }
            }
        }

        public void ParseAndResetBlendShapes(string dataString)
        {
            // Initialize a list to hold the extracted blend shape names
            List<string> blendShapeNames = new List<string>();

            // Split the input string into parts, skipping the first part ("HNGamers_MorphManager_Ranges")
            string[] parts = dataString.Split(',');
            for (int i = 1; i < parts.Length; i += 6) // Increment by 6 to jump to the next blend shape name
            {
                // The blend shape name is at the second position in each set of 6 parts (i.e., index 2, 8, 14, ...)
                string blendShapeName = parts[i + 1];
                if (!blendShapeNames.Contains(blendShapeName))
                {
                    blendShapeNames.Add(blendShapeName);
                }
            }

            // Now that we have all blend shape names, call ResetBlendShapesToInitialValues
            ResetBlendShapesToInitialValues(blendShapeNames);
        }


        public void ResetBlendShapesToInitialValues(List<string> blendShapeNames)
        {
            foreach (var morphShapeObject in morphShapeObjects)
            {
                foreach (var morphShapeValue in morphShapeObject.morphShapeValues)
                {
                    string key = morphShapeObject.objectName + "_" + morphShapeValue.name;
                    if (blendShapeNames.Contains(morphShapeValue.activationName) && initialBlendShapeValues.TryGetValue(key, out float initialValue))
                    {
                        morphShapeValue.currentShapeValue = initialValue;
                        UpdateMorphShape(morphShapeObject, morphShapeValue, initialValue);
                    }
                }
            }
        }

        public void BuildMorphShapeMatchList()
        {
            matchList.Clear();
            matchListIndex = 0;

            for (int o = 0; o < morphShapeObjects.Count; o++)
            {
                MorphShapeGameObject obj = morphShapeObjects[o];
                if (obj.numVisibleShapes == 0)
                    continue;
                if (!obj.gameObjectRef)
                    continue;

                for (int i = 0; i < obj.morphShapeValues.Count; i++)
                {
                    MorphShapeValue value = obj.morphShapeValues[i];
                    if (!value.isVisible)
                        continue;
                    if (value.isExpanded)
                        continue;
                    if (value.controlledShapes.Count > 0)
                        continue;

                    matchList.Add(new MorphShapeMatchList());
                    MorphShapeMatchList newMatchlist = matchList[matchList.Count - 1];

                    newMatchlist.name = obj.gameObjectRef.name + " " + value.activationName;
                    newMatchlist.objectIndex = o;
                    newMatchlist.valueIndex = i;

                    SetMatchShape(value, newMatchlist);
                }
            }

            matchListNames = new string[matchList.Count];
            for (int i = 0; i < matchList.Count; i++)
            {
                matchListNames[i] = matchList[i].name;
            }

        }

        private void SetMatchShape(MorphShapeValue value, MorphShapeMatchList matchList)
        {
            MorphShapeValue matchedValue = morphShapeObjects[matchList.objectIndex].morphShapeValues[matchList.valueIndex];

            if (value == matchedValue)
            {
                return;
            }

            if (matchedValue.controlledShapes.Count > 0)
            {
                return;
            }

            value.controlledShapes.Add(new ControlledShape());
            ControlledShape newMatchValue = value.controlledShapes[^1];
            newMatchValue.objectIndex = matchList.objectIndex;
            newMatchValue.valueIndex = matchList.valueIndex;

            matchedValue.controllerObjectIndex = value.parentObjectIndex;
            matchedValue.controllerValueIndex = value.shapeIndex;
            matchedValue.isControlledByOther = true;
            
            TriggerMorphShape(morphShapeObjects[value.parentObjectIndex], value);
        }

        public void TriggerMorphShape(MorphShapeGameObject obj, MorphShapeValue value, bool triggerAutoMatches = true, bool triggerUserMatches = true)
        {
            if (!obj.gameObjectRef) 
                return;
            if (!obj.meshRenderer)
                return;
            if (!obj.meshRenderer.sharedMesh) return;

            var targetShapeIndex = obj.meshRenderer.sharedMesh.GetBlendShapeIndex(value.completeName);
            if (targetShapeIndex == -1) return;
            obj.meshRenderer.SetBlendShapeWeight(targetShapeIndex, GlobalModifiedValue(value.currentShapeValue));

            if (triggerAutoMatches)
                TriggerMorphShapeAutoMatches(value.activationName, value.currentShapeValue);
        }
        
        public void TriggerMorphShapeAutoMatches(string triggerName, float value)
        {
            for (int o = 0; o < morphShapeObjects.Count; o++)
            {
                for (int i = 0; i < morphShapeObjects[o].morphShapeValues.Count; i++)
                {
                    if (morphShapeObjects[o] == null) continue;
                    if (morphShapeObjects[o].morphShapeValues[i].activationName == triggerName)
                    {
                        if (morphShapeObjects[o].morphShapeValues[i] == null) continue;
                        morphShapeObjects[o].morphShapeValues[i].currentShapeValue = morphShapeObjects[o].morphShapeValues[i].isNegativeShape ? -value : value;
                        TriggerMorphShape(morphShapeObjects[o], morphShapeObjects[o].morphShapeValues[i], false, false);
                    }
                }
            }
        }

        private float GlobalModifiedValue(float value) => value * globalModifier;

        public void UpdateMorphShape(MorphShapeGameObject morphObj, MorphShapeValue morphValue, float newValue)
        {
            if (!morphObj.gameObjectRef || !morphObj.meshRenderer) return;

            var shapeIndex = morphObj.meshRenderer.sharedMesh.GetBlendShapeIndex(morphValue.completeName);
            if (shapeIndex == -1) return;

            float adjustedValue = newValue * globalMultiplier;
            morphObj.meshRenderer.SetBlendShapeWeight(shapeIndex, adjustedValue);
            morphValue.currentShapeValue = adjustedValue;

            if (limitRespect)
            {
                TriggerLinkedShapes(morphObj, morphValue);
            }
        }

        public void SetMorphShapeValues()
        {
            for (int o = 0; o < morphShapeObjects.Count; o++)
            {
                MorphShapeGameObject obj = morphShapeObjects[o];
                if (obj.numVisibleShapes > 0)
                {
                    for (int i = 0; i < obj.morphShapeValues.Count; i++)
                    {
                        MorphShapeValue value = obj.morphShapeValues[i];
                        if (value.isVisible && !value.isControlledByOther)
                        {
                            TriggerMorphShape(obj, value, true, true);
                        }
                    }
                }
            }
        }

        public void UpdateMorphShapeValue(string blendShapeName, float newValue, string morphGameObject)
        {
            MorphShapeGameObject obj = GetMorphShapeGameObject(morphGameObject);
            if (obj != null)
            {
                foreach (var value in obj.morphShapeValues)
                {
                    if (value.activationName == blendShapeName)
                    {
                        value.previousValue = value.currentShapeValue;
                        value.currentShapeValue = newValue;
                        TriggerMorphShape(obj, value, true, true);
                        break;
                    }
                }
            }
        }

        public MorphShapeGameObject GetMorphShapeGameObject(string name)
        {
            foreach (var obj in morphShapeObjects)
            {
                if (obj.objectName == name)
                    return obj;
            }
            Debug.LogWarning("MorphShapeObject not found: " + name);
            return null;
        }

       
        public void InitializeMorphShapes()
        {
            morphShapeObjects.Clear();
            SkinnedMeshRenderer[] renderers = GetComponentsInChildren<SkinnedMeshRenderer>();

            foreach (var renderer in renderers)
            {
                MorphShapeGameObject morphObj = new MorphShapeGameObject
                {
                    gameObjectRef = renderer.gameObject,
                    meshRenderer = renderer,
                    objectName = renderer.gameObject.name,
                    morphShapeValues = new List<MorphShapeValue>()
                };

                for (int i = 0; i < renderer.sharedMesh.blendShapeCount; i++)
                {
                    string shapeName = renderer.sharedMesh.GetBlendShapeName(i);
                    bool isNegative = !shapeName.Contains(plusMinus.Item2); // Check if it's a negative blend shape

                    if (isNegative)
                    {
                        morphObj.numVisibleShapes++;
                    }

                    bool isNegativeShape = shapeName.Contains(plusMinus.Item2);

                    float minShapeValue = 0f;
                    float minAllowedValue = 0f;
                    if (shapeName.Contains(plusMinus.Item2) || shapeName.Contains(plusMinus.Item1))
                    {
                        minShapeValue = -100f;
                        minAllowedValue = -100f;
                    }

                    morphObj.morphShapeValues.Add(new MorphShapeValue
                    {
                        name = shapeName,
                        displayName = GetHumanReadableName(shapeName),
                        completeName = shapeName,
                        isVisible = isNegative,
                        activationName = GetHumanReadableName(shapeName),
                        currentShapeValue = 0, // Initial value
                        maxAllowedValue = 100,
                        minShapeValue = minShapeValue,
                        minAllowedValue = minAllowedValue,
                        isNegativeShape = isNegativeShape

                    });

                }
                morphShapeObjects.Add(morphObj);
            }
        }


        private string GetHumanReadableName(string blendShapeName)
        {
            if (!blendShapeName.Contains(blendShapePrimaryPrefix) && !blendShapeName.Contains(blendShapeMatchPrefix))
                return "";

            string[] parts = blendShapeName.Split('_');
            string humanReadableName = parts[parts.Length - 1].Replace(plusMinus.Item1, "").Replace(plusMinus.Item2, "");
            return humanReadableName;
        }

        private void TriggerLinkedShapes(MorphShapeGameObject morphObj, MorphShapeValue morphValue)
        {
            foreach (var linkedShape in morphValue.controlledShapes)
            {
                var linkedObj = morphShapeObjects[linkedShape.objectIndex];
                var linkedValue = linkedObj.morphShapeValues[linkedShape.valueIndex];

                float finalValue = CalculateControlledShapeValue(morphValue, linkedValue);
                UpdateMorphShape(linkedObj, linkedValue, finalValue);
            }
        }

        private float CalculateControlledShapeValue(MorphShapeValue controllingValue, MorphShapeValue controlledValue)
        {
            float range = controlledValue.maxAllowedValue - controlledValue.minAllowedValue;
            float proportion = (controllingValue.currentShapeValue - controllingValue.minShapeValue) / (controllingValue.maxShapeValue - controllingValue.minShapeValue);
            return controlledValue.minAllowedValue + range * proportion;
        }

        public void LoadRangeFile(TextAsset rangeFile)
        {
            string contents = rangeFile.text;

            if (ImportMorphFromTextAsset(rangeFile))
                return;
        }

        public string ReturnMorphShapeDataString()
        {
            string exportString = "HNGamers_MorphManager_Ranges";

            for (int o = 0; o < morphShapeObjects.Count; o++)
            {
                MorphShapeGameObject obj = morphShapeObjects[o];
                for (int i = 0; i < obj.morphShapeValues.Count; i++)
                {
                    MorphShapeValue value = obj.morphShapeValues[i];
                    if (value.isVisible)
                    {
                        // Check if the currentShapeValue is within the allowed range
                        float shapeValue = (value.currentShapeValue >= value.minAllowedValue && value.currentShapeValue <= value.maxAllowedValue) ? value.currentShapeValue : 0;
                        exportString += "," + obj.objectName + "," + value.completeName + "," + value.minAllowedValue + "," + value.maxAllowedValue + "," + shapeValue;
                    }
                }
            }

            return exportString;
        }

        public bool ImportMorphFromTextAsset(TextAsset textAsset)
        {
            string[] splitText = textAsset.text.Split(new string[] { "," }, System.StringSplitOptions.None);
            if (splitText[0] == "HNGamers_MorphManager_Ranges")
            {
                for (int i = 1; i < splitText.Length; i += 5)
                {
                    string gameObjectName = splitText[i];
                    string fullName = splitText[i + 1];
                    string triggerName = fullName.Replace(plusMinus.Item1, "").Replace(blendShapePrimaryPrefix, "");

                    if (!float.TryParse(splitText[i + 2], out float minLimit) ||
                        !float.TryParse(splitText[i + 3], out float maxLimit) ||
                        !float.TryParse(splitText[i + 4], out float currentAmount))
                    {
                        Debug.LogError($"Invalid format for range or current amount values: {splitText[i + 2]}, {splitText[i + 3]}, {splitText[i + 4]}");
                        continue;
                    }

                    var dataAttempt = TryGetMorphData(gameObjectName, triggerName);
                    MorphShapeGameObject obj = dataAttempt.Item1;
                    MorphShapeValue value = dataAttempt.Item2;

                    if (obj != null && value != null)
                    {
                        // Directly updating the existing MorphShapeValue
                        value.minAllowedValue = minLimit;
                        value.maxAllowedValue = maxLimit;
                        value.currentShapeValue = currentAmount;

                        // Applying changes to the morph shape
                        TriggerMorphShape(obj, value, true, true);
                    }
                    else
                    {
                        Debug.LogWarning($"MorphShapeGameObject or MorphShapeValue not found for: {gameObjectName}, {triggerName}");
                    }
                }
                return true;
            }
            else
            {
                Debug.LogWarning("Invalid file format for morph shape data.");
            }
            return false;
        }

        public (MorphShapeGameObject, MorphShapeValue) TryGetMorphData(string objGameObjectName, string triggerName)
        {
            MorphShapeGameObject obj = GetMorphShapeGameObject(objGameObjectName);

            if (obj != null)
            {
                foreach (MorphShapeValue thisValue in obj.morphShapeValues)
                {
                    if (thisValue.isVisible && thisValue.activationName == triggerName)
                    {
                        return (obj, thisValue);
                    }
                }
            }

            return (null, null); // Return null for both MorphShapeGameObject and MorphShapeValue if not found
        }
    }
}
