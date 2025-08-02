using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "HNGamers/MorphShapeItem", order = 800)]

public class MorphShapeItem : ScriptableObject
{
    public long BlendShapeMobID;
    public TextAsset BlendShapeValue;
}
