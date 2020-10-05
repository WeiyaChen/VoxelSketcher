using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

/// <summary>
/// 管理交互的内容与形式
/// </summary>
public class ToolManager : Singleton<ToolManager>
{
    public VoxelPlacer voxelPlacer;
    public FaceStretcher faceStretcher;
    public ObjectManipulator objectManipulator;

    private VRInputController vrcon;

    public enum ToolMode
    {
        PlaceVoxel,
        FaceStretch,
        ObjectManipulation
    }
    public ToolMode Tmode;
    public enum InteractionMode
    {
        Desktop,
        VR
    }
    public InteractionMode Imode;

    private void Start()
    {
        this.Imode = InteractionMode.VR;
        vrcon = GameObject.Find("VRInputController").GetComponent<VRInputController>();
        ToolModeSwitching();
    }

    // Update is called once per frame
    void Update()
    {
        ToolModeUpdate();
        InteractionModeUpdate();
    }

    private void ToolModeUpdate()
    {
        if (Imode == InteractionMode.Desktop)
        {
            if (Input.GetKeyDown(KeyCode.F1))
            {
                Tmode = ToolMode.ObjectManipulation;
                ToolModeSwitching();
            }
            else if (Input.GetKeyDown(KeyCode.F2))
            {
                if (objectManipulator.objectSelector.selectedObjects.Count != 0)
                {
                    Tmode = ToolMode.PlaceVoxel;
                    ToolModeSwitching();
                }
            }
            else if (Input.GetKeyDown(KeyCode.F3))
            {
                if (objectManipulator.objectSelector.selectedObjects.Count != 0)
                {
                    Tmode = ToolMode.FaceStretch;
                    ToolModeSwitching();
                }
            }
        }
        else if (Imode == InteractionMode.VR)
        {
            if (vrcon.switchModeInput.stateDown)
            { 
                if (Tmode == ToolMode.ObjectManipulation && objectManipulator.objectSelector.selectedObjects.Count > 0)
                {
                    Tmode = ToolMode.PlaceVoxel;
                    ToolModeSwitching();
                }
                else if (Tmode == ToolMode.PlaceVoxel)
                {
                    Tmode = ToolMode.FaceStretch;
                    ToolModeSwitching();
                }
                else if (Tmode == ToolMode.FaceStretch)
                {
                    Tmode = ToolMode.ObjectManipulation;
                    ToolModeSwitching();
                }
                Debug.Log("Current Mode: "+Tmode);
            }
        }
    }

    private void ToolModeSwitching()
    {
        switch (Tmode)
        {
            case ToolMode.ObjectManipulation:
                objectManipulator.gameObject.SetActive(true);

                voxelPlacer.gameObject.SetActive(false);
                faceStretcher.faceSelector.hitPointReader.ToggleVRPointer(false);
                faceStretcher.gameObject.SetActive(false);
                break;
            case ToolMode.PlaceVoxel:
                voxelPlacer.gameObject.SetActive(true);
                voxelPlacer.SetTargetObj();

                objectManipulator.gameObject.SetActive(false);
                faceStretcher.gameObject.SetActive(false);
                break;
            case ToolMode.FaceStretch:
                faceStretcher.gameObject.SetActive(true);
                faceStretcher.faceSelector.hitPointReader.ToggleVRPointer(true);

                voxelPlacer.gameObject.SetActive(false);
                objectManipulator.gameObject.SetActive(false);
                break;
            default:
                break;
        }
    }

    public void InteractionModeUpdate()
    {
        // Switch to VR mode
        if (Input.GetKeyDown(KeyCode.F4) && Imode == InteractionMode.Desktop)
        {
            Imode = InteractionMode.VR;
            Debug.Log("InteractionMode.VR");
        }
        // VR to desktop
        if (Input.GetKeyDown(KeyCode.F5) && Imode == InteractionMode.VR)
        {
            Imode = InteractionMode.Desktop;
            Debug.Log("InteractionMode.Desktop");
        }
    }

    static public void highlightObject(GameObject obj, Color c, float width)
    {
        var outline = obj.AddComponent<Outline>();

        outline.OutlineMode = Outline.Mode.OutlineAll;
        outline.OutlineColor = c;
        outline.OutlineWidth = width;
    }

    static public void unHighlightObject(GameObject obj)
    {
        Destroy(obj.GetComponent<Outline>());
    }
}
