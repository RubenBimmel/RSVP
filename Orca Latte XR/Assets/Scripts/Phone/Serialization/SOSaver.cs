using UnityEngine;    // For Debug.Log, etc.

using System.Text;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Reflection;

public class SOSaver {
	public static string currentFilePath = Application.persistentDataPath + "/save.dat";    // Edit this for different save files

	public static void Save <T>(string filePath, T toSave) where T : ScriptableObject {
		//First we make a new data object. This is where we will store all the things like our current crew, running Scenarios and things of that nature.
		SOPSaveData newData = new SOPSaveData();

		//TODO - temp for now, just making one Scenario Holder object to test. Need to rename all these things.
		SOHolder holder = new SOHolder();

		Type sType = toSave.GetType();
		holder.typeName = sType.Name;

		List<string> intNameList = new List<string>();
		List<int> intValueList = new List<int>();

		List<string> intArrayNameList = new List<string>();
		List<int[]> intArrayValueList = new List<int[]>();

		List<string> stringNameList = new List<string>();
		List<string> stringValueList = new List<string>();

		List<string> stringArrayNameList = new List<string>();
		List<string[]> stringArrayValueList = new List<string[]>();

		List<string> stringListNameList = new List<string>();
		List<List<string>> stringListValueList = new List<List<string>>();

        List<string> stringContactNames = new List<string>();
		List<string> stringContactValueList = new List<string>();

        List<string> stringChatNames = new List<string>();
        List<string> stringChatValueList = new List<string>();

        FieldInfo[] fields = sType.GetFields();
		foreach(FieldInfo f in fields){
			if (f.FieldType == typeof(int)) {
				intNameList.Add (f.Name);
				intValueList.Add ((int)f.GetValue (toSave));
			} 
			else if (f.FieldType == typeof(int[])) {
				intArrayNameList.Add (f.Name);
				intArrayValueList.Add ((int[])f.GetValue (toSave));
			} 
			else if(f.FieldType == typeof(string)){
				stringNameList.Add (f.Name);
				stringValueList.Add ((string)f.GetValue(toSave));
			}
			else if(f.FieldType == typeof(string[])){
				stringArrayNameList.Add (f.Name);
				stringArrayValueList.Add ((string[])f.GetValue(toSave));
			}
			else if(f.FieldType == typeof(List<string>)){
				stringListNameList.Add (f.Name);
				stringListValueList.Add ((List<string>)f.GetValue(toSave));
			}
			else if(f.FieldType == typeof(GamePhone.Contact)){
                stringContactNames.Add(f.Name);
                stringContactValueList.Add(((GamePhone.Contact)f.GetValue(toSave)).name);
            }
            else if (f.FieldType == typeof(GamePhone.Chat)) {
                stringChatNames.Add(f.Name);
                stringChatValueList.Add(((GamePhone.Chat)f.GetValue(toSave)).name);
            }
            else {
				//Debug.Log (f.FieldType);
			}
		}

		holder.stringNames = stringNameList.ToArray();
		holder.stringValues = stringValueList.ToArray();

		holder.stringArrayNames = stringArrayNameList.ToArray();
		holder.stringArrayValues = stringArrayValueList.ToArray();

		holder.stringListNames = stringListNameList.ToArray();
		holder.stringListValues = stringListValueList.ToArray();

		holder.intNames = intNameList.ToArray();
		holder.intValues = intValueList.ToArray();

		holder.intArrayNames = intArrayNameList.ToArray();
		holder.intArrayValues = intArrayValueList.ToArray();

		holder.contactNames = stringContactNames.ToArray();
	    holder.contactValues = stringContactValueList.ToArray();

        holder.chatNames = stringChatNames.ToArray();
        holder.chatValues = stringChatValueList.ToArray();

        //We'll add the SO holders to the data list
        newData.holders.Add (holder);

		//Now write to the given path (default for now)
		Stream stream = File.Open(filePath, FileMode.Create);
		BinaryFormatter bformatter = new BinaryFormatter();
		bformatter.Binder = new VersionDeserializationBinder();
		bformatter.Serialize(stream, newData);
		stream.Close();
	}

