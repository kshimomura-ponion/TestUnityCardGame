using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName="MonsterCardEntity", menuName="Create MonsterCardEntity")]
public class MonsterCardEntity : ScriptableObject
{
    public int id;
    public new string name; // stringクラスはインスタンス化する必要がある
    public int hp;
    public int at;
    public int manaCost;
    public Sprite icon;
    public Ability ability;
    public Sprite abilityIcon;
}
