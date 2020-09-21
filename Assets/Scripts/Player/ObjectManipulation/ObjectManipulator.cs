using System;
using UnityEngine;
using Valve.VR;

/// <summary>
/// 包含所有对Object的操作
/// </summary>
public class ObjectManipulator : MonoBehaviour
{
    public ObjectSelector objectSelector;

    private VRInputController vrcon;

    // 移动物体
    private Vector3 moveStartLocHand;
    private Vector3Int moveStartLocObj;

    private void Start()
    {
        vrcon = GameObject.Find("VRInputController").GetComponent<VRInputController>();
    }

    private void Update()
    {
        ProcessInput(ToolManager.Instance.Imode);
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
            // 按下正面按钮
            if (vrcon.moveObjectInput.stateDown)
            {
                moveStartLocHand = vrcon.rightHand.transform.position;

                // 选中Object，准备平移
                ObjectComponent[] os = WorldDataManager.Instance.ActiveWorld.GetVoxelObjectsAt(vrcon.rightHand.transform.position);
                if (os.Length == 0)
                {
                    objectSelector.selectedObjects.Clear();
                }
                else
                {
                    foreach (var o in os)
                    {
                        if (!objectSelector.selectedObjects.Contains(os[0]))
                        {
                            objectSelector.selectedObjects.Add(o);
                            Debug.Log("Object picked " + objectSelector.selectedObjects);
                            // 记录下Object当前的位置
                            moveStartLocObj = o.gridBasePoint;
                        }
                    }
                }

            }
            // 放开正面按钮
            if (vrcon.moveObjectInput.stateUp)
            {
                objectSelector.selectedObjects.Clear();
            }

            // 保持按住正面按钮
            if (vrcon.moveObjectInput.state || vrcon.copyObjectInput.state)
            {
                MoveObjectByController();
            }

            // 按下扳机键，启动复制
            if (vrcon.copyObjectInput.stateDown)
            {
                moveStartLocHand = vrcon.rightHand.transform.position;

                // 选中Object，准备复制
                ObjectComponent[] os = WorldDataManager.Instance.ActiveWorld.GetVoxelObjectsAt(vrcon.rightHand.transform.position);
                if (os.Length == 0)
                {
                    objectSelector.selectedObjects.Clear();
                }
                else
                {
                    foreach (var o in os)
                    {
                        if (!objectSelector.selectedObjects.Contains(os[0]))
                        {
                            objectSelector.selectedObjects.Add(o);
                            Debug.Log("Object picked " + objectSelector.selectedObjects);
                            // 记录下Object当前的位置
                            moveStartLocObj = o.gridBasePoint;
                        }
                    }
                    CopyObject();
                }
            }

            // 放开扳机键
            if (vrcon.copyObjectInput.stateUp)
            {
                objectSelector.selectedObjects.Clear();
            }
        }
    }

    /// <summary>
    /// 根据双手距离，决定其边长，创建一个立方体
    /// </summary>
    private void CreateNewObject()
    {
        Vector3 leftPoint = vrcon.leftHand.transform.position;
        Vector3 rightPoint = vrcon.leftHand.transform.position;
        float edge = (leftPoint - rightPoint).magnitude;
        //TODO
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
            o.gridBasePoint += delta;
        }
    }
    
    // 根据手相对于抓取时刻的位置，判定Object移动的方向和距离
    private void MoveObjectByController()
    {
        foreach (var o in objectSelector.selectedObjects)
        {
            Vector3 direction = vrcon.rightHand.transform.position - this.moveStartLocHand;
            Debug.Log("this.moveStartLoc: " + this.moveStartLocHand);
            Debug.Log("vrcon.rightHand.transform.position: " + vrcon.rightHand.transform.position);
            Debug.Log("direction: " + direction.ToString("f4"));

            Vector3Int delta_axis = new Vector3Int();
            delta_axis = MathHelper.WorldOriToMainAxis(direction);
            Debug.Log("delta: " + delta_axis);
            int delta_mag = Mathf.CeilToInt(direction.magnitude * 100) / 10;
            delta_axis.Scale(new Vector3Int(delta_mag, delta_mag, delta_mag));
            o.gridBasePoint = this.moveStartLocObj + delta_axis;
            Debug.Log("o.basePoint: " + o.gridBasePoint);
        }
    }
}