using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "Data", menuName = "HNGamers/SkinnedReplacementItem", order = 800)]

public class SkinnedReplacementItem : ScriptableObject
{
    public GameObject modelReplacement;
    public Material modelMaterial;
}
