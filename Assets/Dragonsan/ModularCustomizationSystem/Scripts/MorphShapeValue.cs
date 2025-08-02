using System.Collections.Generic;
using UnityEngine;

namespace HNGamers
{



    [System.Serializable]
    public class ControlledShape
    {
        public int objectIndex;
        public int valueIndex;
    }

    [System.Serializable]
    public class MorphShapeMatchList
    {
        public string name;
        public int objectIndex;
        public int valueIndex;
    }

    [System.Serializable]
    public class MorphShapeValue
    {
        public string name;
        public string displayName;
        public int parentObjectIndex;
        public int shapeIndex;
        public bool isVisible = true;
        public bool isHeadItem = false;
        public string completeName;
        public string activationName;
        public bool isNegativeShape;

        public float currentShapeValue = 0f;
        public float minShapeValue = -100f;
        public float maxShapeValue = 100f;

        [HideInInspector] public float minAllowedValue = -100f;
        [HideInInspector] public float maxAllowedValue = 100f;

        [HideInInspector] public float previousValue = 0f;
        [HideInInspector] public bool showOptions = false;
        [HideInInspector] public bool isExpanded = false;

        public float StartValue { get; set; }
        public float EndValue { get; set; }

        public bool isControlledByOther;
        public int controllerObjectIndex = 0;
        public int controllerValueIndex = 0;

        public List<ControlledShape> controlledShapes = new List<ControlledShape>();
    }
}