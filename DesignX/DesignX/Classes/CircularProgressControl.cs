#region
using Xamarin.Forms;
#endregion

namespace DesignX.Classes
{
    public class CircularProgressControl : Grid
    {
        public static BindableProperty ProgressProperty =
            BindableProperty.Create("Progress", typeof(double), typeof(CircularProgressControl), 0d,
                propertyChanged: ProgressChanged);
        private readonly View _background1;
        private readonly View _background2;
        private readonly View _progress1;
        private readonly View _progress2;
        public CircularProgressControl()
        {
            _progress1 = CreateImage("progress_done");
            _background1 = CreateImage("progress_pending");
            _background2 = CreateImage("progress_pending");
            _progress2 = CreateImage("progress_done");
            HandleProgressChanged(1, 0);
        }

        public double Progress
        {
            get
			{
				return (double) GetValue(ProgressProperty);
				
			}
            set
			{
				SetValue(ProgressProperty, value);
			}
        }

        private View CreateImage(string v1)
        {
            var img = new Image();
            img.Source = ImageSource.FromFile(v1 + ".png");
            Children.Add(img);
            return img;
        }
        private static void ProgressChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var c = bindable as CircularProgressControl;
            c.HandleProgressChanged(Clamp((double) oldValue, 0, 1), Clamp((double) newValue, 0, 1));
        }
        private static double Clamp(double value, double min, double max)
        {
            if (value <= max && value >= min) return value;
            if (value > max) return max;
            return min;
        }
        private void HandleProgressChanged(double oldValue, double p)
        {
            if (p < .5)
            {
                if (oldValue >= .5)
                {
                    // this code is CPU intensive so only do it if we go from >=50% to <50%
                    _background1.IsVisible = true;
                    _progress2.IsVisible = false;
                    _background2.Rotation = 180;
                    _progress1.Rotation = 0;
                }
                var rotation = 360 * p;
                _background1.Rotation = rotation;
            }
            else
            {
                if (oldValue < .5)
                {
                    // this code is CPU intensive so only do it if we go from <50% to >=50%
                    _background1.IsVisible = false;
                    _progress2.IsVisible = true;
                    _progress1.Rotation = 180;
                }
                var rotation = 360 * p;
                _background2.Rotation = rotation;
            }
        }
    }
}