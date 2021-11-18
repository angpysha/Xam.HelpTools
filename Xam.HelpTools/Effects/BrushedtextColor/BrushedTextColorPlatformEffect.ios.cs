using Foundation;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoreAnimation;
using CoreGraphics;
using UIKit;
using Xam.HelpTools;
using Xam.HelpTools.Effects;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using Xam.HelpTools.Effects.BrushedtextColor;
using Xam.HelpTools.Platform.Android.Helpers;

[assembly: ResolutionGroupName("Angpysha")]
[assembly: ExportEffect(typeof(BrushedTextColorPlatformEffect), "BrushedText")]

namespace Xam.HelpTools.Effects.BrushedtextColor
{
    public partial class BrushedTextColorPlatformEffect : PlatformEffect
    {
        const string BackgroundLayer = "BackgroundLayer";
        public UIView View => Control ?? Container;

        protected override async void OnAttached()
        {
            
            if (View is UILabel textView)
            {
                await Task.Delay(TimeSpan.FromSeconds(0.3));
                UpdateText();

            }
        }

        protected override void OnDetached()
        {
            
        }

        internal async void UpdateText()
        {
            if (Control is UILabel label)
            {
                var brush = BrushedTextColor.GetTextColorBrush(Element);
                //  var image = await GetGradientImage();

                var str = label.AttributedText;
                if (str != null)
                {
                    var frame = Control.Frame;
                    var size = new CGSize(frame.Width, frame.Height);
                    var layer = await GetBackgroundLayer(Control, brush, size);
                    var color = GetColorFromLayer(layer);
                    label.TextColor = color;
                }
            }
        }

        private UIColor GetColorFromLayer(CALayer layer)
        {
            UIGraphics.BeginImageContext(layer.Bounds.Size);
            layer.RenderInContext(UIGraphics.GetCurrentContext());
            var image = UIGraphics.GetImageFromCurrentImageContext();
            UIGraphics.EndImageContext();
            return UIColor.FromPatternImage(image);
        }

        public async Task<CALayer> GetBackgroundLayer(UIView control, Brush brush, CGSize size)
        {
            if (control == null)
                return null;

            var bounds = new CGRect(0, 0, size.Width, size.Height);
            if (brush is SolidColorBrush solidColorBrush)
            {
                var linearGradientLayer = new CALayer
                {
                    Name = BackgroundLayer,
                    ContentsGravity = CALayer.GravityResizeAspectFill,
                    Frame = bounds,
                    BackgroundColor = solidColorBrush.Color.ToCGColor()
                };

                return linearGradientLayer;
            }

            if (brush is LinearGradientBrush linearGradientBrush)
            {
                var p1 = linearGradientBrush.StartPoint;
                var p2 = linearGradientBrush.EndPoint;

                var linearGradientLayer = new CAGradientLayer
                {
                    Name = BackgroundLayer,
                    ContentsGravity = CALayer.GravityResizeAspectFill,
                    Frame = bounds,
                    LayerType = CAGradientLayerType.Axial,
                    StartPoint = new CGPoint(p1.X, p1.Y),
                    EndPoint = new CGPoint(p2.X, p2.Y)
                };

                if (linearGradientBrush.GradientStops != null && linearGradientBrush.GradientStops.Count > 0)
                {
                    var orderedStops = linearGradientBrush.GradientStops.OrderBy(x => x.Offset).ToList();
                    linearGradientLayer.Colors = orderedStops.Select(x => x.Color.ToCGColor()).ToArray();
                    var loc = GetCAGradientLayerLocations(orderedStops);
                    var locations = orderedStops.Select(x => new NSNumber(x.Offset)).ToArray();
                    linearGradientLayer.Locations = locations;

                }

                return linearGradientLayer;
            }

            if (brush is RadialGradientBrush radialGradientBrush)
            {
                var center = radialGradientBrush.Center;
                var radius = radialGradientBrush.Radius;

                var radialGradientLayer = new CAGradientLayer
                {
                    Name = BackgroundLayer,
                    Frame = bounds,
                    LayerType = CAGradientLayerType.Radial,
                    StartPoint = new CGPoint(center.X, center.Y),
                    EndPoint = GetRadialGradientBrushEndPoint(center, radius),
                    // CornerRadius = (float)radius
                };

                if (radialGradientBrush.GradientStops != null && radialGradientBrush.GradientStops.Count > 0)
                {
                    var orderedStops = radialGradientBrush.GradientStops.OrderBy(x => x.Offset).ToList();
                    radialGradientLayer.Colors = orderedStops.Select(x => x.Color.ToCGColor()).ToArray();
                    var locations = orderedStops.Select(x => new NSNumber(x.Offset)).ToArray();
                    radialGradientLayer.Locations = locations;
                }

                return radialGradientLayer;
            }

            if (brush is ImageBrush imageBrush)
            {
                var image = await imageBrush.Source.ToUIImage();
                var layer = new CALayer()
                {
                    Name = BackgroundLayer,
                    Frame = bounds
                };
                if (image != null)
                {
                    //    var newImage = ResizeImage(image, size);
                    layer.Contents = image.CGImage;
                }

                return layer;
            }

            return null;
        }

