#region
using System;
using System.Collections;
using System.Collections.Generic;
using Xamarin.Forms;
#endregion

namespace DesignX.Classes
{
    public class CardStackView : ContentView
    {
        private const float BackCardScale = 0.8f;
        // speed of the animations
        private const int AnimLength = 250;
        // 180 / pi
        private const float DegreesToRadians = 57.2957795f;
        // higher the number less the rotation effect
        private const float CardRotationAdjuster = 0.3f;

        // two cards
        private const int NumCards = 2;
        public static readonly BindableProperty ItemsSourceProperty =
            BindableProperty.Create(nameof(ItemsSource), typeof(IList), typeof(CardStackView), null,
                propertyChanged: OnItemsSourcePropertyChanged);
        // distance the card has been moved
        private float _cardDistance;
        private readonly CardView[] _cards = new CardView[NumCards];
        private bool _ignoreTouch;
        // the last items index added to the stack of the cards
        private int _itemIndex;
        // the card at the top of the stack
        private int _topCardIndex;
        public Action<int> SwipedLeft = null;

        // called when a card is swiped left/right with the card index in the ItemSource
        public Action<int> SwipedRight = null;
        public CardStackView()
        {
            var view = new RelativeLayout();

            // create a stack of cards
            for (var i = 0; i < NumCards; i++)
            {
                var card = new CardView();
                _cards[i] = card;
                card.InputTransparent = true;
                card.IsVisible = false;
                view.Children.Add(
                    card,
                    Constraint.Constant(0),
                    Constraint.Constant(0),
                    Constraint.RelativeToParent(parent => { return parent.Width; }),
                    Constraint.RelativeToParent(parent => { return parent.Height; })
                );
            }
            BackgroundColor = Color.Black;
            Content = view;
            var panGesture = new PanGestureRecognizer();
            panGesture.PanUpdated += OnPanUpdated;
            GestureRecognizers.Add(panGesture);
        }
        // distance a card must be moved to consider to be swiped off
        public int CardMoveDistance { get; set; }

        public List<Item> ItemsSource
        {
            get => (List<Item>) GetValue(ItemsSourceProperty);
            set
            {
                SetValue(ItemsSourceProperty, value);
                _itemIndex = 0;
            }
        }

        private static void OnItemsSourcePropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            ((CardStackView) bindable).Setup();
        }
        private void Setup()
        {
            // set the top card
            _topCardIndex = 0;
            // create a stack of cards
            for (var i = 0; i < Math.Min(NumCards, ItemsSource.Count); i++)
            {
                if (_itemIndex >= ItemsSource.Count) break;
                var card = _cards[i];
                card.Name.Text = ItemsSource[_itemIndex].Url;
                card.Photo.Source = ItemsSource[_itemIndex].Url;
                card.IsVisible = true;
                card.Scale = GetScale(i);
                card.RotateTo(0, 0);
                card.TranslateTo(0, -card.Y, 0);
                ((RelativeLayout) Content).LowerChild(card);
                _itemIndex++;
            }
        }
        private void OnPanUpdated(object sender, PanUpdatedEventArgs e)
        {
            switch (e.StatusType)
            {
                case GestureStatus.Started:
                    HandleTouchStart();
                    break;
                case GestureStatus.Running:
                    HandleTouch((float) e.TotalX);
                    break;
                case GestureStatus.Completed:
                    HandleTouchEnd();
                    break;
            }
        }

        // to hande when a touch event begins
        public void HandleTouchStart()
        {
            _cardDistance = 0;
        }

        // to handle te ongoing touch event as the card is moved
        public void HandleTouch(float diffX)
        {
            if (_ignoreTouch)
                return;
            var topCard = _cards[_topCardIndex];
            var backCard = _cards[PrevCardIndex(_topCardIndex)];

            // move the top card
            if (topCard.IsVisible)
            {
                // move the card
                topCard.TranslationX = diffX;

                // calculate a angle for the card
                var rotationAngel = (float) (CardRotationAdjuster * Math.Min(diffX / Width, 1.0f));
                topCard.Rotation = rotationAngel * DegreesToRadians;

                // keep a record of how far its moved
                _cardDistance = diffX;
            }

            // scale the backcard
            if (backCard.IsVisible)
                backCard.Scale =
                    Math.Min(BackCardScale + Math.Abs(_cardDistance / CardMoveDistance * (1.0f - BackCardScale)), 1.0f);
        }

        // to handle the end of the touch event
        public async void HandleTouchEnd()
        {
            _ignoreTouch = true;
            var topCard = _cards[_topCardIndex];

            // if the card was move enough to be considered swiped off
            if (Math.Abs((int) _cardDistance) > CardMoveDistance)
            {
                // move off the screen
                await topCard.TranslateTo(_cardDistance > 0 ? Width : -Width, 0, AnimLength / 2, Easing.SpringOut);
                topCard.IsVisible = false;
                if (SwipedRight != null && _cardDistance > 0)
                    SwipedRight(_itemIndex);
                else if (SwipedLeft != null)
                    SwipedLeft(_itemIndex);

                // show the next card
                ShowNextCard();
            }
            // put the card back in the center
            else
            {
                // move the top card back to the center
                await topCard.TranslateTo(-topCard.X, -topCard.Y, AnimLength, Easing.SpringOut);
                await topCard.RotateTo(0, AnimLength, Easing.SpringOut);

                // scale the back card down
                var prevCard = _cards[PrevCardIndex(_topCardIndex)];
                await prevCard.ScaleTo(BackCardScale, AnimLength, Easing.SpringOut);
            }
            _ignoreTouch = false;
        }

        // show the next card
        private void ShowNextCard()
        {
            if (_cards[0].IsVisible == false && _cards[1].IsVisible == false)
            {
                Setup();
                return;
            }
            var topCard = _cards[_topCardIndex];
            _topCardIndex = NextCardIndex(_topCardIndex);

            // if there are more cards to show, show the next card in to place of 
            // the card that was swipped off the screen
            if (_itemIndex < ItemsSource.Count)
            {
                // push it to the back z order
                ((RelativeLayout) Content).LowerChild(topCard);

                // reset its scale, opacity and rotation
                topCard.Scale = BackCardScale;
                topCard.RotateTo(0, 0);
                topCard.TranslateTo(0, -topCard.Y, 0);

                // set the data
                topCard.Name.Text = ItemsSource[_itemIndex].Name;
                topCard.Photo.Source = ItemsSource[_itemIndex].Url;
                topCard.IsVisible = true;
                _itemIndex++;
            }
        }

        // return the next card index from the top
        private int NextCardIndex(int topIndex)
        {
            return topIndex == 0 ? 1 : 0;
        }

        // return the prev card index from the yop
        private int PrevCardIndex(int topIndex)
        {
            return topIndex == 0 ? 1 : 0;
        }

        // helper to get the scale based on the card index position relative to the top card
        private float GetScale(int index)
        {
            return index == _topCardIndex ? 1.0f : BackCardScale;
        }

        public class Item
        {
            public string Id { get; set; }
            public string Name { get; set; }
            public string Url { get; set; }
            public string Location { get; set; }
            public string Description { get; set; }
        }
    }
}