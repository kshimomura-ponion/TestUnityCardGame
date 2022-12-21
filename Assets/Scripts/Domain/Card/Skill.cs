using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill
{
    ABILITY ability = ABILITY.NONE;
    SPELL spell = SPELL.NONE;

    public Skill(ABILITY a)
    {
        ability = a;
    }

    public Skill(SPELL s)
    {
        spell = s;
    }

    public ABILITY GetAbility()
    {
        return ability;
    }

    public SPELL GetSpell()
    {
        return spell;
    }
}
