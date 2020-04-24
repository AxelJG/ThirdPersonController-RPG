using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerMovement : MonoBehaviour
{
    #region VARIABLES
    PlayerManager playerManager;

    [Header("Interaction with objects")]
    public GameObject weapon;
    public Transform hand, back;

    [Header("Controlled by player")]
    public float speed = 6.0F;
    public float gravity = 9.8f;
    public Transform cameraTransform;
    public Animator animator;

    [Header("Controlled by AI")]
    public Transform target;
    
    //Controlled
    private Vector3 moveDirection = Vector3.zero;
    private CharacterController characterController;

    //AI
    private NavMeshAgent navMeshAgent;
    #endregion

    #region BASE MEDTHODS
    private void Start() {
        playerManager = GetComponent<PlayerManager>();
        characterController = GetComponent<CharacterController>();
        navMeshAgent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        switch (playerManager.mode) {
            case PlayerManager.CharacterMode.Controlled:
                CharacterUsed();
                break;
            case PlayerManager.CharacterMode.AI:
                if(target != null) CharacterAI();
                break;
        }

    }
    #endregion

    #region CHARACTER CONTROLLED METHODS
    //Movimiento del personaje controlado por el jugador
    private void CharacterUsed() {
        Quaternion playerRotation = transform.rotation;

        if (Input.GetAxis("Vertical") != 0f || Input.GetAxis("Horizontal") != 0f) {
            playerRotation = Quaternion.LookRotation(new Vector3(cameraTransform.forward.x, 0f, cameraTransform.forward.z)); //Character mira hacia donde mira camara

            this.GetComponent<CharacterController>().enabled = true;
            this.GetComponent<NavMeshAgent>().enabled = false;
        }

        //Movimiento del player con CharacterController
        moveDirection.y -= gravity * Time.deltaTime; //Aplicar primero siempre la gravedad para el isGrounded

        if (characterController.isGrounded) {//Si el personaje esta en el suelo
            float inputV = Input.GetAxis("Vertical") >= 0f ? Input.GetAxis("Vertical") : Input.GetAxis("Vertical") / 2;
            moveDirection = new Vector3(Input.GetAxis("Horizontal") / 2, 0, inputV);
            moveDirection = new Vector3(cameraTransform.TransformDirection(moveDirection).x, 0f, cameraTransform.TransformDirection(moveDirection).z);
            moveDirection *= speed;
        }

        if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Sheath A Sword") && !animator.GetCurrentAnimatorStateInfo(0).IsName("Draw A Weapon") 
            && this.GetComponent<CharacterController>().enabled && !animator.GetCurrentAnimatorStateInfo(0).IsName("Spell Cast")) {

            characterController.Move(moveDirection * Time.deltaTime);
            transform.rotation = playerRotation;

            animator.SetFloat("Speed", Input.GetAxis("Vertical"));
            animator.SetFloat("Lateral", Input.GetAxis("Horizontal"));
        }

    }
    #endregion

    #region CHARACTER AI METHODS

    private void CharacterAI() {
        navMeshAgent.SetDestination(target.position);

        animator.SetFloat("Speed", navMeshAgent.velocity.magnitude / navMeshAgent.speed);
    }

    public void AIConfiguration() {
        characterController.enabled = false;
        navMeshAgent.enabled = true;
    }

    public void ControlledConfiguration() {
        characterController.enabled = true;
        navMeshAgent.enabled = false;
    }

    #endregion

    #region ITEMS MOVEMENT

    //Anclamos el arma a la mano
    public void GrabWeapon() {
        weapon.transform.parent = hand;
        weapon.transform.localPosition = new Vector3(0.803f, 0.208f, 1.267f);
        weapon.transform.Rotate(weapon.transform.rotation.x, weapon.transform.rotation.y, 20f);
    }

    //Anclamos el arma en su lugar de reposo
    public void SaveWeapon() {
        weapon.transform.parent = back;
        weapon.transform.localPosition = new Vector3(-0.399f, -0.892f, -0.241f);
        weapon.transform.localEulerAngles = new Vector3(111.654f, 81.077f, 85.92699f);
    }

    //Se posiciona el arma para poder ser agarrada
    public IEnumerator AdaptWeaponToHand(float time, Vector3 endPos) {
        float rateTiempo = 1f / time;
        float t = 0.0f;
        Vector3 startPos = weapon.transform.localPosition;

        while (t <= 1f) {
            t += Time.deltaTime * rateTiempo;
            weapon.transform.localPosition = Vector3.Lerp(startPos, endPos, t);
            yield return null;
        }

        yield return null;
    }

    #endregion
}
