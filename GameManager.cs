using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour {

    public static GameManager instance;

    public List<Entity> players = new List<Entity>();


    void Awake() {

        if(instance == null) {
            instance = this;
            DontDestroyOnLoad(transform.parent);
        }
        else
            GameObject.DestroyImmediate(transform.parent.gameObject);
    }

    void Start() {

        Entity zidane = EntityLibrary.instance.GetEntity("zidane");
        zidane.stats.health = zidane.stats.maxHealth;
        zidane.stats.mana = zidane.stats.maxMana;

        players.Add(EntityLibrary.instance.GetEntity("zidane"));

        //StartGame();
        EnterCombat();
        //BattleManager.instance.StartBattle();
    }

    public void QuitGame() {

        Application.Quit();
    }

    public void StartGame() {

        SceneManager.LoadScene("Overworld");
        SceneManager.sceneLoaded += CallbackOverworld;
    }

    void CallbackOverworld(Scene scene, LoadSceneMode mode) {

        OverworldManager.instance.SpawnPlayer();
        SceneManager.sceneLoaded -= CallbackOverworld;
    }

    void CallbackBattle(Scene scene, LoadSceneMode mode) {

        BattleData bd = BattleDataLibrary.instance.GetBattleData("default");

        BattleManager.instance.StartBattle(bd, players);
        SceneManager.sceneLoaded -= CallbackBattle;
    }

    public void EnterCombat() {

        SceneManager.LoadScene("Battle");
        SceneManager.sceneLoaded += CallbackBattle;
    }
}
