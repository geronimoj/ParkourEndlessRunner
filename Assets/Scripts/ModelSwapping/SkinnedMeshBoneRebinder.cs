using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkinnedMeshBoneRebinder : MonoBehaviour
{
    /// <summary>
    /// Swaps the model of avatar, being an avatar to the model of the avatar 'targetModel'
    /// The bone heirachies of both avatars should be identical in name with the target being allowed a suffix
    /// </summary>
    /// <param name="avatar">Contains the default avatar, reference to the players rootbone and reference to the players models</param>
    /// <param name="targetModel">The target model with its avatar</param>
    /// <returns>Returns false if it fails to swap models</returns>
    public static bool SwapModel(ModelInfo avatar, ModelInfo targetModel)
    {
        if (!avatar.IsValid || avatar.RootBone == null || !targetModel.IsValid)
            return false;
        //Get the skinned mesh renderer of the target model
        SkinnedMeshRenderer[] targetRenderer = targetModel.Model.GetComponentsInChildren<SkinnedMeshRenderer>();
        //We get the avatars models which we use to know where to store the new mesh renderers and which skinnedMeshRenderers to delete
        List<SkinnedMeshRenderer> avatarRenderer = new List<SkinnedMeshRenderer>(avatar.Model.GetComponentsInChildren<SkinnedMeshRenderer>());
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
            int count = avatarRenderer.Count;
            for (int i = 0; i < targetRenderer.Length - count; i++)
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
                int index = name.IndexOf(':');
                if (index != -1)
                    name = name.Remove(0, index + 1);
                //Loop over all transforms the player has and check if they are valid.
                foreach (Transform t in avatar.RootBone.GetComponentsInChildren<Transform>())
                {
                    //Otherwise time to loop through the transforms
                    if (name == t.name)
                    {
                        aBones[bone] = t;
                        continue;
                    }
                }
            }

            avatarRenderer[rend].bones = aBones;
        }

        HumanDescription h = targetModel.ModelAvatar.humanDescription;
        HumanDescription aH = avatar.ModelAvatar.humanDescription;
        //Loop over the human bone avatar description
        //And set to use default values to avoid having to copy over other stuff to make things easier
        for (int i = 0; i < aH.human.Length; i++)
        {
            aH.human[i].limit.useDefaultValues = true;
        }
        //Loop over the skeleton bones and compare identical bones, copying the values over
        List<SkeletonBone> aSkBones = new List<SkeletonBone>(aH.skeleton);

        for (int skBone = 0; skBone < aSkBones.Count; skBone++)
        {
            SkeletonBone a = aSkBones[skBone];

            foreach(SkeletonBone t in h.skeleton)
            {
                name = t.name;
                int index = name.IndexOf(':');
                if (index != -1)
                    name = name.Remove(0, index + 1);

                if (a.name == name)
                {
                    a.position = t.position;
                    a.rotation = t.rotation;
                    a.scale = t.scale;
                    break;
                }
            }

            aSkBones[skBone] = a;
        }
        aH.skeleton = aSkBones.ToArray();
        //Ensure the names are the same. Because of heirachy issues, we need to set the rootBones parents name simply because we can't change the parent naming heirachy in SkeletonBone struct
        avatar.Model.name = aH.skeleton[0].name;
        //Now that we have fixed up the human bones and skeleton bones we copy those two arrays into h specifically
        h.human = aH.human;
        h.skeleton = aH.skeleton;


        Avatar av = AvatarBuilder.BuildHumanAvatar(avatar.Model.gameObject, h);
        if (av.isValid)
            avatar.Model.GetComponentInChildren<Animator>().avatar = av;
        

        return true;
    }
}
