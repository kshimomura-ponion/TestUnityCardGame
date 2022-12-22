using UnityEngine;
using DG.Tweening;
using TestUnityCardGame.View.Card;

namespace TestUnityCardGame
{
    public class CardController : MonoBehaviour
    {
        [SerializeField] AudioManager audioManager;
        [System.NonSerialized] public CardView view;
        [System.NonSerialized] public CardModel model;
        [System.NonSerialized] public CardMovement movement;
        private float xDestination = 0.0f;

        private Player owner;
        private bool isDraggable;

        private void Awake()
        {
            view = GetComponent<CardView>();
            movement = GetComponent<CardMovement>();
            isDraggable = false;
        }

        public void Init(CardEntity cardEntity, Player player)
        {
            model = new CardModel(cardEntity);
            owner = player;
            view.Show(model);
        }

        public int Attack(CardController enemyCard)
        {
            audioManager.PlaySE(SE.Attack);
            int at = model.Attack(enemyCard);
            view.SetActiveSelectablePanel(false);
            SetCanAttack(false);
            return at;
        }

        public void Heal(CardController friendCard)
        {
            audioManager.PlaySE(SE.Heal);
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
            audioManager.PlaySE(SE.Died);
            Instantiate(view.explosionParticle, transform.position, view.explosionParticle.transform.rotation);
            Destroy(this.gameObject);
        }

        public void UseSpellTo(CardController targetCard, HeroController ownerHero)
        {
            switch (model.GetSpell()) {
                case SPELL.DAMAGE_ENEMY_CARD:
                    // 特定の敵を攻撃する
                    if (targetCard == null)
                    {
                        return;
                    }
                    if (targetCard.GetOwner() == owner)
                    {
                        return;
                    }
                    Attack(targetCard);
                    targetCard.CheckAlive();
                    break;
                case SPELL.HEAL_FRIEND_CARD:
                    if (targetCard == null)
                    {
                        return;
                    }
                    if (targetCard.GetOwner() != owner)
                    {
                        return;
                    }
                    Heal(targetCard);
                    break;
                case SPELL.NONE:
                    break;
            }

            // カード所有者のマナコストを減少させる
            ownerHero.model.ReduceManaCost(model.GetManaCost());
        }

        public void UseSpellTo(CardController[] targetCards, HeroController ownerHero)
        {
            switch (model.GetSpell()) {
                case SPELL.DAMAGE_ENEMY_CARDS:
                    // 相手フィールドの全てのカードに攻撃する
                    foreach (CardController targetCard in targetCards)
                    {
                        Attack(targetCard);
                    }
                    foreach (CardController targetCard in targetCards)
                    {
                        targetCard.CheckAlive();
                    }
                    break;
                case SPELL.HEAL_FRIEND_CARDS:
                    foreach (CardController targetCard in targetCards)
                    {
                        Heal(targetCard);
                    }
                    break;
                case SPELL.NONE:
                    break;
            }

            // カード所有者のマナコストを減少させる
            ownerHero.model.ReduceManaCost(model.GetManaCost());
        }

        public void UseSpellTo(HeroController target, HeroController ownerHero)
        {
            switch (model.GetSpell()) {
                case SPELL.DAMAGE_ENEMY_HERO:
                    target.Attacked(this);
                    break;
                case SPELL.HEAL_FRIEND_HERO:
                    target.Healed(this);
                    break;
                case SPELL.NONE:
                    break;
            }

            // カード所有者のマナコストを減少させる
            ownerHero.model.ReduceManaCost(model.GetManaCost());
        }

        public void DestroyUsedSpellCard()
        {
            if(model.GetSpell() != SPELL.NONE) {
                Destroy(this.gameObject);   // スペルカードの破棄
            }
        }

        public Player GetOwner()
        {
            return owner;
        }
        public bool IsDraggable()
        {
            return isDraggable;
        }
        public void SetDraggable(bool isDraggable)
        {
            this.isDraggable = isDraggable;
        }
    }
}