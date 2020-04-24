using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TroopManager : MonoBehaviour
{
    public List<GameObject> playableCharacters;
    public Transform nextEnemy;

    private CameraFollow cameraFollow;
    private int counterPlayableCharacter = 0;

    // Start is called before the first frame update
    void Start()
    {
        cameraFollow = GetComponent<CameraFollow>();
        cameraFollow.cameraFollowObj = playableCharacters[0].transform.Find("CameraFollow").gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab)) {
            if (playableCharacters.Count >= 2) {
                counterPlayableCharacter = counterPlayableCharacter == 0 ? 1 : 0;
                ChangePlayableCharacter(counterPlayableCharacter);
            }
        }
    }

    private void ChangePlayableCharacter(int chID) {
        cameraFollow.cameraFollowObj = playableCharacters[chID].transform.Find("CameraFollow").gameObject;

        foreach (GameObject p in playableCharacters) {
            p.GetComponent<PlayerManager>().mode = PlayerManager.CharacterMode.AI;
            p.GetComponent<PlayerMovement>().AIConfiguration();
        }
        playableCharacters[chID].GetComponent<PlayerManager>().mode = PlayerManager.CharacterMode.Controlled;
        playableCharacters[chID].GetComponent<PlayerMovement>().ControlledConfiguration();

    }
}
