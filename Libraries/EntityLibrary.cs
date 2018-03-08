using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EntityLibrary : MonoBehaviour {

    public static EntityLibrary instance;

    public Entity[] players;
    public Entity[] enemies;

    List<Entity> entities = new List<Entity>();


	void Awake() {
        if(instance == null)
            instance = this;

        foreach(Entity p in players) {
            entities.Add(p);
        }
        foreach(Entity e in enemies) {
            entities.Add(e);
        }
    }

    public Entity GetEntity(string id) {
        foreach(Entity e in entities) {
            if(e.id == id)
                return e;
        }
        Debug.LogError("No entity with id \"" + id + "\" found.");
        return null;
    }
}