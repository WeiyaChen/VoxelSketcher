using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoxelPlacer : MonoBehaviour
{
    // 桌面变量
    public HitPointReader hitPointReader;
    public ObjectSelector objectSelector;

    private VRInputController vrcon;
    // 当前正在被修改的Object
    public ObjectComponent targetObj;
    public List<Voxel> selectedVoxels;

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
        selectedVoxels = new List<Voxel>();
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
        else // VR mode
        {
            // 如果有某个按键被按下
            if (vrcon.createVoxelInput.state || vrcon.deleteVoxelInput.state
                || vrcon.selectVoxelInput.state)
            {
                // 选中位置的信息存入一个Voxel对象
                Vector3Int pos = vrcon.GetScaledHandLocation(vrcon.rightHand);
                Debug.Log(pos);
                Voxel v = this.targetObj.voxelObjectData.GetVoxelAt(pos);
                // 如果此处没有voxel
                if (v.voxel == null)
                {
                    // 进一步判断是否与已有voxel相连，如果相连，则处理按键的事件
                    if (this.targetObj.IsNearVoxel(pos) && vrcon.createVoxelInput.state)
                    {
                        WorldDataManager.Instance.ActiveWorld.SetVoxelAt(this.targetObj, pos, voxelArg);
                        this.targetObj.UpdateObjectMesh();
                    }
                }
                else // 如果有voxel，则根据按键选中或者删除voxel
                {
                    // 删除这个voxel
                    if (vrcon.deleteVoxelInput.state)
                    {
                        WorldDataManager.Instance.ActiveWorld.DeleteVoxelAt(this.targetObj,pos);
                        if (this.targetObj.voxelObjectData.VoxelDataDict.Count == 0)
                            WorldDataManager.Instance.ActiveWorld.DeleteObject(this.targetObj);
                        this.targetObj.UpdateObjectMesh();
                    }
                    else if (!this.selectedVoxels.Contains(v) && vrcon.selectVoxelInput.state)
                    {
                        this.selectedVoxels.Add(v);
                        v.color = Color.yellow;
                    }
                }

            }
            // 放开正面按钮
            if (vrcon.moveObjectInput.stateUp)
            {
                objectSelector.selectedObjects.Clear();
            }

            // 放开扳机键
            if (vrcon.copyObjectInput.stateUp)
            {
                objectSelector.selectedObjects.Clear();
            }
        }
    }

    public void SetTargetObj() 
    {
        this.targetObj = this.objectSelector.GetSelectedObject();
    }
}