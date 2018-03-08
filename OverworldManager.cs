using UnityEngine;
using System.Collections;

public class OverworldManager : MonoBehaviour {

    public static OverworldManager instance;

    public GameObject dummyCharacterGraphics;

    public GameObject fang;
    public Transform characterSpawnPoint;

    OverworldEntity player;

    Vector2 input;
    Vector2 dir = new Vector2(1, 0);


    void Awake() {
        if(instance == null)
            instance = this;
    }

    public void ICollided() {
        GameManager.instance.EnterCombat();
    }

    void Update() {

        if(player != null) {

            Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position, player.transform.position + Vector3.up * 5.5f + Vector3.back * 2f, 10f * Time.deltaTime);

            input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

            if(input.magnitude > 0)
                dir = input.normalized;

            Quaternion targ = Quaternion.LookRotation(new Vector3(dir.x, 0f, dir.y));
            player.transform.rotation = Quaternion.Slerp(player.transform.rotation, targ, 10f * Time.deltaTime);

            player.transform.position = Vector3.Lerp(player.transform.position, player.transform.position + new Vector3(input.x, 0f, input.y), 5f * Time.deltaTime);
        }
    }

    public void SpawnPlayer() {

        player = SpawnOverworldEntity();
        player.Initialize(EntityLibrary.instance.GetEntity("zidane"), characterSpawnPoint.position);
    }

    OverworldEntity SpawnOverworldEntity() {

        GameObject newEntity = new GameObject();

        return newEntity.AddComponent<OverworldEntity>();
    }
}
