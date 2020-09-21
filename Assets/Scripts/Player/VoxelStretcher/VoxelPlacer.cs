using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoxelPlacer : MonoBehaviour
{
    public HitPointReader hitPointReader;
    public ObjectSelector objectSelector;
    
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
    }

    // Update is called once per frame
    void Update()
    {
        if (hitPointReader.hitting)
        {
            // 按下鼠标左键
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                /*
                foreach (var o in objectSelector.selectedObjects)
                {
                    WorldDataManager.Instance.ActiveWorld.SetVoxelAt(
                        o, 
                        hitPointReader.hitPoint.position + hitPointReader.hitPoint.normal / 2, 
                        voxelArg);
                    o.UpdateObjectMesh();
                }
                */
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
                    var o= WorldDataManager.Instance.ActiveWorld.
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
                foreach (var o in objectSelector.selectedObjects)
                {
                    WorldDataManager.Instance.ActiveWorld.DeleteVoxelAt(o, 
                        hitPointReader.hitPoint.position - hitPointReader.hitPoint.normal / 2);
                    o.UpdateObjectMesh();
                }
            }
        }
        
    }
}
