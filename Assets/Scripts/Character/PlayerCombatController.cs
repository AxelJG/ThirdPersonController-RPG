using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerCombatController : MonoBehaviour
{
    #region VARIABLES
    [Header("MAGIC")]
    public GameObject magicHandEffect;
    public GameObject magicProjectile;
    public GameObject magicHand;

    [Header("PROJECTILE'S")]
    public GameObject projectile;
    public GameObject muzzleFlare;
    public GameObject bulletSpawnPoint;
    public GameObject muzzleSpawnPoint;

    private TroopManager troopManager;
    private PlayerManager playerManager;
    private PlayerMovement playerMovement;
    private Animator animator;
    private NavMeshAgent navMeshAgent;

    //Tiempo que el jugador se encuentra sin atacar
    private const float INACTIVE_TIME = 15f;
    [SerializeField]
    private float inactiveTime = INACTIVE_TIME;

    //Para que IA compruebe si el arma ya esta lista o no
    private bool weaponReady = false;
    #endregion

    #region BASE METHODS
    void Start()
    {
        playerManager = GetComponent<PlayerManager>();
        playerMovement = GetComponent<PlayerMovement>();
        troopManager = Camera.main.transform.parent.GetComponent<TroopManager>();
        animator = GetComponent<Animator>();
        navMeshAgent = GetComponent<NavMeshAgent>();
    }

    private void Update() {
        if(navMeshAgent.enabled == true && playerManager.mode == PlayerManager.CharacterMode.Controlled) { //Control de velocidad de animacion PJ con navmesh cuando atacamos
            animator.SetFloat("Speed", navMeshAgent.velocity.magnitude / navMeshAgent.speed);
        }
    }
    #endregion

    #region COMBAT STATES
    //Preparamos el arma para el combate
    public void PrepareWeapon() {

        playerManager.FightMode = true;
        StartCoroutine(playerMovement.AdaptWeaponToHand(0.5f, playerManager.character.adaptWeaponToHand));
        animator.SetBool("FightMode", true);
        troopManager.Encounter = true;

        weaponReady = true;
        StartCoroutine(InactiveTimeAttack());
    }

    //Pasado un tiempo si no hay ataque pasa a modo reposo
    public IEnumerator InactiveTimeAttack() {
        inactiveTime = INACTIVE_TIME;

        while (inactiveTime > 0f) {
            inactiveTime -= Time.deltaTime;
            yield return null;
        }

        FinishFight();

    }
    #endregion

    #region ATTACKS
    //Ataque simple
    public IEnumerator AttackSimple() {
        this.GetComponent<CharacterController>().enabled = false;
        navMeshAgent.enabled = true;

        if(playerManager.character.weaponType == Character.WeaponType.shortDistance) { //Si el arma es a corta distancia
            navMeshAgent.SetDestination(playerManager.ElementSelected.position);

            while (Vector3.Distance(transform.position, playerManager.ElementSelected.position) > navMeshAgent.stoppingDistance) {
                yield return null;
            }
        } else { //Si el arma es a larga distancia
            navMeshAgent.SetDestination(this.transform.position);

            Vector3 lookAtVec = new Vector3(playerManager.ElementSelected.position.x, transform.position.y, playerManager.ElementSelected.position.z);
            transform.LookAt(lookAtVec);

            if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Draw A Weapon"))
                InstantiateBulletProjectile();
        }

        animator.SetTrigger("SimpleAttack");
        inactiveTime = INACTIVE_TIME;

    }

    //Ataque magico simple
    public void SimpleMagicAttack() {
        this.GetComponent<CharacterController>().enabled = false;
        navMeshAgent.enabled = true;
        navMeshAgent.SetDestination(transform.position);

        Vector3 lookAtVec = new Vector3(playerManager.ElementSelected.position.x, transform.position.y, playerManager.ElementSelected.position.z);
        transform.LookAt(lookAtVec);

        GameObject magicHandEffectObj = Instantiate(magicHandEffect, magicHand.transform);
        magicHandEffectObj.transform.localPosition = Vector3.zero;

        animator.SetTrigger("SimpleMagic");
        inactiveTime = INACTIVE_TIME;
    }

    public void FinishFight() {
        weaponReady = false;
        playerManager.FightMode = false;
        playerManager.ElementSelected = this.transform;
        animator.SetBool("FightMode", false);

        if(troopManager.Encounter)
            troopManager.Encounter = false;
    }
    #endregion

    #region INSTANTIATE PROJECTILES
    public void InstantiateMagicProjectile() {
        GameObject p = Instantiate(magicProjectile, magicHand.transform.position, transform.rotation);
    }

    private void InstantiateBulletProjectile() {
        GameObject bulletTrace = Instantiate(projectile, bulletSpawnPoint.transform.position, transform.rotation);
        bulletTrace.GetComponent<ExplodingProjectile>().missileTarget = playerManager.ElementSelected;

        Instantiate(muzzleFlare, muzzleSpawnPoint.transform.position, muzzleSpawnPoint.transform.rotation);
    }
    #endregion

    #region COMBAT AI MODE

    public IEnumerator FightSequence() {
        foreach (GameObject c in troopManager.playableCharacters) {
            if(c.GetComponent<PlayerManager>().mode == PlayerManager.CharacterMode.Controlled) {
                playerManager.ElementSelected = c.GetComponent<PlayerManager>().ElementSelected;
                break;
            }
        }

        while (troopManager.Encounter && playerManager.mode == PlayerManager.CharacterMode.AI) {
            if (!weaponReady) {
                PrepareWeapon();
                weaponReady = true;
                yield return new WaitForSeconds(1.2f);
            }

            yield return StartCoroutine(AttackSimple());
            yield return new WaitForSeconds(1.2f);
        }

        yield return null;
    }

    #endregion
}
