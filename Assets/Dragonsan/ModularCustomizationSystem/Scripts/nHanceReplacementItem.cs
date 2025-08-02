using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "Data", menuName = "HNGamers/nHanceReplacementItem", order = 800)]
[Obsolete("nHanceReplacementItem is deprecated, please use SkinnedReplacementItem instead.")]
public class nHanceReplacementItem : ScriptableObject
{
    public GameObject modelReplacement;
    public Material modelMaterial;
}
