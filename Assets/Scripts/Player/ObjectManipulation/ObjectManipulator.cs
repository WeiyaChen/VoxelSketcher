using System;
using UnityEngine;
using Valve.VR;

public class ObjectManipulator : MonoBehaviour
{
    public ObjectSelector objectSelector;
    public SteamVR_Action_Boolean copyInput;
    public SteamVR_Action_Boolean moveInput;
    public SteamVR_Action_Vector3 handPose;

    private Vector3 moveStartLoc;


    private void Update()
    {
        ProcessInput(ToolManager.Instance.Imode);

        if (ToolManager.Instance.Imode == ToolManager.InteractionMode.VR)
        {
            if (moveInput.stateDown)
            {
                moveStartLoc = handPose.axis;
            }
        }
    }

    private void ProcessInput(ToolManager.InteractionMode mode)
    {
        if (mode == ToolManager.InteractionMode.Desktop)
        {
            //Copy
            if (Input.GetKeyDown(KeyCode.C))
            {
                CopyObject();
            }

            //Merge
            if (objectSelector.selectedObjects.Count > 1)
            {
                if (Input.GetKey(KeyCode.M))
                {
                    if (Input.GetKeyDown(KeyCode.Alpha1))
                    {
                        MergeObject(WorldData.MergeType.Or);
                    }
                    if (Input.GetKeyDown(KeyCode.Alpha2))
                    {
                        MergeObject(WorldData.MergeType.And);
                    }
                    if (Input.GetKeyDown(KeyCode.Alpha3))
                    {
                        MergeObject(WorldData.MergeType.Not);
                    }
                }
            }

            //Move
            MoveObjectByKeyboard();
        }
        else if (mode == ToolManager.InteractionMode.VR)
        {
            if (this.moveInput.state)
            {
                MoveObjectByController();
            }
        }

    }

    private void CopyObject()
    {
        foreach (var o in objectSelector.selectedObjects)
        {
            WorldDataManager.Instance.ActiveWorld.CopyObject(o);
        }
    }

    private void MergeObject(WorldData.MergeType t)
    {
        for (int i = 1; i < objectSelector.selectedObjects.Count; i++)
        {
            WorldDataManager.Instance.ActiveWorld.MergeTwoObjects(
                objectSelector.selectedObjects[0],
                objectSelector.selectedObjects[i],
                t);
        }
    }

    private void MoveObjectByKeyboard()
    {
        Vector3Int delta = new Vector3Int();
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            delta.x = -1;
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            delta.x = 1;
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            delta.z = -1;
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            delta.z = 1;
        }
        else if (Input.GetKeyDown(KeyCode.PageDown))
        {
            delta.y = -1;
        }
        else if (Input.GetKeyDown(KeyCode.PageUp))
        {
            delta.y = 1;
        }
        foreach (var o in objectSelector.selectedObjects)
        {
            o.basePoint += delta;
        }
    }
                              
    private void MoveObjectByController()
    {
        foreach (var o in objectSelector.selectedObjects)
        {
            Vector3 direction = this.handPose.axis - this.moveStartLoc;
            Vector3Int delta = new Vector3Int();
            delta = MathHelper.WorldOriToMainAxis(direction);
            o.basePoint += delta;
        }
    }
}