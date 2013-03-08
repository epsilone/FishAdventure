using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using com.funcom.legoxmlreader.modelviewer;
using UnityEngine;

public static class LegoScanner
{
    public static GameObject GetLegoObjectByLxfml(string lxfml, string gameObjectName)
    {
        //Create teh game object
        GameObject gameObject = new GameObject();
        gameObject.name = gameObjectName;
        Model model = gameObject.AddComponent("Model") as Model;

        //model.Clear();
        Resources.UnloadUnusedAssets();

        bool success = false;
        MemoryStream ms = new MemoryStream(new System.Text.UTF8Encoding().GetBytes(lxfml));

        //try
        {
            PartDatabase.Singleton.StartModelEntry();
            success = true;
            XmlTextReader reader = new XmlTextReader(ms);
            reader.WhitespaceHandling = WhitespaceHandling.None;
            Dictionary<List<int>, PartGroup> groupDraft = new Dictionary<List<int>, PartGroup>();

            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element && reader.Name.Equals("GroupSystems"))
                {
                    XmlReader groupSystemScopeReader = reader.ReadSubtree();
                    while (groupSystemScopeReader.Read())
                    {
                        if (groupSystemScopeReader.NodeType == XmlNodeType.Element && groupSystemScopeReader.Name.Equals("GroupSystem"))
                        {
                            XmlReader groupScopeReader = groupSystemScopeReader.ReadSubtree();
                            while (groupScopeReader.Read())
                            {
                                List<int> partRefsList = new List<int>();
                                string name = "";
                                if (groupScopeReader.NodeType == XmlNodeType.Element && groupScopeReader.Name.Equals("Group"))
                                {
                                    groupScopeReader.MoveToAttribute("name");
                                    name = groupScopeReader.Value;

                                    groupScopeReader.MoveToAttribute("partRefs");
                                    string[] partRefs = groupScopeReader.Value.Split(new char[] { ',' });

                                    foreach (string partRef in partRefs)
                                    {
                                        partRefsList.Add(Convert.ToInt32(partRef));
                                    }

                                    if (partRefsList.Count > 0)
                                    {
                                        groupDraft[partRefsList] = new PartGroup(partRefsList.Count, name);
                                    }
                                }
                            }
                        }
                    }
                }
            }

            ms.Position = 0;
            reader.ResetState();
            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element && reader.Name.Equals("Brick"))
                {
                    XmlReader partScopeReader = reader.ReadSubtree();
                    while (partScopeReader.Read())
                    {
                        if (partScopeReader.NodeType == XmlNodeType.Element && partScopeReader.Name.Equals("Part"))
                        {
                            List<string> decoStrings;
                            List<int> decoInts = new List<int>();

                            int designId = 0;
                            int materialId = 0;
                            string decoId = "";
                            string partRefId = "";
                            XmlReader boneScopeReader = partScopeReader.ReadSubtree();

                            while (partScopeReader.MoveToNextAttribute())
                            {
                                switch (reader.Name)
                                {
                                    case "designID": //key
                                        {
                                            designId = System.Convert.ToInt32(reader.Value);
                                        } break;

                                    case "materials": // key
                                        {
                                            if (reader.Value.IndexOf(',') != -1)
                                            {   // only the first material is used
                                                materialId = Mathf.Abs(Convert.ToInt16(reader.Value.Split(new char[] { ',' })[0]));
                                            }
                                            else
                                            {
                                                materialId = Mathf.Abs(Convert.ToInt16(reader.Value));
                                            }
                                        } break;
                                    case "decoration": //key
                                        {
                                            decoId = reader.Value;
                                            if (decoId.IndexOf(',') != -1)
                                            {
                                                decoStrings = new List<string>(decoId.Split(new char[] { ',' }));
                                            }
                                            else
                                            {
                                                decoStrings = new List<string>();
                                                decoStrings.Add(decoId);
                                            }
                                            foreach (string s in decoStrings)
                                            {
                                                decoInts.Add(Convert.ToInt32(s));
                                            }
                                        } break;

                                    case "refID":
                                        {
                                            partRefId = reader.Value;
                                        } break;

                                    default: break;
                                }
                            }

                            // Most of the end product of this goes into PartsDatabase
                            //////////////////////////////////////////////////////////////////////////////////////////
                            // we are dealing here with parts - or more accurately bones - that's where the transforms are contained.
                            // we need to create a bones structure or class, fill that with data from the xml file and pass a list
                            // of the bones to the PartsDatabase (maybe to be renamed as "visibleItemsDatabase?" - if there is one bone then
                            // creation of a copy of a prefab will be in the single part method, otherwise it'll be multiple bones and
                            // the flex brick creation mechanism. Flying stuff should be ok if the bones fly off whereever - we do not need
                            // to keep them as a coherent whole and move them as a group (working down the bones with local/world matrix positions etc.)
                            // - does not matter for this application.

                            // every visibleItem has a model transform (one for each model - contained in the models items db) and a transform in the
                            // visible item itself - its current position - and a calculated exploded position as before
                            // flying is done as before in the visible item itself.

                            // so, from the lxfml file we create a parts/bones list of transformations of the model assembled in the Model object,
                            // and if the part and its bone(s) have not been created we create an entry in the partsdatabase (with the current models transforms)
                            // after the model is assembled, we take a snapshot of it, then snapshots of any groups.
                            // then we move on to loading the next model.

                            // make a list of bones in this part (more their numbers and transforms) 1 = non-flex part
                            List<Matrix4x4> transformations = new List<Matrix4x4>();
                            Matrix4x4 transformation = new Matrix4x4();
                            while (boneScopeReader.Read())
                            {	//get the transformation(s) from the bone(s)
                                if (boneScopeReader.NodeType == XmlNodeType.Element
                                    && partScopeReader.Name.Equals("Bone"))
                                {
                                    boneScopeReader.MoveToAttribute("transformation");
                                    string[] stringTransformation = reader.Value.Split(new char[] { ',' });
                                    float[] mArr = new float[stringTransformation.Length];

                                    for (int m = 0; m < stringTransformation.Length; ++m)
                                    {
                                        mArr[m] = Convert.ToSingle(stringTransformation[m]);
                                    }

                                    for (int i = 0; i < 4; i++)
                                    {
                                        transformation.SetRow(i, new Vector4(mArr[i * 3], mArr[i * 3 + 1], mArr[i * 3 + 2], 0));
                                    }

                                    transformations.Add(transformation);
                                }
                            }
                            if (transformations.Count == 1)
                            {
                                Part p = PartDatabase.Singleton.AddPart(designId, materialId, decoInts, transformations, partRefId);
                                if (p != null)
                                {
                                    foreach (KeyValuePair<List<int>, PartGroup> entry in groupDraft)
                                    {
                                        if (entry.Key.Contains(Convert.ToInt32(partRefId)))
                                        {
                                            entry.Value.AddPart(p);
                                        }
                                    }
                                    model.AddPart(p);
                                }
                            }
                            else
                            {
                                Debug.Log("encountered flex/hinge brick NOT SUPPORTED YET");
                            }
                        }
                    }
                }
            }
            model.PartGroups = new List<PartGroup>(groupDraft.Values);
            model.PostLoadRoutine();
            PartDatabase.Singleton.EndModelEntry();
        }
        /*		catch(Exception e)
                {
                    Debug.LogError(e);
                    model.Clear();
                    success = false;
                }*/

        //PartDatabase.Singleton.ShowContents();
        return gameObject;
    }
}