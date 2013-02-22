using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace com.funcom.legoxmlreader.modelviewer
{
    public class PartDatabase
    {
        // singletonisation...
        private static PartDatabase singleton;
        public static PartDatabase Singleton
        {
            get
            {
                if (singleton == null)
                {
                    singleton = new PartDatabase();
                }
                return singleton;
            }
        }
        public Vector3 modelOffsetPosition = Vector3.zero;

        private struct PartUsageItem
        {
            public int count;
            public bool noLbx;
        };

        private SortedDictionary<int, PartUsageItem> partUsageList = new SortedDictionary<int, PartUsageItem>();


        private Dictionary<int, Color> solidColors;
        private Dictionary<int, Color> transparentColors;
        private Shader overloadVertexLitShader;

        //key = designId, next key is a material id, then the value is a list of parts of this design ids with these materials and decorations.
        private Dictionary<string, List<Part>> parts = new Dictionary<string, List<Part>>();
        //private List<Bone> bones = new List<Bone>();
        //parts should be keyed on designID, material and decoration(s)
        //private  Dictionary<int,Dictionary<int,Dictionary<List<int>, db;

        private PartDatabase()
        {
            parts.Clear();
            this.overloadVertexLitShader = Shader.Find("OverloadD/VertexLit");
            solidColors = new Dictionary<int, Color>();
            transparentColors = new Dictionary<int, Color>();
            AddColors();
        }

        public List<Part> GetAllParts()
        {
            List<Part> allParts = new List<Part>();
            foreach (KeyValuePair<string, List<Part>> entry in parts)
            {
                foreach (Part p in entry.Value)
                {
                    allParts.Add(p);
                }
            }
            return allParts;
        }
        public void StartModelEntry()
        {
            setAllInUse(false);
        }

        public void EndModelEntry()
        {
            setAllInUse(false); // this looks strange but the building process sets appropriate parts as in use as the process progresses - this is a reset.
        }

        private void setAllInUse(bool pState)
        {
            Dictionary<string, List<Part>>.ValueCollection partLists = parts.Values;

            foreach (List<Part> partList in partLists)
            {
                foreach (Part p in partList)
                {
                    p.AlreadyInUse = pState;
                }
            }
        }

        public Part AddPart(int pDesignId, int pMaterialId, List<int> pDecorations, List<Matrix4x4> pBoneTransforms, string pPartRefId)
        {
            Part p = null;
            string partKey = MakePartKey(pDesignId, pMaterialId, pDecorations);
            List<Part> partsList;

            try
            {
                if (parts.TryGetValue(partKey, out partsList))
                {
                    //Kev: Seriously? Why would we want to re-use part of existing model!?
                    /*foreach(Part pt in partsList)
                    {
                        if(pt.AlreadyInUse == false)
                        {   // found an existing unused part that matches - must be from another model - use this
                            p = pt;
                            break;
                        }	
                    }*/

                    if (p == null)
                    {	// no unused part in the list for this type found - create a new part and add it to the list
                        p = CreateNewPart(pDesignId, pMaterialId, pDecorations, pBoneTransforms, pPartRefId);
                        p.AlreadyInUse = true;
                        parts[partKey].Add(p);
                    }
                    else
                    {	// unused part found - set it up with the appropriate info and add it
                        SetPartData(p, pBoneTransforms);
                        p.AlreadyInUse = true;
                    }
                }
                else
                {	// no part of this type - create a new part with appropriate bones
                    p = CreateNewPart(pDesignId, pMaterialId, pDecorations, pBoneTransforms, pPartRefId);
                    p.AlreadyInUse = true;
                    List<Part> pl = new List<Part>();
                    pl.Add(p);
                    parts.Add(partKey, pl);
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                p = null;
            }

            return p;
        }

        private void SetPartData(Part pPart, List<Matrix4x4> pBoneTransforms)
        {	// expand for flex  using bones					
            if (pBoneTransforms.Count == 1)
            {	// not a flex - just the one bone
                pPart.transform.position = GetPosition(pBoneTransforms[0]);
                pPart.transform.rotation = GetRotation(pBoneTransforms[0]);
            }
        }

        private Part CreateNewPart(int pDesignId, int pMaterialId, List<int> pDecorations, List<Matrix4x4> pBoneTransforms, string pPartRefId)
        {
            Part newPart = null;
            bool noLbx = true;

            GameObject prefab = (GameObject)Resources.Load("LEGO/Geometry/" + pDesignId.ToString(), typeof(GameObject));
            if (prefab != null)
            {
                noLbx = false;
                GameObject partGameObject = UnityEngine.Object.Instantiate(prefab) as GameObject;
                partGameObject.name = pPartRefId;
                UnityEngine.Object.Destroy(partGameObject.animation);

                if (pBoneTransforms.Count == 1)
                {	// not a flex - just the one bone
                    partGameObject.transform.position = GetPosition(pBoneTransforms[0]);
                    partGameObject.transform.rotation = GetRotation(pBoneTransforms[0]);
                    newPart = partGameObject.AddComponent<Part>() as Part;
                    newPart.SetData(pDesignId, pMaterialId, pDecorations);
                    CreateMaterial(partGameObject, pMaterialId);
                    AddDecorations(partGameObject, pDecorations);
                }
                //			else
                //			{ // flex - NOT YET SUPPORTED
                //				boneGameObject.transform.position = Vector3.zero;
                //				boneGameObject.transform.rotation = Quaternion.identity;
                //				for( int i=0; i < pBoneTransforms.Count; i++)
                //				{
                //					foreach(Transform child in boneGameObject.transform)
                //					{
                //						if(child.gameObject.name.Equals("Bone_" + Convert.ToString(i)))
                //						{	
                //							Bone newBone = boneGameObject.AddComponent<Bone>() as Bone;
                //							child.transform.position = GetPosition(pBoneTransforms[i]);
                //							child.transform.rotation = GetRotation(pBoneTransforms[i]);
                //							newPart.AddBone(newBone,i);							
                //							break;
                //						}
                //					}
                //				}
                //			}
                //			CreateMaterial(boneGameObject, pMaterialId);
                //			AddDecorations(boneGameObject, pDecorations);
            }
            else
            {
                Debug.LogWarning("Missing FBX -> " + pDesignId.ToString());
            }
            AddToPartUsageList(pDesignId, noLbx);
            return newPart;
        }

        private void CreateMaterial(GameObject obj, int matId)
        {
            Material newMat = null;

            if (solidColors.ContainsKey(matId))
            {
                newMat = new Material(Shader.Find("Specular"));
                newMat.color = solidColors[matId]; ;
                newMat.SetFloat("_Shininess", 0.2f);
            }
            else if (transparentColors.ContainsKey(matId))
            {
                //newMat = new Material(Shader.Find("Transparent/SpecularOverride"));
                newMat = new Material(Shader.Find("Transparent/Specular"));

                newMat.color = transparentColors[matId]; ;
            }
            else
            {
                return;
            }

            Renderer rootRenderer = obj.renderer;
            if (rootRenderer != null)
            {
                rootRenderer.material = newMat;
            }
            else
            {
                Transform t = obj.transform;
                foreach (Transform child in t)
                {
                    Renderer childRenderer = child.gameObject.renderer;
                    if (childRenderer != null)
                    {
                        if (!child.gameObject.name.StartsWith("Decoration"))
                        {
                            childRenderer.material = newMat;
                        }
                        else
                        {
                            childRenderer.enabled = false;
                        }
                    }
                }
            }
        }

        public void AddDecorations(GameObject decorateObject, List<int> pDecoIds)
        {

            for (int i = 0; i < pDecoIds.Count; i++)
            {
                foreach (Transform t in decorateObject.transform)
                {
                    if (t.gameObject.name.Equals("Decoration_" + Convert.ToString(i + 1)))
                    {
                        if (pDecoIds[i] != 0)
                        {
                            Texture2D decoTexture = (Texture2D)Resources.Load("Decorations/" + pDecoIds[i], typeof(Texture2D));
                            Renderer decoRenderer = t.gameObject.renderer;

                            if (decoTexture == null)
                            {
                                Debug.LogWarning("Missing decoration -> " + pDecoIds[i]);
                                //Debug.Break();
                            }
                            else if (decoRenderer)
                            {
                                decoTexture.wrapMode = TextureWrapMode.Clamp;

                                foreach (Transform cc in decorateObject.transform)
                                {
                                    if (cc.gameObject.name.Equals("Material_" + Convert.ToString(i + 1)))
                                    {
                                        Color prevColor = cc.gameObject.renderer.material.color;
                                        cc.gameObject.renderer.material = new Material(overloadVertexLitShader);
                                        cc.gameObject.renderer.material.color = prevColor;
                                    }
                                }

                                decoRenderer.enabled = true;
                                decoRenderer.material = new Material(Shader.Find("Transparent/Cutout/Diffuse"));
                                decoRenderer.material.SetFloat("_Cutoff", 0.25f);
                                decoRenderer.material.SetTexture("_MainTex", decoTexture);
                            }
                        }
                    }
                }
            }
        }

        private string MakePartKey(int pDesignId, int pMaterialId, List<int> pDecos)
        {
            string partKey = pDesignId.ToString() + "|" + pMaterialId.ToString() + "|";
            if (pDecos.Count > 0)
            {
                partKey += "<";
                for (int i = 0; i < pDecos.Count; i++)
                {
                    partKey += pDecos[i].ToString() + ((i == pDecos.Count - 1) ? ">" : ",");
                }
            }
            return partKey;
        }

        private Vector3 GetPosition(Matrix4x4 m)
        {
            return new Vector3(-m[3], m[7], m[11]);
        }

        private Quaternion GetRotation(Matrix4x4 a)
        {
            Quaternion q = new Quaternion();
            float trace = a[0, 0] + a[1, 1] + a[2, 2];
            if (trace > 0)
            {
                float s = 0.5f / Mathf.Sqrt(trace + 1.0f);
                q.w = 0.25f / s;
                q.x = (a[2, 1] - a[1, 2]) * s;
                q.y = (a[0, 2] - a[2, 0]) * s;
                q.z = (a[1, 0] - a[0, 1]) * s;
            }
            else
            {
                if (a[0, 0] > a[1, 1] && a[0, 0] > a[2, 2])
                {
                    float s = 2.0f * Mathf.Sqrt(1.0f + a[0, 0] - a[1, 1] - a[2, 2]);
                    q.w = (a[2, 1] - a[1, 2]) / s;
                    q.x = 0.25f * s;
                    q.y = (a[0, 1] + a[1, 0]) / s;
                    q.z = (a[0, 2] + a[2, 0]) / s;
                }
                else if (a[1, 1] > a[2, 2])
                {
                    float s = 2.0f * Mathf.Sqrt(1.0f + a[1, 1] - a[0, 0] - a[2, 2]);
                    q.w = (a[0, 2] - a[2, 0]) / s;
                    q.x = (a[0, 1] + a[1, 0]) / s;
                    q.y = 0.25f * s;
                    q.z = (a[1, 2] + a[2, 1]) / s;
                }
                else
                {
                    float s = 2.0f * Mathf.Sqrt(1.0f + a[2, 2] - a[0, 0] - a[1, 1]);
                    q.w = (a[1, 0] - a[0, 1]) / s;
                    q.x = (a[0, 2] + a[2, 0]) / s;
                    q.y = (a[1, 2] + a[2, 1]) / s;
                    q.z = 0.25f * s;
                }
            }

            q.x = -q.x;
            return q;
        }

        private void AddToPartUsageList(int pDesignId, bool pNoLbx)
        {
            PartUsageItem pui;
            if (partUsageList.TryGetValue(pDesignId, out pui))
            {	// design already exists
                pui.count++;
                pui.noLbx = pNoLbx;
            }
            else
            {	// no part of this design - add to db
                pui.count = 1;
                pui.noLbx = pNoLbx;
            }
            partUsageList[pDesignId] = pui;
        }

        public void WriteBrickUsage(string pFile)
        {
            //partUsageList
            using (StreamWriter sw = new StreamWriter(pFile))
            {
                sw.WriteLine("Design	Count	Lbx not found");
                foreach (KeyValuePair<int, PartUsageItem> pu in partUsageList)
                {
                    sw.WriteLine(pu.Key.ToString() + "	" + pu.Value.count + "	" + pu.Value.noLbx);
                }
            }
        }

        private void AddColors()
        {
            solidColors[1] = new Color(244.0f / 255.0f, 244.0f / 255.0f, 244.0f / 255.0f, 1);
            solidColors[5] = new Color(217.0f / 255.0f, 187.0f / 255.0f, 123.0f / 255.0f, 1);
            solidColors[18] = new Color(214.0f / 255.0f, 114.0f / 255.0f, 64.0f / 255.0f, 1);
            solidColors[21] = new Color(222.0f / 255.0f, 0.0f / 255.0f, 13.0f / 255.0f, 1);
            solidColors[23] = new Color(0.0f / 255.0f, 87.0f / 255.0f, 168.0f / 255.0f, 1);
            solidColors[24] = new Color(254.0f / 255.0f, 196.0f / 255.0f, 0.0f / 255.0f, 1);
            solidColors[26] = new Color(1.0f / 255.0f, 1.0f / 255.0f, 1.0f / 255.0f, 1);
            solidColors[28] = new Color(0.0f / 255.0f, 123.0f / 255.0f, 40.0f / 255.0f, 1);
            solidColors[37] = new Color(0.0f / 255.0f, 150.0f / 255.0f, 36.0f / 255.0f, 1);
            solidColors[38] = new Color(168.0f / 255.0f, 61.0f / 255.0f, 21.0f / 255.0f, 1);
            solidColors[42] = new Color(182.0f / 255.0f, 224.0f / 255.0f, 239.0f / 255.0f, 1);
            solidColors[102] = new Color(71.0f / 255.0f, 140.0f / 255.0f, 198.0f / 255.0f, 1);
            solidColors[106] = new Color(231.0f / 255.0f, 99.0f / 255.0f, 24.0f / 255.0f, 1);
            solidColors[113] = new Color(238.0f / 255.0f, 157.0f / 255.0f, 195.0f / 255.0f, 1);
            solidColors[119] = new Color(149.0f / 255.0f, 185.0f / 255.0f, 11.0f / 255.0f, 1);
            solidColors[120] = new Color(216.0f / 255.0f, 228.0f / 255.0f, 141.0f / 255.0f, 1);
            solidColors[124] = new Color(156.0f / 255.0f, 0.0f / 255.0f, 127.0f / 255.0f, 1);
            solidColors[131] = new Color(156.0f / 255.0f, 149.0f / 255.0f, 199.0f / 255.0f, 1);
            solidColors[315] = new Color(156.0f / 255.0f, 149.0f / 255.0f, 199.0f / 255.0f, 1); //silver metallic - wrong RGB
            solidColors[135] = new Color(94.0f / 255.0f, 116.0f / 255.0f, 140.0f / 255.0f, 1);
            solidColors[138] = new Color(141.0f / 255.0f, 116.0f / 255.0f, 82.0f / 255.0f, 1);
            solidColors[139] = new Color(116.0f / 255.0f, 73.0f / 255.0f, 48.0f / 255.0f, 1);
            solidColors[140] = new Color(0.0f / 255.0f, 37.0f / 255.0f, 65.0f / 255.0f, 1);
            solidColors[141] = new Color(0.0f / 255.0f, 52.0f / 255.0f, 22.0f / 255.0f, 1);
            solidColors[143] = new Color(202.0f / 255.0f, 227.0f / 255.0f, 246.0f / 255.0f, 1);
            solidColors[148] = new Color(73.0f / 255.0f, 63.0f / 255.0f, 59.0f / 255.0f, 1);
            solidColors[151] = new Color(95.0f / 255.0f, 130.0f / 255.0f, 101.0f / 255.0f, 1);
            solidColors[154] = new Color(128.0f / 255.0f, 8.0f / 255.0f, 27.0f / 255.0f, 1);
            solidColors[191] = new Color(244.0f / 255.0f, 155.0f / 255.0f, .0f / 255.0f, 1);
            solidColors[192] = new Color(98.0f / 255.0f, 28.0f / 255.0f, 12.0f / 255.0f, 1);
            solidColors[194] = new Color(150.0f / 255.0f, 150.0f / 255.0f, 150.0f / 255.0f, 1);
            solidColors[199] = new Color(76.0f / 255.0f, 81.0f / 255.0f, 86.0f / 255.0f, 1);
            solidColors[208] = new Color(228.0f / 255.0f, 228.0f / 255.0f, 218.0f / 255.0f, 1);
            solidColors[212] = new Color(135.0f / 255.0f, 192.0f / 255.0f, 234.0f / 255.0f, 1);
            solidColors[221] = new Color(222.0f / 255.0f, 55.0f / 255.0f, 139.0f / 255.0f, 1);
            solidColors[222] = new Color(238.0f / 255.0f, 157.0f / 255.0f, 195.0f / 255.0f, 1);
            solidColors[226] = new Color(255.0f / 255.0f, 228.0f / 255.0f, 104.0f / 255.0f, 1);
            solidColors[268] = new Color(44.0f / 255.0f, 21.0f / 255.0f, 119.0f / 255.0f, 1);
            solidColors[283] = new Color(245.0f / 255.0f, 193.0f / 255.0f, 137.0f / 255.0f, 1);
            solidColors[294] = new Color(254.0f / 255.0f, 252.0f / 255.0f, 213.0f / 255.0f, 1);
            solidColors[297] = new Color(170.0f / 255.0f, 127.0f / 255.0f, 46.0f / 255.0f, 1);
            solidColors[308] = new Color(53.0f / 255.0f, 33.0f / 255.0f, 0.0f / 255.0f, 1);
            solidColors[298] = new Color(118.0f / 255.0f, 118.0f / 255.0f, 118.0f / 255.0f, 1);

            transparentColors[40] = new Color(238.0f / 255.0f, 238.0f / 255.0f, 238.0f / 255.0f, 0.75f);
            transparentColors[41] = new Color(224.0f / 255.0f, 52.0f / 255.0f, 41.0f / 255.0f, 0.75f);
            transparentColors[42] = new Color(182.0f / 255.0f, 224.0f / 255.0f, 239.0f / 255.0f, 0.75f);
            transparentColors[43] = new Color(80.0f / 255.0f, 177.0f / 255.0f, 232.0f / 255.0f, 0.75f);
            transparentColors[44] = new Color(249.0f / 255.0f, 239.0f / 255.0f, 105.0f / 255.0f, 0.75f);
            transparentColors[47] = new Color(231.0f / 255.0f, 102.0f / 255.0f, 72.0f / 255.0f, 0.75f);
            transparentColors[48] = new Color(99.0f / 255.0f, 178.0f / 255.0f, 110.0f / 255.0f, 0.75f);
            transparentColors[49] = new Color(250.0f / 255.0f, 237.0f / 255.0f, 91.0f / 255.0f, 0.75f);
            transparentColors[126] = new Color(154.0f / 255.0f, 149.0f / 255.0f, 199.0f / 255.0f, 0.75f);
            transparentColors[182] = new Color(236.0f / 255.0f, 118.0f / 255.0f, 14.0f / 255.0f, 0.75f);
            transparentColors[111] = new Color(189.0f / 255.0f, 173.0f / 255.0f, 162.0f / 255.0f, .75f);
        }

        public void ShowContents()
        {//DEBUG
            Dictionary<string, List<Part>>.KeyCollection keyColl = parts.Keys;
            foreach (string s in keyColl)
            {
                Debug.Log("Key :" + s);
            }
        }
    }
}
