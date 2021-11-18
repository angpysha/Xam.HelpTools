using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xamarin.Forms;

namespace Xam.HelpTools.Effects.BrushedtextColor
{
    public static class BrushedTextColor
    {
        public static BindableProperty TextColorBrushProperty = BindableProperty.CreateAttached("TextColorBrush",
           typeof(Brush),
           typeof(BrushedTextColor),
           default,
           propertyChanged: OnTextColorBrushChanged);

        public static Brush GetTextColorBrush(BindableObject bindable)
        {
            return (Brush)bindable.GetValue(TextColorBrushProperty);
        }

        public static void SetTextColorBrush(BindableObject bindable, Brush brush)
        {
            bindable.SetValue(TextColorBrushProperty, brush);
        }

        private static void OnTextColorBrushChanged(BindableObject bindableObject, object oldValue, object newValue)
        {
            if (bindableObject is Label || bindableObject is Entry || bindableObject is Editor)
            {
                var view = (View)bindableObject;

                if (view.Effects.Any(x => x is BrushedTextColorRoutingEffect gradientTextColorRoutingEffect))
                {

                }
                else
                {
                    var effect = new BrushedTextColorRoutingEffect();
                    view.Effects.Add(effect);
                }
            }
        }
    }

    public class BrushedTextColorRoutingEffect : RoutingEffect
    {
        public const string EffectGroupName = "Angpysha";
        public const string EffectName = "BrushedText";

        public BrushedTextColorRoutingEffect() : base($"{EffectGroupName}.{EffectName}")
        {

        }
    }

    public partial class BrushedTextColorPlatformEffect
    {

    }
}
