using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName="HeroEntity", menuName="Create HeroEntity")]
public class HeroEntity : ScriptableObject
{
    public new string name; // stringクラスはインスタンス化する必要がある
    public int hp;
    public Sprite leftIcon;  // ->向きの絵（Player用）
    public Sprite rightIcon; // <-向きの絵（Enemy用）
}
