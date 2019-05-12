//ThickMod by Kaema aka Kerver [Bepinex port by Ryan Pallesen]
//Version 1.0.0
using BepInEx;
using RoR2;
using UnityEngine;
using System.Collections.Generic;
using System;
using BepInEx.Configuration;
using System.Reflection;
using MonoMod.Cil;
using KinematicCharacterController;
using System.Linq;
using EntityStates;
using R2API;

namespace Flood_Warning
{
    [BepInDependency("com.bepis.r2api")]

    [BepInPlugin("com.PallesenProductions.ThiccMod", "ThiccMod", "1.0.0")]

    public class ThiccMod : BaseUnityPlugin
    {

        bool isThicc = false;
        bool setupThicc = false;
        bool removeScarfAndSkirt = true;
        bool applyRuntimeMeshModification = true;
        bool mageMeshThicc = false;
        bool huntressMeshThicc = false;
        Vector3 thighPositionLeft = Vector3.zero;
        Vector3 thighPositionRight = Vector3.zero;
        Vector3 calfPositionLeft = Vector3.zero;
        Vector3 calfPositionRight = Vector3.zero;
        Vector3 pelvisPosition = Vector3.zero;

        public void Awake()//Code that runs when the game starts
        {

            On.RoR2.CharacterBody.Start += (orig, self) =>
            {
                orig(self);
                if (self.isPlayerControlled)
                {
                    Transform model = self.modelLocator.modelTransform;
                    bool isMage = model.Find("MageArmature"); // rigging on mage pelvis is bad, so apply pelvis offset if body is mage
                    Vector3 pelvisScale = new Vector3(1.25f, 1.25f, 1.25f);
                    Vector3 thighScale = new Vector3(1f, 1f, 1f);
                    thighScale = Vector3.Scale(thighScale, new Vector3(1f, 1f / pelvisScale.y, 1f));
                    Vector3 calfScale = new Vector3(1.1f, 1f, 1.1f); //new Vector3(1f / scale.x, 1f / scale.y, 1f / scale.z); // Preserve calf size. You can disable this if you want by changing it to Vector3.one.
                    Vector3 pelvisOffset = isMage ? new Vector3(0f, 0f, 0.05f) : Vector3.zero;
                    Vector3 thighOffset = Vector3.zero; // Manual slight adjustments to thigh offset. Calf height is preserved.
                    Vector3 calfOffset = Vector3.Scale(thighOffset, new Vector3(0f, -1f, 0f));
                    Vector3 mirror = new Vector3(-1f, 1f, 1f);
                    Transform armature = null;

                    for (int i = 0; i < model.childCount; i++)
                    {
                        Transform child = model.GetChild(i);
                        if (child.Find("ROOT"))
                        {
                            armature = child;
                            break;
                        }
                    }

                    if (armature)//if model has root child
                    {

                        Vector3 ikScale = new Vector3(pelvisScale.x, 1f, pelvisScale.z);
                        Transform ikLeft = armature.Find("ROOT/IKLegTarget.l");//Has Left leg
                        if (ikLeft)
                        {
                            ikLeft.localPosition = Vector3.Scale(ikLeft.localPosition, ikScale);
                        }

                        Transform ikRight = armature.Find("ROOT/IKLegTarget.r");//has Right Leg
                        if (ikRight)
                        {

                            ikRight.localPosition = Vector3.Scale(ikRight.localPosition, ikScale);
                        }

                        Transform pelvis = armature.Find("ROOT/base/pelvis");//has pelvis
                        if (pelvis)
                        {

                            if (!setupThicc)
                            {

                                pelvisPosition = pelvis.localPosition;
                            }
                            pelvis.localScale = pelvisScale;
                            pelvis.localPosition = pelvisPosition + pelvisOffset;
                            Transform thigh_r = pelvis.Find("thigh.r");
                            Transform thigh_l = pelvis.Find("thigh.l");
                            if (thigh_r)
                            {

                                thigh_r.localScale = thighScale;
                                Transform calf_r = thigh_r.Find("calf.r");
                                if (calf_r)
                                {

                                    calf_r.localScale = calfScale;
                                    if (!setupThicc)
                                    {

                                        thighPositionRight = thigh_r.localPosition;
                                        calfPositionRight = calf_r.localPosition;
                                    }
                                    calf_r.localPosition = calfPositionRight + calfOffset;
                                }
                                thigh_r.localPosition = thighPositionRight + thighOffset;
                            }
                            if (thigh_l)
                            {

                                thigh_l.localScale = thighScale;
                                Transform calf_l = thigh_l.Find("calf.l");
                                if (calf_l)
                                {

                                    calf_l.localScale = calfScale;
                                    if (!setupThicc)
                                    {

                                        thighPositionLeft = thigh_l.localPosition;
                                        calfPositionLeft = calf_l.localPosition;
                                    }
                                    calf_l.localPosition = calfPositionLeft + calfOffset;
                                }
                                thigh_l.localPosition = thighPositionLeft + Vector3.Scale(thighOffset, mirror);
                            }
                            setupThicc = true;

                        }
                    }


                    MakeThicc(self);


                };

            };

        }

