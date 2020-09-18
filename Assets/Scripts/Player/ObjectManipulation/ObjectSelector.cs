﻿using UnityEngine;
using System.Collections.Generic;
public class ObjectSelector : MonoBehaviour
{
    public HitPointReader hitPointReader;
    public List<ObjectComponent> selectedObjects;

    private VRInputController vrcon;

    private void Awake()
    {
        selectedObjects = new List<ObjectComponent>();
    }

    private void Start()
    {
        vrcon = GameObject.Find("VRInputController").GetComponent<VRInputController>();
        //selectedObjects.Add(WorldDataManager.Instance.ActiveWorld.GetVoxelObject(0));
    }

    private void Update()
    {
        ProcessInput(ToolManager.Instance.Imode);           
    }

    private void ProcessInput(ToolManager.InteractionMode mode)
    {
        if (mode == ToolManager.InteractionMode.Desktop)
        {
            // click to select pointing object 
            if (Input.GetKeyDown(KeyCode.Mouse0) && hitPointReader.hitting)
            {
                // 按住 contorl 键增加被选中的物体
                if (!Input.GetKey(KeyCode.LeftControl))
                {
                    selectedObjects.Clear();
                }

                ObjectComponent[] os =
                    WorldDataManager.Instance.ActiveWorld.GetVoxelObjectsAt(
                        hitPointReader.hitPoint.position - hitPointReader.hitPoint.normal / 2);


                //holding shift to only get first one
                if (Input.GetKey(KeyCode.LeftShift))
                {
                    if (!selectedObjects.Contains(os[0]))
                    {
                        selectedObjects.Add(os[0]);
                    }

                }
                else
                {
                    foreach (var o in os)
                    {
                        if (!selectedObjects.Contains(os[0]))
                        {
                            selectedObjects.Add(o);
                        }
                    }
                }


                Debug.Log("Selected Object " + selectedObjects);
            }
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                var last = selectedObjects[0];
                selectedObjects.Clear();
                selectedObjects.Add(WorldDataManager.Instance.ActiveWorld.GetNextObject(last));
                Debug.Log("Selected Object " + selectedObjects);
            }

        }
        else
        {
            
            
        }
    }
}
