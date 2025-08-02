using System.Collections.Generic;
using UnityEngine;

namespace HNGamers
{
    [System.Serializable]
    public class MorphShapeGameObject
    {
        public string objectName;
        public GameObject gameObjectRef;
        public SkinnedMeshRenderer meshRenderer;
        public int numVisibleShapes = 0;
        public List<MorphShapeValue> morphShapeValues = new List<MorphShapeValue>();
        [HideInInspector] public bool displayValues = false;
    }
}