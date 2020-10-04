using UnityEngine;
using Valve.VR.InteractionSystem;

public class MergeOptions : MonoBehaviour
{
    public ObjectManipulator om;

    public void OnPressForAdd(Hand hand)
    {
        om.MergeObject(WorldData.MergeType.And);
        Debug.Log("OnPressForAdd!");
        this.gameObject.SetActive(false);
    }

    public void OnPressForOr(Hand hand)
    {
        om.MergeObject(WorldData.MergeType.Or);
        Debug.Log("OnPressForOr!");
        this.gameObject.SetActive(false);
    }

    public void OnPressForNot(Hand hand)
    {
        om.MergeObject(WorldData.MergeType.Not);
        Debug.Log("OnPressForNot!");
        this.gameObject.SetActive(false);
    }
}
