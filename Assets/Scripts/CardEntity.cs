using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName="CardEntity", menuName="Create CardEntity")]
public class CardEntity : ScriptableObject
{
    public new string name; // stringクラスはインスタンス化する必要がある
    public int hp;
    public int at;
    public int cost;
    public Sprite icon;
}
