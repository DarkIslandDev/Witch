using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class CharacterSelector : MonoBehaviour
{
    [SerializeField] protected CharacterBlueprint[] characterBlueprints;
    [SerializeField] protected GameObject characterCardPrefab;
    [SerializeField] protected CoinDisplay coinDisplay;

    public CharacterStats characterStats;

    public void Init()
    {
        for (int i = 0; i < characterBlueprints.Length; i++)
        {
            CharacterCard characterCard = Instantiate(characterCardPrefab, this.transform).GetComponent<CharacterCard>();
            characterCard.Init(this, characterBlueprints[i], coinDisplay);
            
            
        }
        
        characterStats.Init(characterBlueprints[0]);
    }
        
    public void StartGame(CharacterBlueprint characterBlueprint)
    {
        CrossSceneData.CharacterBlueprint = characterBlueprint;
        SceneManager.LoadScene(1);
    }
}