using UnityEngine;
using System.Collections;

public class OverworldEntity : MonoBehaviour {

    public Entity entity;
    public Rigidbody rbd;
    public BoxCollider col;

    public float modelHeight;

    public void Initialize(Entity entity, Vector3 pos) {

        this.entity = entity;
        GameObject model;

        if(entity == null)
            return;

        if(entity.graphics != null) {
            model = entity.graphics;
            modelHeight = entity.height;
        }
        else {
            Debug.Log("Character \"" + entity.id + "\" has no graphics. Using Dummy Character as graphics.");
            model = OverworldManager.instance.dummyCharacterGraphics;
            modelHeight = 1f;
        }

        transform.name = entity.name;
        transform.position = pos + Vector3.up * modelHeight / 2f;

        GameObject graphics = (GameObject)GameObject.Instantiate(model, transform);
        graphics.name = "Graphics";
        graphics.transform.localPosition = Vector3.zero;

        rbd = gameObject.AddComponent<Rigidbody>();
        rbd.isKinematic = true;

        col = gameObject.AddComponent<BoxCollider>();
        col.isTrigger = true;
        col.center = new Vector3(0f, -0.05f, 0f);
        col.size = new Vector3(0.25f, .85f, .25f);
    }
}