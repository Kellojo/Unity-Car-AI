using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class FileManager {

    public void SaveData(string s, string filename) {
        using (var writer = new BinaryWriter(File.OpenWrite(filename))) {
            writer.Write(s);
            writer.Close();
        }
    }

    public string LoadData(string mapName) {
        string s = "";
        using (var reader = new BinaryReader(File.OpenRead(mapName))) {
            s = reader.ReadString();
            reader.Close();
        }
        return s;
    }
}