        private UIImage ResizeImage(UIImage image, CGSize size)
        {
            // + (UIImage *)imageWithImage:(UIImage *)image scaledToSize:(CGSize)newSize {
            //     //UIGraphicsBeginImageContext(newSize);
            //     // In next line, pass 0.0 to use the current device's pixel scaling factor (and thus account for Retina resolution).
            //     // Pass 1.0 to force exact pixel size.
            //     UIGraphicsBeginImageContextWithOptions(newSize, NO, 0.0);
            //     [image drawInRect:CGRectMake(0, 0, newSize.width, newSize.height)];
            //     UIImage *newImage = UIGraphicsGetImageFromCurrentImageContext();    
            //     UIGraphicsEndImageContext();
            //     return newImage;
            // }
            //UIGraphics.BeginImageContext(size);
            UIGraphics.BeginImageContextWithOptions(size, false, 0);
            image.Draw(new CGRect(0, 0, size.Width, size.Height));
            var newImage = UIGraphics.GetImageFromCurrentImageContext();
            UIGraphics.EndImageContext();
            return newImage;
        }


        static CGPoint GetRadialGradientBrushEndPoint(Point startPoint, double radius)
        {
            double x = startPoint.X == 1 ? (startPoint.X - radius) : (startPoint.X + radius);

            if (x < 0)
                x = 0;

            if (x > 1)
                x = 1;

            double y = startPoint.Y == 1 ? (startPoint.Y - radius) : (startPoint.Y + radius);

            if (y < 0)
                y = 0;

            if (y > 1)
                y = 1;

            return new CGPoint(x, y);
        }

        static NSNumber[] GetCAGradientLayerLocations(List<GradientStop> gradientStops)
        {
            if (gradientStops == null || gradientStops.Count == 0)
                return new NSNumber[0];

            if (gradientStops.Count > 1 && gradientStops.Any(gt => gt.Offset != 0))
                return gradientStops.Select(x => new NSNumber(x.Offset)).ToArray();
            else
            {
                int itemCount = gradientStops.Count;
                int index = 0;
                float step = 1.0f / itemCount;

                NSNumber[] locations = new NSNumber[itemCount];

                foreach (var gradientStop in gradientStops)
                {
                    float location = step * index;
                    bool setLocation = !gradientStops.Any(gt => gt.Offset > location);

                    if (gradientStop.Offset == 0 && setLocation)
                        locations[index] = new NSNumber(location);
                    else
                        locations[index] = new NSNumber(gradientStop.Offset);

                    index++;
                }

                return locations;
            }
        }
    }
}
