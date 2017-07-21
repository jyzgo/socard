using UnityEngine;

namespace MTUnity.Actions
{
    public class CardFadeAction : MTFiniteTimeAction
    {
        #region Constructors

        public CardAbstract _card;
        public CardFadeAction(float duration, CardAbstract card) : base(duration)
        {
            _card = card;
        }

        #endregion Constructors


        protected internal override MTActionState StartAction(GameObject target)
        {
            return new CardFadeState(this, target);
        }

        public override MTFiniteTimeAction Reverse()
        {
            return null;
        }
    }

    public class CardFadeState : MTFiniteTimeActionState
    {
        protected Card _card;

        public SpriteRenderer Center;
        public SpriteRenderer Icon;
        public SpriteRenderer Num;
        public SpriteRenderer Back;
        public SpriteRenderer Front;


        Color BackColor;




        public CardFadeState(CardFadeAction action, GameObject target)
            : base(action, target)
        {
            _card =(Card) action._card;
            Center = _card.Center;
            Num = _card.Num;
            Back = _card.Back;
            Front = _card.Front;


            BackColor = Back.color;
            _card.SetCardAlpha(1);
            Back.color = new Color(BackColor.r, BackColor.g, BackColor.b, 0f);


        }




        public override void Update(float time)
        {
            if (Target == null)
                return;

            float r = 1f - time;//Mathf.Lerp(startTime, endTime, currentTime);
            _card.SetCardAlpha(r);

        }
    }


}