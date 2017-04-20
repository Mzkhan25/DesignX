#region
using Xamarin.Forms;
#endregion

namespace DesignX.Classes
{
    public class CardView : ContentView
    {
        public CardView()
        {
            var view = new RelativeLayout();

            // box view as the background
            var boxView1 = new BoxView
            {
                Color = Color.Black,
                InputTransparent = true
            };
            view.Children.Add(boxView1,
                Constraint.Constant(0), Constraint.Constant(0),
                Constraint.RelativeToParent(parent => parent.Width),
                Constraint.RelativeToParent(parent => parent.Height)
            );
            Photo = new Image
            {
                InputTransparent = true,
                Aspect = Aspect.Fill
            };
            view.Children.Add(Photo,
                Constraint.Constant(0),
                Constraint.RelativeToParent(parent => 0),
                Constraint.RelativeToParent(parent => parent.Width),
                Constraint.RelativeToParent(parent => parent.Height * 1)
            );
            Name = new Label
            {
                TextColor = Color.White,
                FontSize = 22,
                InputTransparent = true
            };
            Content = view;
        }
        public Label Name { get; set; }
        public Image Photo { get; set; }
    }
}