using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BattleDataLibrary : MonoBehaviour {

    public static BattleDataLibrary instance;

    public BattleData[] battles;

    List<BattleData> battle = new List<BattleData>();


    void Awake() {
        if(instance == null)
            instance = this;

        foreach(BattleData bd in battles) {
            battle.Add(bd);
        }
    }

    public BattleData GetBattleData(string id) {
        foreach(BattleData bd in battle) {
            if(bd.id == id)
                return bd;
        }
        Debug.LogError("No battle data with id \"" + id + "\" found.");
        return null;
    }
}