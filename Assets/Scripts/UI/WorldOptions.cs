using System;
using UnityEngine;
using Valve.VR.InteractionSystem;

public class WorldOptions : MonoBehaviour
{
    public ObjectManipulator om;
    public int worldCounter;

    public void Start() 
    {
        this.worldCounter = Convert.ToInt32(WorldDataManager.Instance.ActiveWorld.name);
    }

    public void OnPressForCreate(Hand hand)
    {
        string name = this.worldCounter + "";
        SaveData.SaveWorldData(name);
        this.worldCounter++;
        name = this.worldCounter + "";
        WorldData newWorld = WorldDataManager.Instance.CreateNewWorld(name);
        Debug.Log("OnPressForCreate! "+this.worldCounter);

        // 切换到新world中去
        WorldDataManager.Instance.NextWorld();

        this.gameObject.SetActive(false);
    }

    public void OnPressForSwitch(Hand hand)
    {
        WorldDataManager.Instance.NextWorld();
        Debug.Log("OnPressForSwitch!");
        this.gameObject.SetActive(false);
    }

    public void OnPressForSave(Hand hand)
    {
        string name = this.worldCounter + "";
        SaveData.SaveWorldData(name);
        Debug.Log("OnPressForSave!");
        this.gameObject.SetActive(false);
    }

    public void OnPressForLoad(Hand hand)
    {
        string name = this.worldCounter + "";
        SaveData.SaveWorldData(name);
        // TODO: not implemented
        SaveData.LoadWorldData(name);
        Debug.Log("OnPressForLoad!");
        this.gameObject.SetActive(false);
    }
}
