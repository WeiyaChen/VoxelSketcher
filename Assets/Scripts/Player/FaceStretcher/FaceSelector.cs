using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct SelectionPoint
{
    public Vector3Int position;
    public Vector3 normal;
}
public class FaceSelector : MonoBehaviour
{
    // desktop变量
    public HitPointReader hitPointReader;
    public ObjectSelector objectSelector;

    // VR变量
    public FaceStretcher faceStretcher;
    private VRInputController vrcon;
    

    public List<Vector3Int> selectionPoints;

    public Vector3? normal;

    //The point when left mouse clicked down
    private HitPoint? m_downPoint;
    //The point when left mouse release up
    private HitPoint? m_upPoint;

    // Start is called before the first frame update
    private void Awake()
    {
        normal = null;
        m_downPoint = null;
        m_upPoint = null;
        selectionPoints = new List<Vector3Int>();
        vrcon = GameObject.Find("VRInputController").GetComponent<VRInputController>();
    }

    // Update is called once per frame
    void Update()
    {
        GetSelectionPoints(ToolManager.Instance.Imode);
    }

    private void GetSelectionPoints(ToolManager.InteractionMode mode)
    {
        if (mode == ToolManager.InteractionMode.Desktop)
        {
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                TriggerSelection();
            }
            if (Input.GetKey(KeyCode.Mouse0))
            {
                Selecting();
            }
        }
        else // VR mode
        {
            if (vrcon.selectFaceInput.stateDown)
            {
                TriggerSelection();
            }
            if (vrcon.selectFaceInput.state)
            {
                Selecting();
            }
        }
    }

    private void TriggerSelection()
    {
        m_upPoint = null;
        m_downPoint = null;
        Debug.Log("selectFaceInput");
        if (hitPointReader.hitting)
        {
            m_downPoint = hitPointReader.hitPoint;
            normal = m_downPoint.Value.normal;
            Debug.Log("hit position: "+m_downPoint.Value.position);
        }
        else
        {
            selectionPoints.Clear();
            faceStretcher.stretchedPoints.Clear(); // 为了可视化
        }
    }

    private void Selecting()
    {
        HitPoint currentPoint = hitPointReader.hitPoint;
        if (m_downPoint != null && hitPointReader.hitting)
        {
            //Must be same normal face
            if (currentPoint.normal == m_downPoint.Value.normal &&
                Vector3.Dot(currentPoint.position - m_downPoint.Value.position, currentPoint.normal) == 0)
            {
                m_upPoint = currentPoint;
                Vector3Int min, max;
                Vector3 down = m_downPoint.Value.position - m_downPoint.Value.normal / 2 * WorldDataManager.Instance.ActiveWorld.worldSize;
                Vector3 up = m_upPoint.Value.position - m_upPoint.Value.normal / 2 * WorldDataManager.Instance.ActiveWorld.worldSize;
                MathHelper.GetMinMaxPoint(down, up, out min, out max);

                UpdateSelectionPoints(min, max);
                //Debug.Log("min "+min);
                //Debug.Log("max "+max);
            }
        }
    }

    /// <summary>
    /// 通过缩放后的边界点，得到一组选中的点集
    /// </summary>
    /// <param name="min">最小边界点</param>
    /// <param name="max">最大边界点</param>
    private void UpdateSelectionPoints(Vector3Int min, Vector3Int max)
    {
        selectionPoints.Clear();

        List<Vector3Int> grid = MathHelper.GenerateGridFromDiagnal(min, max);
        foreach (var p in grid)
        {
            Vector3Int pos = p - this.faceStretcher.targetObj.gridBasePoint;
            //Debug.Log("pos " + pos);
            Voxel v = this.faceStretcher.targetObj.voxelObjectData.GetVoxelAt(pos);
            if ( v.voxel!= null)
            {
                //Debug.Log("hhhhhhhhhhhhhhhhhhhhhh " + v.voxel.posOffset);
                selectionPoints.Add(pos);
                faceStretcher.stretchedPoints.Add(pos); // 为了可视化
                v.color = Color.yellow;
            }
        }
    }
}
