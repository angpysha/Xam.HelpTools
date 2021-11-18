using Android.Graphics;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Threading.Tasks;
using Xam.HelpTools.Effects.BrushedtextColor;
using Xam.HelpTools.Platform.Android.Helpers;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using AView = Android.Views.View;

[assembly: ExportEffect(typeof(BrushedTextColorPlatformEffect), "BrushedText")]

namespace Xam.HelpTools.Effects.BrushedtextColor
{
    public partial class BrushedTextColorPlatformEffect : PlatformEffect
    {
        public AView View => Control ?? Container;
        protected override async void OnAttached()
        {
            // await Task.Delay(TimeSpan.FromSeconds(0.3));
            //UpdateTextColor();
            if (this.View is TextView textView)
            {
                textView.ViewTreeObserver.PreDraw += ViewTreeObserver_PreDraw;
                //  textView.TextChanged += TextChanged;
            }

        }

        private void ViewTreeObserver_PreDraw(object sender, Android.Views.ViewTreeObserver.PreDrawEventArgs e)
        {
            if (this.View is TextView textView)
            {
                textView.ViewTreeObserver.PreDraw -= ViewTreeObserver_PreDraw;
                textView.TextChanged += TextChanged;
                MainThread.BeginInvokeOnMainThread(UpdateTextColor);
            }
        }

        private void TextChanged(object sender, Android.Text.TextChangedEventArgs e)
        {
            MainThread.BeginInvokeOnMainThread(UpdateTextColor);

        }


        protected override void OnDetached()
        {
            if (this.View is TextView textView)
            {
                textView.TextChanged -= TextChanged;
            }
        }

        protected override void OnElementPropertyChanged(PropertyChangedEventArgs args)
        {
            base.OnElementPropertyChanged(args);

            if (args.PropertyName == BrushedTextColor.TextColorBrushProperty.PropertyName)
            {
                UpdateTextColor();
            }
        }

        private async void UpdateTextColor()
        {
            var gradient = BrushedTextColor.GetTextColorBrush(Element);
            var shader = await BrushToShader(gradient);
            if (this.View is TextView textView)
            {
                textView.Paint.SetShader(shader);
            }
        }

        private async Task<Shader> BrushToShader(Brush brush)
        {

            if (this.View is TextView textView)
            {

                if (brush == null || brush.IsEmpty)
                    return null;

                var width = textView.MeasuredWidth;
                var hh = textView.TextSize;
                var height = textView.MeasuredHeight;
                //height = hh;
                if (brush is SolidColorBrush solidColorBrush)
                {
                    var backgroundColor = solidColorBrush.Color;
                    //    paint.Color = backgroundColor.ToAndroid();
                    return null;
                }

                if (brush is LinearGradientBrush linearGradientBrush)
                {
                    var p1 = linearGradientBrush.StartPoint;
                    var x1 = (float)p1.X;
                    var y1 = (float)p1.Y;

                    var p2 = linearGradientBrush.EndPoint;
                    var x2 = (float)p2.X;
                    var y2 = (float)p2.Y;

                    var gradientBrushData = GetGradientBrushData(linearGradientBrush);
                    var colors = gradientBrushData.Item1;
                    var offsets = gradientBrushData.Item2;

                    if (colors.Length < 2)
                        return null;

                    var linearGradientShader = new LinearGradient(
                        width * x1,
                        height * y1,
                        width * x2,
                        height * y2,
                        colors,
                        offsets,
                        Shader.TileMode.Clamp);


                    return linearGradientShader;
                }

                if (brush is RadialGradientBrush radialGradientBrush)
                {
                    var center = radialGradientBrush.Center;
                    float centerX = (float)center.X;
                    float centerY = (float)center.Y;
                    float radius = (float)radialGradientBrush.Radius;

                    var gradientBrushData = GetGradientBrushData(radialGradientBrush);
                    var colors = gradientBrushData.Item1;
                    var offsets = gradientBrushData.Item2;

                    if (colors.Length < 2)
                        return null;

                    var radialGradientShader = new RadialGradient(
                        width * centerX,
                        height * centerY,
                        Math.Max(height, width) * radius,
                        colors,
                        offsets,
                        Shader.TileMode.Clamp);

                    return radialGradientShader;
                }

                if (brush is ImageBrush imageBrush)
                {
                    var bitmap = await imageBrush.Source.ToBitmap(Control.Context);
                    var imageShader = new BitmapShader(bitmap, Shader.TileMode.Clamp, Shader.TileMode.Clamp);
                    return imageShader;
                }
            }

            return null;
        }

        internal static Tuple<int[], float[]> GetGradientBrushData(GradientBrush gradientBrush)
        {
            var orderStops = gradientBrush.GradientStops;

            int[] colors = new int[orderStops.Count];
            float[] offsets = new float[orderStops.Count];

            int count = 0;
            foreach (var orderStop in orderStops)
            {
                colors[count] = orderStop.Color.ToAndroid().ToArgb();
                offsets[count] = orderStop.Offset;
                count++;
            }

            return Tuple.Create(colors, offsets);
        }
    }
}
