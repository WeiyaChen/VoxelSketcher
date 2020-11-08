using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldObject : MonoBehaviour
{
    // Start is called before the first frame update
    void Awake()
    {
        WorldDataManager.Instance.CreateNewWorld("0");
        WorldDataManager.Instance.ActivateWorld("0");

        //Initial Platform
        WorldDataManager.Instance.ActiveWorld.CreateNewObject(new Vector3Int(0, 0, 0),true);
        Voxel v = new Voxel() { voxel = VoxelInfoLibrary.GetVoxel("Stone"), color = Color.white };
        for (int x = 0; x < 10; x++)
        {
            for (int z = 0; z < 10; z++)
            {
                WorldDataManager.Instance.ActiveWorld.SetVoxelAt(
                    WorldDataManager.Instance.ActiveWorld.GetVoxelObject(0),
                    new Vector3(x, 0, z),
                    v);
            }
        }
        var voxelizers = GameObject.FindObjectsOfType<Voxelizer>();
        foreach (var voxelizer in voxelizers)
        {
            voxelizer.Voxelize();
        }
        WorldDataManager.Instance.ActiveWorld.UpdateAllObjects();
    }

}
