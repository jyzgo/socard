using UnityEngine;
using System.Collections;
using System.IO;
using MTUnity.Utils;

public class SeedMgr : MonoBehaviour {


    public string seedFileName = string.Empty;

    public static SeedMgr current;
    // Use this for initialization
    void Awake()
    {
        current = this;
    }
    void Start() {

    }

    // Update is called once per frame
    void Update() {

    }

    public void LoadFrom()
    {
        var path = Application.persistentDataPath + "/" + seedFileName;
        string content = File.ReadAllText(path);
        Debug.Log("load from " + path);

        MTJSONObject js = MTJSON.Deserialize(content);
        LevelMgr.current.LoadGame(js);
    }


}
