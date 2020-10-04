using System;
using UnityEngine;
using Valve.VR;

/// <summary>
/// 包含所有对Object的操作
/// </summary>
public class ObjectManipulator : MonoBehaviour
{
    public ObjectSelector objectSelector;

    // VR
    private VRInputController vrcon;
    public MergeOptions mOptions;

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
            // 按下正面按钮，启动物体移动
            if (vrcon.moveObjectInput.stateDown)
            {
                moveStartLocHand = vrcon.rightHand.transform.position;
                if (this.objectSelector.selectedObjects.Count > 0)
                {
                    moveStartLocObj = this.objectSelector.GetSelectedObject().gridBasePoint;
                }
            }
            else if (vrcon.moveObjectInput.stateUp) // 放开正面按钮，结束物体移动
            {
                objectSelector.selectedObjects.Clear();
            }
            else if (vrcon.moveObjectInput.state || vrcon.copyObjectInput.state) // 保持按住正面按钮，移动物体
            {
                MoveObjectByController();
            }
            else if (vrcon.copyObjectInput.stateDown) // 按下扳机键，启动复制
            {
                moveStartLocHand = vrcon.rightHand.transform.position;
                CopyObject();
            }
            else if (vrcon.copyObjectInput.stateUp) // 放开扳机键
            {
                objectSelector.selectedObjects.Clear();
            }
            else if (vrcon.createObjectInput.stateDown) // 启动创建新Object
            {
                CreateNewObject();
            }
            else if (vrcon.combineObjectInput.stateDown) // 启动合并Object
            {
                // 根据菜单选择合并模式
                mOptions.gameObject.SetActive(true);
            }
            
        }
    }

    /// <summary>
    /// 根据双手距离，决定其边长，创建一个立方体
    /// </summary>
    private void CreateNewObject()
    {
        Vector3 leftPoint = vrcon.leftHand.transform.position;
        Vector3 rightPoint = vrcon.rightHand.transform.position;
        Vector3Int min, max;
        MathHelper.GetMinMaxPoint(leftPoint, rightPoint, out min, out max);
        ;
        // 根据双手位置生成一组点，并生成对应的Object
        WorldDataManager.Instance.ActiveWorld.CreateNewObjectFromGridData(
            MathHelper.GenerateGridFromDiagnal(min, max), new Voxel());
    }

    private void CopyObject()
    {
        if (objectSelector.selectedObjects.Count > 0)
        {
            moveStartLocObj = this.objectSelector.GetSelectedObject().gridBasePoint;
            foreach (var o in objectSelector.selectedObjects)
            {
                WorldDataManager.Instance.ActiveWorld.CopyObject(o);
            }
        }
        else
        {
            Debug.Log("No objects selected");
        }
        
    }

    public void MergeObject(WorldData.MergeType t)
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
        if (objectSelector.selectedObjects.Count > 0)
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
        else
        {
            Debug.Log("No objects selected");
        }
    }

    /// <summary>
    /// 每次调用，将被选中物体旋转90度
    /// </summary>
    private void RotateObject()
    {
        // TODO
    }
}