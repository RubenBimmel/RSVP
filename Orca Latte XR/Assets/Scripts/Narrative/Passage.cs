using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[System.Serializable]
public class Passage {
    public string name;
    public int id;
    public string message;
    public Decision[] decisions;

    public string[] GetMessages () {
        List<string> messages = message.Split('\n').ToList();
        List<string> messagesToRemove = new List<string>();
        foreach (string m in messages) {
            if (m.Length <= 1 || (m[0] == '[' && m[1] == '[')) {
                messagesToRemove.Add(m);
            }
        }
        foreach (string m in messagesToRemove) {
            messages.Remove(m);
        }
        return messages.ToArray();
    }
}
