using UnityEngine;
public static class MathHelper
{
    /// <summary>
    /// 将一个浮点3D坐标转换为整数
    /// </summary>
    /// <param name="worldPos"></param>
    /// <returns></returns>
    static public Vector3Int WorldPosToWorldIntPos(Vector3 worldPos)
    {
        Vector3Int value = new Vector3Int(
            Mathf.FloorToInt(worldPos.x),
            Mathf.FloorToInt(worldPos.y),
            Mathf.FloorToInt(worldPos.z));
        return value;
    }

    static public Vector3Int WorldOriToMainAxis(Vector3 worldOri)
    {
        if ((Mathf.Abs(worldOri.x) > Mathf.Abs(worldOri.y)) && (Mathf.Abs(worldOri.x) > Mathf.Abs(worldOri.y)))
        {
            return new Vector3Int(1, 0, 0);
        }
        else if ((Mathf.Abs(worldOri.y) > Mathf.Abs(worldOri.x)) && (Mathf.Abs(worldOri.y) > Mathf.Abs(worldOri.z)))
        {
            return new Vector3Int(0, 1, 0);
        }
        else
        {
            return new Vector3Int(0, 0, 1);
        }
    }
}