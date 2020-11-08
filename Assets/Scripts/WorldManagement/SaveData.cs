using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

[System.Serializable]
public class SaveData
{
	public float worldSize;
	public List<SerializableObject> Objs = new List<SerializableObject>();

	static public SaveData SaveWorldData(string SaveFileName)
	{
		var saveData = new SaveData();
		saveData.worldSize = WorldDataManager.Instance.ActiveWorld.worldSize;
		foreach(var e in WorldDataManager.Instance.ActiveWorld.ObjectList)
		{
			saveData.Objs.Add(e.Serialize());
		}
		BinaryFormatter bf = new BinaryFormatter();
		
		var fs = File.Create(Application.dataPath + "/"+ SaveFileName + ".save");
		bf.Serialize(fs, saveData);
		fs.Close();
		return saveData;
	}
	static public void LoadWorldData(string SaveFileName)
	{
		BinaryFormatter bf = new BinaryFormatter();
		var path = Application.dataPath + "/" + SaveFileName + ".save";
		if (!System.IO.Directory.Exists(path)) Debug.LogError("Load Path Doesn't Exist!");
		var fs = File.Open(Application.dataPath + "/" + SaveFileName + ".save",FileMode.Open);
		fs.Seek(0, SeekOrigin.Begin);
		SaveData saveData = (SaveData)bf.Deserialize(fs);
		fs.Close();
		Debug.Log("Load " + SaveFileName);
		var world=WorldDataManager.Instance.CreateNewWorld(SaveFileName);
		world.WorldInit(saveData.Objs, saveData.worldSize);

		WorldDataManager.Instance.ActivateWorld(SaveFileName);

	}
}
