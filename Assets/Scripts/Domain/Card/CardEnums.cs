public enum ABILITY
{
    NONE,
    INIT_ATTACKABLE,    // 場に出してすぐ攻撃可能（速攻カード）
    SHIELD,             // プレイヤーや他のカードを守る（シールドカード）
}

public enum SPELL
{
    NONE,
    DAMAGE_ENEMY_CARD,
    DAMAGE_ENEMY_CARDS,
    DAMAGE_ENEMY_HERO,
    HEAL_FRIEND_CARD,
    HEAL_FRIEND_CARDS,
    HEAL_FRIEND_HERO,
}

// カードタイプがモンスター、か呪文なのか
public enum CARDTYPE
{
    MONSTER,
    SPELL
}
