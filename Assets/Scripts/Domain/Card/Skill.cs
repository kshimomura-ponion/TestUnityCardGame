using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill
{
    Ability ability = Ability.None;
    Spell spell = Spell.None;

    public Skill(Ability a)
    {
        ability = a;
    }

    public Skill(Spell s)
    {
        spell = s;
    }

    public Ability GetAbility()
    {
        return ability;
    }

    public Spell GetSpell()
    {
        return spell;
    }
}