        private void MakeThicc(CharacterBody self)
        {

            Transform model = self.modelLocator.modelTransform;
            Transform armature = null;
            for (int i = 0; i < model.childCount; i++)
            {
                Transform child = model.GetChild(i);
                if (child.Find("ROOT"))
                {

                    armature = child;
                    break;
                }
            }
            if (armature)
            {
                //The following are hacky methods used to detect the vertexes of the jets on the Mages back and the two things on the left and right of the Huntress' belt.
                //I cannot completely remove the Huntress belt, there is no geometry under it.
                //I could remove the Mages arm braces, but trust me her forearms are squeezed so thin it looks horrifying.
                Transform chest = armature.Find("ROOT/base/stomach/chest");
                Transform head = armature.Find("ROOT/base/stomach/chest/neck/head");
                Transform pelvis = armature.Find("ROOT/base/stomach/pelvis");
                Vector3 cullPos = new Vector3(float.NaN, float.NaN, float.NaN);
                if (model.Find("MageArmature"))
                {

                    if (removeScarfAndSkirt)
                    {

                        model.Find("MageCapeMesh").gameObject.SetActive(false);
                    }
                    if (applyRuntimeMeshModification)
                    { //Remove jets
                        for (int i = 0; i < chest.childCount; i++)
                        {
                            Transform child = chest.GetChild(i);
                            if (child.name == "Jets")
                            {

                                child.localScale = Vector3.zero;
                            }
                        }
                        if (!mageMeshThicc)
                        {


                            SkinnedMeshRenderer renderer = model.Find("MageMesh").GetComponent<SkinnedMeshRenderer>();
                            Vector3[] vertices = renderer.sharedMesh.vertices;
                            BoneWeight[] boneWeights = renderer.sharedMesh.boneWeights;
                            List<Vector2> uvs = new List<Vector2>();
                            renderer.sharedMesh.GetUVs(0, uvs);
                            float[] jetUvs = new float[]
                            {
                            0.155523732f,
                            0.161421865f,
                            0.04235405f,
                            0.00210663863f,
                            0.115523428f,
                            0.169746473f,
                            0.160440609f,
                            0.547309041f,
                            0.208297536f,
                            0.175094485f,
                            0.5473088f,
                            0.5701668f,
                            0.46927318f,
                            0.171110779f,
                            0.172749758f,
                            0.9341732f,
                            0.790172338f
                            };
                            for (int i = 0; i < vertices.Length; i++)
                            {
                                Vector2 uv = uvs[i];
                                BoneWeight weight = boneWeights[i];
                                if (weight.boneIndex0 == 3)
                                {
                                    for (int u = 0; u < jetUvs.Length; ++u)
                                    {
                                        if (uv.y < 0.3f || Mathf.Abs(uv.x - jetUvs[u]) < 0.0005f || Mathf.Abs(uv.y - jetUvs[u]) < 0.0005f)
                                        {
                                            vertices[i] = cullPos;
                                            break;
                                        }
                                    }
                                }
                            }
                            renderer.sharedMesh.vertices = vertices;
                            mageMeshThicc = true;
                        }
                    }
                }
                else if (model.Find("HuntressArmature"))
                {
                    if (removeScarfAndSkirt)
                    {

                        model.Find("HuntressScarfMesh").gameObject.SetActive(false);
                    }
                    if (!huntressMeshThicc && applyRuntimeMeshModification)
                    {


                        SkinnedMeshRenderer renderer = model.Find("HuntressMesh").GetComponent<SkinnedMeshRenderer>();
                        Vector3[] vertices = renderer.sharedMesh.vertices;
                        BoneWeight[] boneWeights = renderer.sharedMesh.boneWeights;
                        List<Vector2> uvs = new List<Vector2>();
                        List<Color32> colors = new List<Color32>();
                        Color32 black = new Color32(0, 0, 0, 255);
                        renderer.sharedMesh.GetColors(colors);
                        renderer.sharedMesh.GetUVs(0, uvs);
                        Vector2 uvCircleCenter = new Vector2(396.631653f / 512f, 324.391174f / 512f);
                        Vector3 chestCenter = new Vector3(0f, -0.044645f, 1.419872f);
                        Vector3 breastCenter = new Vector3(0.071101f, -0.146735f, 1.416376f);
                        for (int i = 0; i < vertices.Length; i++)
                        {
                            Vector2 uv = uvs[i];
                            BoneWeight weight = boneWeights[i];
                            if (weight.boneIndex0 == 39 & (uv.x > (343.022938f / 512f) || Mathf.Abs(uv.x - 307.158661f / 512f) < 0.0001f))
                            {
                                vertices[i] = cullPos;
                            }
                            else if (weight.boneIndex0 == 3)
                            {
                                Vector3 vertex = vertices[i];
                                float distance = Vector2.Distance(uv, uvCircleCenter);
                                if (distance < 0.05f)
                                {
                                    vertices[i] = cullPos;
                                    vertex = cullPos;
                                }
                                else if (uv.x == 0.5669036f || uv.x == 0.583716333f || uv.x == 0.6001201f || uv.x == 0.6146834f || uv.x == 0.6304007f)
                                {
                                    uvs[i] = new Vector2(326.118683f / 512f, 327.132751f / 512f);
                                }
                                if (Mathf.Abs(vertex.x) < 0.0765f && vertex.y < -0.0875f)
                                {
                                    bool center = vertex.x == 0f;
                                    if (center)
                                    {
                                        float zdis = 1f - Mathf.Min(1f, Mathf.Abs(vertex.z - 1.317341f) / 0.22f);
                                        vertices[i] = Vector3.Lerp(vertex, chestCenter, zdis * .4f);
                                        colors[i] = Color32.Lerp(colors[i], black, zdis);
                                    }
                                }
                            }
                        }
                        renderer.sharedMesh.vertices = vertices;
                        renderer.sharedMesh.SetUVs(0, uvs);
                        renderer.sharedMesh.SetColors(colors);
                        renderer.sharedMesh.RecalculateNormals();
                        huntressMeshThicc = true;
                    }
                }
                isThicc = true;
            }
        }

    }
}









//    // END
//    // END
//    // END


//    //FOR THIS: Right click CharacterBody.Start and click Edit Method (C#)
//    //Insert the 4 lines in this at the end of the function
//    private partial void Start()
//    {
//        if (this.isPlayerControlled)
//        {
//            this.MakeThicc();
//        }
//    }
//}
//    }
//}
