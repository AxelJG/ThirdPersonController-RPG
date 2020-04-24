using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Character", menuName = "Character")]
public class Character : ScriptableObject
{
    [Header("General")]
    public Sprite faceImage;
    public string nameCh;
    public string surname;
    public enum WeaponType { shortDistance, largeDistance };
    public WeaponType weaponType;

    [Header("Stats")]
    public int exp;
    public int level = 1;
    public float health;
    public float mana;
    public enum Job { Soldier, DarkMage, Thief, WhiteMage, Sniper }
    public Job job;

    [Header("Adapt the position of the weapon where it draws")]
    public Vector3 adaptWeaponToHand;

    [Header("Adapt the weapon to the hand that takes it")]
    public Vector3 grabWeaponPosition;
    public Vector3 grabWeaponRotation;

    [Header("Adapt the weapon where it rests")]
    public Vector3 saveWeaponPosition;
    public Vector3 saveWeaponRotation;
}
