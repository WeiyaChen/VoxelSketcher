using System;
using System.Collections.Generic;
using UnityEngine;

public class WorldDataManager : Singleton<WorldDataManager>
{
    private List<WorldData> m_availableWorlds;

    public WorldData ActiveWorld { get; private set; }

    new private void Awake()
    {
        base.Awake();
        m_availableWorlds = new List<WorldData>();
    }

    public WorldData CreateNewWorld(string name)
    {
        WorldData world = new WorldData(name);
        m_availableWorlds.Add(world);
        return world;
    }
    public void CreateNewWorld(WorldData world)
    {
        m_availableWorlds.Add(world);
    }
    public void ActivateWorld(string name)
    {
        if (ActiveWorld != null)
        {
            for (int i = 0; i < ActiveWorld.ObjectList.Count; i++)
            {
                ActiveWorld.DeleteObject(i);
                Debug.Log("DeleteObject! " + i);
            }
        }
        ActiveWorld = m_availableWorlds.Find(x => x.name == name);
    }

    public void NextWorld()
    {
        // 只有一个world时无法切换
        if (m_availableWorlds.Count > 1)
        {
            if (ActiveWorld != null)
            {
                SaveData.SaveWorldData(ActiveWorld.name);
            }
            int counter = (Convert.ToInt32(ActiveWorld.name) + 1) % m_availableWorlds.Count;
            string name = counter.ToString();
            Debug.Log("m_availableWorlds.Count! " + m_availableWorlds.Count);
            Debug.Log("name! "+ name);
            ActivateWorld(name);
        }
        
    }

    public WorldData[] GetAvailableWorlds()
    {
        return m_availableWorlds.ToArray();
    }

}