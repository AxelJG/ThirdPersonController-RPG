using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerManager : MonoBehaviour
{
    [Header("Player Configuration")]
    public float rayMouseLenght;
    public LayerMask layerMaskMouse;
    public enum CharacterMode { Controlled, AI };
    public CharacterMode mode;
    public Character character;

    private bool fightMode = false;
    private Transform elementSelected;
    private PlayerCombatController playerCombatController;

    //Variables para el control de estres del las key's
    private bool stressKey = true;
    private const float BASE_STRESS_TIME = 1f;
    private float stressTime = BASE_STRESS_TIME;

    private void Start() {
        elementSelected = this.transform;
        playerCombatController = GetComponent<PlayerCombatController>();
    }

    private void Update() {
        //Mouse
        if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject() && mode == CharacterMode.Controlled) {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit, rayMouseLenght, layerMaskMouse)) {
                elementSelected = hit.transform;

                if (!fightMode && elementSelected.tag.Equals("Enemy")) { //Si hemos hecho click sobre un enemigo, preparamos arma
                    playerCombatController.PrepareWeapon();
                }
            }
        }

        //Keyboard
        if (Input.GetKeyDown(KeyCode.Alpha1) && elementSelected.tag.Equals("Enemy") && mode == CharacterMode.Controlled && stressKey) {
            StartCoroutine(playerCombatController.AttackSimple());
            stressKey = false;
        }

        if (Input.GetKeyDown(KeyCode.Alpha2) && elementSelected.tag.Equals("Enemy") && mode == CharacterMode.Controlled && stressKey) {
            playerCombatController.SimpleMagicAttack();
            stressKey = false;
        }

        StressKeyTimer();
    }

    private void StressKeyTimer() {
        if (!stressKey) {
            if (stressTime > 0f) {
                stressTime -= Time.deltaTime;
            } else {
                stressKey = true;
                stressTime = BASE_STRESS_TIME;
            }
        }
    }

    public bool FightMode { get => fightMode; set => fightMode = value; }
    public Transform ElementSelected { get => elementSelected; set => elementSelected = value; }
}
