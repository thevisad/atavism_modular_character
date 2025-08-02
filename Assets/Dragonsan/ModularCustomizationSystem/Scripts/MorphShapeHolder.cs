using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Data", menuName = "HNGamers/MorphShapeHolder", order = 800)]

public class MorphShapeHolder : ScriptableObject
{
    public List<MorphShapeItem> blendShapeItem;
}