    public static T[] LoadAll<T>(string folderPath) where T : ScriptableObject {
        List<T> list = new List<T>();
        foreach (string file in Directory.GetFiles(folderPath)) {
            T newSo = Load<T>(file);
            if (newSo != null) {
                list.Add(newSo);
            }
        }
        return list.ToArray();
    }

    public static T Load <T>(string filePath) where T : ScriptableObject {
		if(!File.Exists(filePath)){
			Debug.Log ("NO FILE WITH PATH: " + filePath);
			return null;
		}

		Stream stream = File.Open(filePath, FileMode.Open);
		BinaryFormatter bformatter = new BinaryFormatter();
		bformatter.Binder = new VersionDeserializationBinder();
		SOPSaveData loadData = (SOPSaveData)bformatter.Deserialize(stream);
		stream.Close();

		if(loadData != null){
			if(loadData.holders != null && loadData.holders.Count > 0){
				SOHolder newHolder = loadData.holders[0];

				if(newHolder.typeName != null){
					T newSO = ScriptableObject.CreateInstance<T>();
					if(newSO != null){
						Type mType = newSO.GetType();
						for(int i=0; newHolder.stringNames != null && i<newHolder.stringNames.Length; i++){
							mType.GetField(newHolder.stringNames[i]).SetValue(newSO, newHolder.stringValues[i]);
						}

						for(int i=0; newHolder.intNames != null && i<newHolder.intNames.Length; i++){
							mType.GetField(newHolder.intNames[i]).SetValue(newSO, newHolder.intValues[i]);
						}

						for(int i=0; newHolder.stringArrayNames != null && i<newHolder.stringArrayNames.Length; i++){
							mType.GetField(newHolder.stringArrayNames[i]).SetValue(newSO, newHolder.stringArrayValues[i]);
						}

						for(int i=0; newHolder.intArrayNames != null && i<newHolder.intArrayValues.Length; i++){
							mType.GetField(newHolder.intArrayNames[i]).SetValue(newSO, newHolder.intArrayValues[i]);
						}

						for(int i=0; newHolder.stringListNames != null && i<newHolder.stringListNames.Length; i++){
							mType.GetField(newHolder.stringListNames[i]).SetValue(newSO, newHolder.stringListValues[i]);
						}

                        for (int i = 0; newHolder.contactNames != null && i < newHolder.contactNames.Length; i++) {
                            string path = "Apps/ChatApp/Contacts/" + newHolder.contactValues[i];
                            mType.GetField(newHolder.contactNames[i]).SetValue(newSO, Resources.Load<GamePhone.Contact>(path));
                        }

                        for (int i = 0; newHolder.chatNames != null && i < newHolder.chatNames.Length; i++) {
                            string path = "Apps/ChatApp/Chats/" + newHolder.chatValues[i];
                            mType.GetField(newHolder.chatNames[i]).SetValue(newSO, Resources.Load<GamePhone.Chat>(path));
                        }

                        return newSO;
					}
				}
			}
		}
		return null;
	}

}

[Serializable]
public class SOPSaveData : ISerializable {
	public List<SOHolder> holders = new List<SOHolder>();

	public SOPSaveData () {}

	public SOPSaveData (SerializationInfo info, StreamingContext context) {
		holders = (List<SOHolder>)info.GetValue("holders", typeof(List<SOHolder>));
	}

	public void GetObjectData (SerializationInfo info, StreamingContext ctxt) {
		info.AddValue("holders", holders);
	}
}

[Serializable]
public class SOHolder {
	public string typeName;

	public string[] intNames;
	public int[] intValues;

	public string[] intArrayNames;
	public int[][] intArrayValues;

	public string[] floatNames;
	public float[] floatValues;

	public string[] stringNames;
	public string[] stringValues;

	public string[] stringArrayNames;
	public string[][] stringArrayValues;

	public string[] stringListNames;
	public List<string>[] stringListValues;

	public string[] boolNames;
	public bool[] boolValues;

	public string[] contactNames;
	public string[] contactValues;

    public string[] chatNames;
    public string[] chatValues;
}