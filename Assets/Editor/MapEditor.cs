using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor (typeof(MapGenerator))]
public class MapEditor : Editor {

    public override void OnInspectorGUI()
    {
        MapGenerator MapGen = target as MapGenerator;
        if(DrawDefaultInspector())
        {
            MapGen.GenerateMap();
        }

        if(GUILayout.Button("Generate Map"))
        {
            MapGen.GenerateMap();
        }
    }
}
