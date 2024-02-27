using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(RoomGenerator), true)]
public class RoomDungeonGeneratorEditor : Editor
{
    private RoomGenerator generator;
    
    private void Awake()
    {
        generator = (RoomGenerator)target;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        
        if (GUILayout.Button("Create dungeon"))
        {
            generator.Clear();
            generator.GenerateRoom();
        }

        if (GUILayout.Button("Clear field"))
        {
            generator.Clear();
        }
    }
}