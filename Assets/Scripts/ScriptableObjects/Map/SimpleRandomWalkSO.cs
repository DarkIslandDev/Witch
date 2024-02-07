using UnityEngine;

[CreateAssetMenu(menuName = "Map/SimpleRandomWalkSO", fileName = "SimpleRandomWalkParameters_", order = 0)]
public class SimpleRandomWalkSO : ScriptableObject
{
    public int iterations = 10;
    public int walkLenght = 10;
    public bool startRandomlyEachIteration = true;
}