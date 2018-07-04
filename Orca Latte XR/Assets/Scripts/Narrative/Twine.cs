using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

[System.Serializable]
public class Twine {
    public TwinePassage[] passages;
    public string name;
    public int startnode;

    /*#if UNITY_EDITOR

    [MenuItem("Tools/Twine/Import")]
    public static void ImportStory() {
        string path = EditorUtility.OpenFilePanel("Import twine story", "Assets/Stories/", "json");
        if (path != null) {
            using (StreamReader r = new StreamReader(path)) {
                string json = r.ReadToEnd();
                Story newStory = ImportStory(json);

                string targetPath = EditorUtility.SaveFilePanel("Save Story", "Assets/", "NewTwineStory", "asset");
                string relativepath = "Assets" + targetPath.Substring(Application.dataPath.Length);


                AssetDatabase.CreateAsset(newStory, relativepath);

                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
        }
    }

#endif*/

    public static Story ImportStory(string json)
    {
        Twine twine = JsonUtility.FromJson<Twine>(json);

        Story newStory = new Story();

        List<Passage> passages = new List<Passage>();
        foreach (TwinePassage p in twine.passages)
        {
            List<Decision> decisions = new List<Decision>();
            if (p.links != null)
            {
                foreach (TwineLink l in p.links)
                {
                    Decision decision = new Decision();
                    decision.name = l.link;
                    decision.message = l.name;
                    decision.link = l.pid;

                    decisions.Add(decision);
                }
            }

            Passage passage = new Passage();

            passage.name = p.name;
            passage.id = p.pid;
            passage.message = p.text;
            passage.decisions = decisions.ToArray();

            passages.Add(passage);
        }

        newStory.name = twine.name;
        newStory.passages = passages.ToArray();
        newStory.startPassage = twine.startnode;
        newStory.currentPassage = twine.startnode;

        return newStory;
    }
}

[System.Serializable]
public class TwinePassage {
    public string text;
    public TwineLink[] links;
    public string name;
    public int pid;
    public TwinePosition position;
}

[System.Serializable]
public class TwineLink {
    public string name;
    public string link;
    public int pid;
}

[System.Serializable]
public class TwinePosition
{
    public int x;
    public int y;
}