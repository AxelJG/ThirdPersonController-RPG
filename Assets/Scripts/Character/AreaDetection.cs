using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaDetection : MonoBehaviour {

    private PlayerCombatController playerCombatController;

    private void Awake() {
        playerCombatController = transform.parent.GetComponent<PlayerCombatController>(); 
    } 

    private void OnTriggerEnter(Collider other) {
        if(other.tag.Equals("Enemy")){
            playerCombatController.setEnemyDetect(other.transform);
        }
    }
}