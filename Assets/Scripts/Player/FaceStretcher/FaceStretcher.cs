using UnityEngine;
using System.Collections.Generic;
using System.Drawing;

public class FaceStretcher : MonoBehaviour
{
    // 当前正在被修改的Object
    public ObjectComponent targetObj;

    public FaceSelector faceSelector;
    public FaceIndicator selectionIndicator;
    public int stretchResult;
    private Vector3? m_downCursorPoint;
    private Vector3? m_upCursorPoint;

    public List<Vector3Int> stretchedPoints;

    // VR
    private VRInputController vrcon;

    private void Awake()
    {
        stretchedPoints = new List<Vector3Int>();
        stretchResult = 0;
        m_downCursorPoint = null;
        m_upCursorPoint = null;
        vrcon = GameObject.Find("VRInputController").GetComponent<VRInputController>();
    }

    private void Update()
    {
        //Stretch
        ComputeStretching(ToolManager.Instance.Imode);

        //Apply
        if (ToolManager.Instance.Imode == ToolManager.InteractionMode.Desktop)
        {
            if (Input.GetKeyUp(KeyCode.Mouse1) && faceSelector.normal != null)
            {
                ApplyStretching();
            }
        }
        else
        {
            if (vrcon.pullFaceInput.stateUp && faceSelector.normal != null)
            {
                ApplyStretching();
            }
        }

    }

    private void ComputeStretching(ToolManager.InteractionMode mode)
    {
        if (mode == ToolManager.InteractionMode.Desktop)
        {
            if (Input.GetKeyDown(KeyCode.Mouse1))
            {
                m_upCursorPoint = null;
                m_downCursorPoint = Input.mousePosition;
            }
            if (Input.GetKey(KeyCode.Mouse1))
            {
                if (m_downCursorPoint != null && selectionIndicator.data.Count > 0 && faceSelector.normal != null)
                {
                    var normal = new Vector3Int(
                        (int)faceSelector.normal.Value.x,
                        (int)faceSelector.normal.Value.y,
                        (int)faceSelector.normal.Value.z);

                    m_upCursorPoint = Input.mousePosition;
                    Camera camera = Camera.main;
                    Vector3 p1 = camera.ScreenToWorldPoint(new Vector3(m_upCursorPoint.Value.x, m_upCursorPoint.Value.y, camera.nearClipPlane));
                    Vector3 p2 = camera.ScreenToWorldPoint(new Vector3(m_downCursorPoint.Value.x, m_downCursorPoint.Value.y, camera.nearClipPlane));

                    float result = Vector3.Dot(p1 - p2, normal);
                    stretchResult = (int)(result * 50);
                }

                //Make stretched data
                UpdateStretchedPointDict();

            }
        }
        else // VR mode
        {
            if (vrcon.pullFaceInput.stateDown)
            {
                m_upCursorPoint = null;
                m_downCursorPoint = vrcon.rightHand.transform.position;
                Debug.Log("vrcon.pullFaceInput.stateDown");
            }
            if (vrcon.pullFaceInput.state)
            {
                if (m_downCursorPoint != null && selectionIndicator.data.Count > 0 && faceSelector.normal != null)
                {
                    var normal = new Vector3Int(
                        (int)faceSelector.normal.Value.x,
                        (int)faceSelector.normal.Value.y,
                        (int)faceSelector.normal.Value.z);

                    m_upCursorPoint = vrcon.rightHand.transform.position;

                    float result = Vector3.Dot(m_upCursorPoint.Value - m_downCursorPoint.Value, normal);
                    stretchResult = (int)(result * 50);
                }
                //Make stretched data
                UpdateStretchedPointDict();
            }
        }
    }

    private void UpdateStretchedPointDict()
    {
        if (faceSelector.normal != null)
        {
            var normal = new Vector3Int(
                (int)faceSelector.normal.Value.x,
                (int)faceSelector.normal.Value.y,
                (int)faceSelector.normal.Value.z);

            stretchedPoints.Clear();

            // 根据选择的面和推拉的距离，生成推拉后的点集
            foreach (var p in faceSelector.selectionPoints)
            {
                //Add 
                if (stretchResult > 0)
                {
                    for (int i = 0; i <= stretchResult; i++)
                    {
                        //Stretch out
                        stretchedPoints.Add(p + i * normal);
                    }
                }
                //Substract
                else
                {
                    for (int i = stretchResult; i <= 0; i++)
                    {
                        //Stretch out
                        stretchedPoints.Add(p + i * normal);
                        
                    }
                }
            }
        }
    }

    private void ApplyStretching()
    {
        
        var normal = new Vector3Int(
            (int)faceSelector.normal.Value.x,
            (int)faceSelector.normal.Value.y,
            (int)faceSelector.normal.Value.z);
        //Add
        if (stretchResult > 0)
        {
            foreach (var p in faceSelector.selectionPoints)
            {
                Voxel v = this.targetObj.voxelObjectData.GetVoxelAt(p);
                for (int i = 1; i <= stretchResult; i++)
                {
                    this.targetObj.voxelObjectData.SetVoxelAt(p + normal * i, v);
                }
            }
            this.targetObj.UpdateObjectMesh();

        }
        //Substract
        else
        {
            foreach (var p in faceSelector.selectionPoints)
            {
                for (int i = stretchResult; i <= 0; i++)
                {
                    //Delete voxel
                    this.targetObj.voxelObjectData.DeleteVoxelAt(p + normal * i);
                }
            }
            this.targetObj.UpdateObjectMesh();
        }

        faceSelector.selectionPoints.Clear();
        stretchedPoints.Clear();
        stretchResult = 0;
    }
}