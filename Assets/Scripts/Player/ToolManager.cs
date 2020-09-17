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

    public ISteamVR_Action_Boolean switchMode;

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
        this.Imode = InteractionMode.Desktop;
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
                Tmode = ToolMode.PlaceVoxel;
                ToolModeSwitching();
            }
            else if (Input.GetKeyDown(KeyCode.F2))
            {
                Tmode = ToolMode.FaceStretch;
                ToolModeSwitching();
            }
            else if (Input.GetKeyDown(KeyCode.F3))
            {
                Tmode = ToolMode.ObjectManipulation;
                ToolModeSwitching();
            }
        }
        else if (Imode == InteractionMode.VR)
        {

        }
    }

    private void ToolModeSwitching()
    {
        switch (Tmode)
        {
            case ToolMode.PlaceVoxel:
                voxelPlacer.gameObject.SetActive(true);
                faceStretcher.gameObject.SetActive(false);
                objectManipulator.gameObject.SetActive(false);
                break;
            case ToolMode.FaceStretch:
                voxelPlacer.gameObject.SetActive(false);
                faceStretcher.gameObject.SetActive(true);
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
        if (Input.GetKeyDown(KeyCode.F4) && Imode == InteractionMode.Desktop)
        {
            Imode = InteractionMode.VR;
        }
        // VR to desktop
        if (Input.GetKeyDown(KeyCode.F5) && Imode == InteractionMode.VR)
        {
            Imode = InteractionMode.Desktop;
        }
    }
}
