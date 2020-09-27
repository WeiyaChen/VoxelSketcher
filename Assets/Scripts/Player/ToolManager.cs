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
        VoxelManipulation,
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
                    Tmode = ToolMode.VoxelManipulation;
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
                    Tmode = ToolMode.VoxelManipulation;
                    ToolModeSwitching();
                }
                else if (Tmode == ToolMode.VoxelManipulation)
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
            case ToolMode.VoxelManipulation:
                voxelPlacer.gameObject.SetActive(true);
                voxelPlacer.SetTargetObj();
                objectManipulator.gameObject.SetActive(false);
                break;
            case ToolMode.ObjectManipulation:
                voxelPlacer.gameObject.SetActive(false);
                faceStretcher.gameObject.SetActive(false);
                objectManipulator.gameObject.SetActive(true);
                break;
            default:
                break;
        }
    }

    public void InteractionModeUpdate()
    {
        // Switch to VR mode
        if (Input.GetKeyDown(KeyCode.F3) && Imode == InteractionMode.Desktop)
        {
            Imode = InteractionMode.VR;
            Debug.Log("InteractionMode.VR");
        }
        // VR to desktop
        if (Input.GetKeyDown(KeyCode.F4) && Imode == InteractionMode.VR)
        {
            Imode = InteractionMode.Desktop;
            Debug.Log("InteractionMode.Desktop");
        }
    }
}
