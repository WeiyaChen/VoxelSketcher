using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoxelSelector : MonoBehaviour
{
    public List<Voxel> selectedVoxels;
    private VRInputController vrcon;
    private ObjectComponent targetObj;

    public VoxelSelector(ObjectComponent target)
    {
        this.targetObj = target;
    }

    // Start is called before the first frame update
    void Start()
    {
        selectedVoxels = new List<Voxel>();
        vrcon = GameObject.Find("VRInputController").GetComponent<VRInputController>();
    }

    // Update is called once per frame
    void Update()
    {
        ProcessInput();
    }

    private void ProcessInput()
    {
        // 如果有选择事件被触发
        if (vrcon.selectVoxelInput.state)
        {
            // 选中位置的信息存入一个Voxel对象
            Vector3Int pos = vrcon.GetScaledHandLocation(vrcon.rightHand);
            Voxel v = this.targetObj.voxelObjectData.GetVoxelAt(pos);
            // 如果此处没有voxel
            if (v.voxel == null)
            {
                // 该位置不靠近已有的体素，取消所有选中的voxel
                if (!this.targetObj.IsNearVoxel(pos))
                {
                    this.selectedVoxels.Clear();
                }
            }
            // 如果有voxel，则选中该voxel并高亮显示
            else if (!this.selectedVoxels.Contains(v))
            {
                this.selectedVoxels.Add(v);
                v.color = Color.yellow;
            }
        }
    }
}
