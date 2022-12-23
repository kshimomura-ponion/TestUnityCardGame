using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName="SpellCardEntity", menuName="Create SpellCardEntity")]
public class SpellCardEntity : ScriptableObject
{
    public int id;
    public new string name; // stringクラスはインスタンス化する必要がある
    public int at;
    public int manaCost;
    public Sprite icon;
    public Spell spell;
    public Sprite spellIcon;
}
