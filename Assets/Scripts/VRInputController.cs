using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;

public class VRInputController : MonoBehaviour
{
    // Object级输入
    public SteamVR_Action_Boolean createObjectInput;
    public SteamVR_Action_Boolean copyObjectInput;
    public SteamVR_Action_Boolean moveObjectInput;
    public SteamVR_Action_Boolean rotateObjectInput;
    public SteamVR_Action_Boolean combineObjectInput;

    // Voxel级输入
    public SteamVR_Action_Boolean createVoxelInput;
    public SteamVR_Action_Boolean deleteVoxelInput;
    public SteamVR_Action_Boolean selectVoxelInput;
    public SteamVR_Action_Boolean pullVoxelInput;

    public Hand leftHand;
    public Hand rightHand;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
