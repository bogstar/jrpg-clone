using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace JRPG {

    public class TerrainLibrary : MonoBehaviour {

        public static TerrainLibrary instance;

        public JRPG.Terrain[] terrains;

        List<JRPG.Terrain> terrain = new List<JRPG.Terrain>();


        void Awake() {
            if(instance == null)
                instance = this;

            foreach(JRPG.Terrain t in terrains) {
                terrain.Add(t);
            }
        }

        public JRPG.Terrain GetTerrain(string id) {
            foreach(JRPG.Terrain t in terrain) {
                if(t.id == id)
                    return t;
            }
            Debug.LogError("No entity with id \"" + id + "\" found.");
            return null;
        }
    }
}