using UnityEngine;
using Valve.VR.Extras;

[System.Serializable]
public struct HitPoint
{
    public Vector3 position;
    public Vector3 normal;
}
public class HitPointReader : MonoBehaviour
{
    public HitPoint hitPoint;
    public bool hitting { get; private set; }

    private VRInputController vrcon;
    private SteamVR_LaserPointer laserPointer;

    private void Awake()
    {
        vrcon = GameObject.Find("VRInputController").GetComponent<VRInputController>();
        this.laserPointer = vrcon.rightHand.GetComponent<SteamVR_LaserPointer>();
    }

    private void Update()
    {
        SelectVoxel();
    }

    private void SelectVoxel()
    {
        hitting = false;
        hitPoint.position = Vector3.zero;
        hitPoint.normal = Vector3.zero;

        RaycastHit hit;
        if (ToolManager.Instance.Imode == ToolManager.InteractionMode.Desktop)
        {
            Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit);
        }
        else // VR mode
        {
            Physics.Raycast(new Ray(laserPointer.transform.position, laserPointer.transform.forward), out hit);
        }
        
        if (hit.collider)
        {
            hitting = true;
            hitPoint.position = hit.point / WorldDataManager.Instance.ActiveWorld.worldSize;
            hitPoint.normal = hit.normal;
        }

    }

    public void ToggleVRPointer(bool active)
    {
        if (ToolManager.Instance.Imode == ToolManager.InteractionMode.Desktop)
        {
            this.laserPointer.active = false;
        }
        else
        {
            this.laserPointer.active = active;
        }
    }
}