public enum Ability
{
    None,
    InitAttackable,    // 場に出してすぐ攻撃可能（速攻カード）
    Shield,             // プレイヤーや他のカードを守る（シールドカード）
}

public enum Spell
{
    None,
    AttackEnemyCard,
    AttackEnemyCards,
    AttackEnemyHero,
    HealFriendCard,
    HealFriendCards,
    HealFriendHero,
}

// カードタイプがモンスター、か呪文なのか
public enum CardType
{
    Monster,
    Spell
}
