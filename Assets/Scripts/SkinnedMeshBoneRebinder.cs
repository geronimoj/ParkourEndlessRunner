using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkinnedMeshBoneRebinder : MonoBehaviour
{
    /// <summary>
    /// Swaps the model of avatar, being an avatar to the model of the avatar 'targetModel'
    /// The bone heirachies of both avatars should be identical in name with the target being allowed a suffix
    /// </summary>
    /// <param name="avatar">The players avatar.</param>
    /// <param name="rootBone">The root bone of the players avatar.</param>
    /// <param name="targetModel">The target avatar who you want to take the model from</param>
    /// <param name="targetRootBone">The root bone of the target avatar</param>
    /// <param name="targetBoneSuffix">The suffix on the name of targetModels bone heirachy</param>
    /// <returns>Returns false if it fails to swap models</returns>
    public static bool SwapModel(GameObject avatar, Transform rootBone, GameObject targetModel, string targetBoneSuffix = "")
    {
        if (avatar == null || rootBone == null || targetModel == null)
            return false;
        //Get the skinned mesh renderer of the target model
        SkinnedMeshRenderer[] targetRenderer = targetModel.GetComponentsInChildren<SkinnedMeshRenderer>();
        //We get the avatars models which we use to know where to store the new mesh renderers and which skinnedMeshRenderers to delete
        List<SkinnedMeshRenderer> avatarRenderer = new List<SkinnedMeshRenderer>(avatar.GetComponentsInChildren<SkinnedMeshRenderer>());
        //If we have nothing to copy, don't do anything
        if (targetRenderer.Length == 0)
            return false;

        //We can re-use some of the avatarRenderers
        if (targetRenderer.Length < avatarRenderer.Count)
        {   //Delete the un-necessary renderers
            float count = avatarRenderer.Count;
            for (int i = 0; i < count - targetRenderer.Length; i++)
            {   //Delete the skinned mesh renderers gameObjects at the front since we don't need them
                DestroyImmediate(avatarRenderer[0].gameObject);
                avatarRenderer.RemoveAt(0);
            }
        }    
        else if (targetRenderer.Length > avatarRenderer.Count)
        {   //We need to create more renderers
            for (int i = 0; i < targetRenderer.Length - avatarRenderer.Count; i++)
            {   //Create an empty game object
                GameObject obj = Instantiate(new GameObject(), avatarRenderer[0].transform.parent);
                //Add the skinned mesh renderer and put it in the avatars renderers
                SkinnedMeshRenderer sm = obj.AddComponent<SkinnedMeshRenderer>();
                avatarRenderer.Add(sm);
            }
        }
        //If neither case got hit, that means they are already equal length
        //Step 2 is to copy over the necessary information except the bones.
        for (int renderer = 0; renderer < targetRenderer.Length; renderer++)
        {   //Copy over the mesh
            avatarRenderer[renderer].sharedMesh = targetRenderer[renderer].sharedMesh;
            //Copy over the bounds
            avatarRenderer[renderer].localBounds = targetRenderer[renderer].localBounds;
            //Copy over the material
            Material[] mats = new Material[targetRenderer[renderer].sharedMaterials.Length];
            //This isn't particularly efficient but it SHOULD work
            int index = 0;
            foreach (Material m in targetRenderer[renderer].sharedMaterials)
            {   //Copy over the information
                mats[index] = new Material(m);
                index++;
            }
            //Store the infromation on the renderer
            avatarRenderer[renderer].materials = mats;
            //We might as well make the bones array the same length while we are at it.
            avatarRenderer[renderer].bones = new Transform[targetRenderer[renderer].bones.Length];

        }

        //Step 3: loop over the bones of the target renderer and copy the bone locations over relative to our root bone using their names
        string name;

        for (int rend = 0; rend < targetRenderer.Length; rend++)
        {
            Transform[] tBones = targetRenderer[rend].bones;
            Transform[] aBones = avatarRenderer[rend].bones;
            for (int bone = 0; bone < tBones.Length; bone++)
            {   //Get the name without the suffix
                name = tBones[bone].name;
                if (name.StartsWith(targetBoneSuffix))
                    name = name.Remove(0, targetBoneSuffix.Length);
                //Loop over all transforms the player has and check if they are valid.
                foreach (Transform t in rootBone.GetComponentsInChildren<Transform>())
                {
                    //Otherwise time to loop through the transforms
                    if (name == t.name)
                    {
                        aBones[bone] = t;
                        //Update the transform position of our bones to match theirs
                        t.localPosition = tBones[bone].localPosition;

                        BoneRotationFixer brf = t.GetComponent<BoneRotationFixer>();
                        if (brf == null)
                            brf = t.gameObject.AddComponent<BoneRotationFixer>();

                        //brf.localRotationOffset = Vector3.zero - tBones[bone].localEulerAngles;

                        continue;
                    }
                }
            }

            avatarRenderer[rend].bones = aBones;
        }

        Avatar av = avatar.GetComponentInChildren<Animator>().avatar;
        HumanDescription h = av.humanDescription;
        //Loop over and fix the names of all the bones
        for (int i = 0; i < h.human.Length; i++)
        {
            HumanBone hb = h.human[i];

            if (hb.boneName.StartsWith(targetBoneSuffix))
                hb.boneName = hb.boneName.Remove(0, targetBoneSuffix.Length);

            if (hb.boneName.Contains("Ch24_nonPBR(Clone)"))
                Debug.LogError("Found it");

            if (hb.humanName.Contains("Ch24_nonPBR(Clone)"))
                Debug.LogError("Found it");

            h.human[i] = hb;
        }
        List<SkeletonBone> skBs = new List<SkeletonBone>();
        //h.skeleton[0].name = rootBone.parent.name;
        ////Repeat for skeleton

        foreach(Transform t in rootBone.parent.GetComponentsInChildren<Transform>())
        {
            SkeletonBone sb;
            sb.name = t.name;
            sb.position = t.localPosition;
            sb.scale = t.localScale;
            sb.rotation = t.localRotation;
            if (sb.name.Contains("Ch24_nonPBR(Clone)"))
                Debug.LogError("Found it");
            if (sb.name.Contains("Hips"))
                break;
            skBs.Add(sb);
        }
        bool foundStart = false;
        //Loop over the skeleton bones and only add the ones that weren't already added.
        for (int i = 0; i < h.skeleton.Length; i++)
        {
            SkeletonBone sb = h.skeleton[i];

            if (!foundStart && !sb.name.Contains("Hips"))
                continue;
            else if (!foundStart && sb.name.Contains("Hips"))
                foundStart = true;

            if (sb.name.StartsWith(targetBoneSuffix))
                sb.name = sb.name.Remove(0, targetBoneSuffix.Length);
            if (sb.name.Contains("Ch24_nonPBR(Clone)"))
                Debug.LogError("Found it");

            skBs.Add(sb);
        }
        {
            SkeletonBone sb;
            sb.name = "Neck1";
            sb.position = Vector3.zero;
            sb.scale = new Vector3(1, 1, 1);
            sb.rotation = Quaternion.identity;
            if (sb.name.Contains("Ch24_nonPBR(Clone)"))
                Debug.LogError("Found it");
            skBs.Add(sb);
        }
        h.skeleton = skBs.ToArray();

        av = AvatarBuilder.BuildHumanAvatar(rootBone.parent.gameObject, h);
        if (av.isValid)
            avatar.GetComponentInChildren<Animator>().avatar = av;
        

        return true;
    }
}
