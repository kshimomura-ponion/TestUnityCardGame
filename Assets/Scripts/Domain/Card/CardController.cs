using UnityEngine;
using DG.Tweening;

namespace TestUnityCardGame
{
    public class CardController : MonoBehaviour
    {
        [System.NonSerialized] public CardView view;
        [System.NonSerialized] public CardModel model;
        [System.NonSerialized] public CardMovement movement;
        private float xDestination = 0.0f;

        private PLAYER owner;

        private void Awake()
        {
            view = GetComponent<CardView>();
            movement = GetComponent<CardMovement>();
        }

        public void Init(CardEntity cardEntity, PLAYER player)
        {
            model = new CardModel(cardEntity);
            owner = player;
            view.Show(model);
        }

        public int Attack(CardController enemyCard)
        {
            SoundManager.instance.PlaySE(SE.ATTACK);
            int at = model.Attack(enemyCard);
            view.SetActiveSelectablePanel(false);
            SetCanAttack(false);
            return at;
        }

        public void Heal(CardController friendCard)
        {
            SoundManager.instance.PlaySE(SE.HEAL);
            model.Heal(friendCard);
            friendCard.view.Refresh(model);
        }

        public void SetCanAttack(bool canAttack)
        {
            model.SetCanAttack(canAttack);
            view.SetActiveSelectablePanel(canAttack);
        }

        public void CheckAlive()
        {
            view.GetDamageInfo().SetActive(true);
            RefreshOrDestoroy();
        }

        void RefreshOrDestoroy()
        {
            view.GetDamageInfo().transform.DOLocalMove(new Vector3(0f,20.0f,0f), 0.5f).SetEase(Ease.InOutQuart).OnComplete(MoveXAxis);

            // hpが0になったらオブジェクトを消す
            if(model.IsAlive()) {
                view.Refresh(model);
            } else {
                Invoke("DestroyCard", 0.5f);
            }
        }

        void MoveXAxis(){
            xDestination = transform.position.x - 25;
            XAxisTransForm();
            xDestination = transform.position.x + 25;
            Invoke("XAxisTransForm", 0.1f);
            xDestination = transform.position.x + 25;
            Invoke("XAxisTransForm", 0.1f);
            xDestination = transform.position.x - 25;
            Invoke("XAxisTransForm", 0.1f);
            RewindDamageInfo();
        }

        void XAxisTransForm()
        {
            transform.DOLocalMove(new Vector3(xDestination,0f,0f), 0.02f);
        }

        void RewindDamageInfo()
        {
            view.GetDamageInfo().SetActive(false);
            view.GetDamageInfo().transform.DORewind();
            xDestination = 0.0f;
        }

        void DestroyCard()
        {
            SoundManager.instance.PlaySE(SE.DIED);
            Instantiate(view.explosionParticle, transform.position, view.explosionParticle.transform.rotation);
            Destroy(this.gameObject);
        }

        public bool CanUseSpell()
        {
            switch (model.GetSpell()) {
                case SPELL.DAMAGE_ENEMY_CARD:
                case SPELL.DAMAGE_ENEMY_CARDS:
                    // 相手フィールドの全てのカードに攻撃する
                    // CardController[] enemyCards = gameManager.GetOpponentFieldCards(owner);
                    /*if (enemyCards.Length > 0)
                    {
                        return true;
                    }*/
                    return false;
                case SPELL.DAMAGE_ENEMY_HERO:
                case SPELL.HEAL_FRIEND_HERO:
                    return true;
                case SPELL.HEAL_FRIEND_CARD:
                case SPELL.HEAL_FRIEND_CARDS:
                    // CardController[] friendCards = gameManager.GetFriendFieldCards(owner);
                    /*if (friendCards.Length > 0)
                    {
                        return true;
                    }*/
                    return false;
                case SPELL.NONE:
                    return false;
            }
            return false;
        }

        public void UseSpellTo(CardController target, HeroController hero)
        {
            switch (model.GetSpell()) {
                case SPELL.DAMAGE_ENEMY_CARD:
                    // 特定の敵を攻撃する
                    if (target == null)
                    {
                        return;
                    }
                    if (target.GetOwner() == owner)
                    {
                        return;
                    }
                    Attack(target);
                    target.CheckAlive();
                    break;
                case SPELL.DAMAGE_ENEMY_CARDS:
                    // 相手フィールドの全てのカードに攻撃する
                    // CardController[] enemyCards = GameManager.instance.GetOpponentFieldCards(owner);
                    /*foreach (CardController enemyCard in enemyCards)
                    {
                        Attack(enemyCard);
                    }
                    foreach (CardController enemyCard in enemyCards)
                    {
                        enemyCard.CheckAlive();
                    }*/
                    break;
                case SPELL.DAMAGE_ENEMY_HERO:
                    if(owner == PLAYER.PLAYER1){
                        // gameManager.player2Hero.Attacked(this);
                    } else {
                        // gameManager.player1Hero.Attacked(this);
                    }
                    break;
                case SPELL.HEAL_FRIEND_CARD:
                    if (target == null)
                    {
                        return;
                    }
                    if (target.GetOwner() != owner)
                    {
                        return;
                    }
                    Heal(target);
                    break;
                case SPELL.HEAL_FRIEND_CARDS:
                    // CardController[] friendCards = gameManager.GetFriendFieldCards(owner);
                    /*foreach (CardController friendCard in friendCards)
                    {
                        Heal(friendCard);
                    }*/
                    break;
                case SPELL.HEAL_FRIEND_HERO:
                    // gameManager.turnController.HealToHero(this);
                    break;
                case SPELL.NONE:
                    break;
            }

            // カード所有者のマナコストを減少させる
            hero.model.ReduceManaCost(model.GetManaCost());

            // スペルカードは使用したらすぐに破棄する
            Destroy(this.gameObject);
        }

        public PLAYER GetOwner()
        {
            return owner;
        }
    }
}