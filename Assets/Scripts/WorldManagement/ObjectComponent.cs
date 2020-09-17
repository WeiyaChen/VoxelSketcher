using UnityEngine;

public class ObjectComponent : MonoBehaviour
{
    public Vector3Int basePoint;
    //Local Coordinates data
    public ObjectData voxelObjectData;

    public MeshFilter meshFilter;
    public MeshRenderer meshRenderer;
    public MeshCollider meshCollider;

    private void Update()
    {
        transform.position = basePoint;
    }

    /// <summary>
    /// 判断某个位置点是否与该Object相邻
    /// </summary>
    /// <param name="worldPosition">待判断的位置点</param>
    /// <returns></returns>
    public bool IsNearVoxel(Vector3 worldPosition)
    {
        var localPos = MathHelper.WorldPosToWorldIntPos(worldPosition)-basePoint;
        foreach (var p in voxelObjectData.VoxelDataDict.Keys)
        {
            if ((localPos - p).magnitude <= 1.01f)
                return true;
        }
        return false;
    }

    public void UpdateObjectMesh()
    {
        Mesh mesh;
        Material[] mats;
        voxelObjectData.GenerateMeshAndMats(out mesh, out mats);
        meshFilter.mesh = mesh;
        meshCollider.sharedMesh = mesh;
        meshRenderer.materials = mats;
    }
}

