using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoxelPlacer : MonoBehaviour
{
    public HitPointReader hitPointReader;
    public ObjectSelector objectSelector;

    private VRInputController vrcon;

    public Voxel voxelArg;

    // Start is called before the first frame update
    private void Start()
    {
        //Setup voxel arg
        voxelArg = new Voxel()
        {
            voxel = VoxelInfoLibrary.GetVoxel("Stone"),
            color = Color.white
        };
        vrcon = GameObject.Find("VRInputController").GetComponent<VRInputController>();
    }

    // Update is called once per frame
    void Update()
    {
        ProcessInput(ToolManager.Instance.Imode);
    }

    private void ProcessInput(ToolManager.InteractionMode mode)
    {
        if (mode == ToolManager.InteractionMode.Desktop)
        {
            if (hitPointReader.hitting)
            {
                // 按下鼠标左键
                if (Input.GetKeyDown(KeyCode.Mouse0))
                {
                    // 保存当前世界中与选中Voxel位置相邻的Object
                    List<ObjectComponent> Objects = new List<ObjectComponent>();

                    foreach (var o in WorldDataManager.Instance.ActiveWorld.ObjectList)
                    {
                        if (o.IsNearVoxel(hitPointReader.hitPoint.position + hitPointReader.hitPoint.normal / 2) && !o.voxelObjectData.isStatic)
                        {
                            WorldDataManager.Instance.ActiveWorld.SetVoxelAt(
                            o,
                            hitPointReader.hitPoint.position + hitPointReader.hitPoint.normal / 2,
                            voxelArg);
                            o.UpdateObjectMesh();
                            Objects.Add(o);
                        }
                    }
                    // 如果没有Object与该Voxel相邻，则认为创建了一个新的Object
                    if (Objects.Count == 0)
                    {
                        var o = WorldDataManager.Instance.ActiveWorld.
                            CreateNewObject(MathHelper.WorldPosToWorldIntPos(hitPointReader.hitPoint.position + hitPointReader.hitPoint.normal / 2));
                        WorldDataManager.Instance.ActiveWorld.SetVoxelAt(
                           o,
                           hitPointReader.hitPoint.position + hitPointReader.hitPoint.normal / 2,
                           voxelArg);
                        o.UpdateObjectMesh();
                    }
                    else // 将所有相邻的Object融合成一个新的Object
                    {
                        var firstObj = Objects[0];
                        for (int i = 1; i < Objects.Count; i++)
                        {
                            WorldDataManager.Instance.ActiveWorld.MergeTwoObjects(firstObj, Objects[i], WorldData.MergeType.Or);
                        }
                    }
                }

                // 点击鼠标右键
                else if (Input.GetKeyDown(KeyCode.Mouse1))
                {
                    // 遍历被选中的Object，删除选中位置的Voxel
                    List<ObjectComponent> deleteObjects = new List<ObjectComponent>();
                    foreach (var o in objectSelector.selectedObjects)
                    {
                        WorldDataManager.Instance.ActiveWorld.DeleteVoxelAt(o,
                            hitPointReader.hitPoint.position - hitPointReader.hitPoint.normal / 2);
                        if (o.voxelObjectData.VoxelDataDict.Count == 0)
                            deleteObjects.Add(o);
                        o.UpdateObjectMesh();
                    }
                    for (int i = 0; i < deleteObjects.Count; i++)
                    {
                        WorldDataManager.Instance.ActiveWorld.DeleteObject(deleteObjects[i]);
                    }
                }
            }
        }
        else
        {
            // 按下正面按钮
            if (vrcon.moveObjectInput.stateDown)
            {
                // 选中Voxel
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
}