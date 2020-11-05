using System.Collections.Generic;
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
            }
        }
        ActiveWorld = m_availableWorlds.Find(x => x.name == name);
    }

    public WorldData[] GetAvailableWorlds()
    {
        return m_availableWorlds.ToArray();
    }

}