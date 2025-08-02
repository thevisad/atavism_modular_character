using System.Collections.Generic;
using UnityEngine;

namespace HNGamers
{
    public class SkinnedEquipment : MonoBehaviour
    {
        public GameObject target;

        public void ProcessBones()
        {
            if (target == null) return;

            // Build the bone map from the target
            var boneMap = BuildBoneMap(target);

            // Process each SkinnedMeshRenderer in this GameObject
            var renderers = GetComponentsInChildren<SkinnedMeshRenderer>();
            foreach (var renderer in renderers)
            {
                MapRendererBones(renderer, boneMap);
            }
        }

        private Dictionary<string, Transform> BuildBoneMap(GameObject target)
        {
            var boneMap = new Dictionary<string, Transform>();
            var renderers = target.GetComponentsInChildren<SkinnedMeshRenderer>();

            foreach (var renderer in renderers)
            {
                foreach (var bone in renderer.bones)
                {
                    if (!boneMap.ContainsKey(bone.name))
                    {
                        boneMap[bone.name] = bone;
                    }
                }
            }

            return boneMap;
        }

        private void MapRendererBones(SkinnedMeshRenderer renderer, Dictionary<string, Transform> boneMap)
        {
            var newBones = new Transform[renderer.bones.Length];

            for (int i = 0; i < newBones.Length; i++)
            {
                var bone = renderer.bones[i];
                if (boneMap.TryGetValue(bone.name, out var newBone))
                {
                    newBones[i] = newBone;
                }
                else
                {
                    Debug.LogWarning($"Unable to map bone \"{bone.name}\" to target skeleton.");
                }
            }

            renderer.bones = newBones;

            if (renderer.rootBone != null && boneMap.TryGetValue(renderer.rootBone.name, out var newRootBone))
            {
                renderer.rootBone = newRootBone;
            }
            else
            {
                Debug.LogWarning($"Unable to map root bone \"{renderer.rootBone?.name}\" to target skeleton.");
            }

            renderer.updateWhenOffscreen = true;
        }
    }
}
