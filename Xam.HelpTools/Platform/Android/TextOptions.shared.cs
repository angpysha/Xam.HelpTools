using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xamarin.Forms;

namespace Xam.HelpTools.Platform.Android
{
    public static class TextOptions
    {
        const string EffectName = "Angpysha.AndroidTextOptionsEffect";
        public static BindableProperty UseFontResourcesProperty = BindableProperty.CreateAttached("UseFontResources", typeof(bool),
            typeof(TextOptions),
            false,
            propertyChanged: OnUseFontResourcesChanged);

        public static bool GetUseFontResources(BindableObject bindable)
        {
            return (bool)bindable.GetValue(UseFontResourcesProperty);
        }

        public static void SetUseFontResources(BindableObject bindable, bool value)
        {
            bindable.SetValue(UseFontResourcesProperty, value);
        }

        public static IPlatformElementConfiguration<Xamarin.Forms.PlatformConfiguration.Android, View> SetUseFontResources(this IPlatformElementConfiguration<Xamarin.Forms.PlatformConfiguration.Android, View> platformElement, bool value)
        {
            SetUseFontResources(platformElement, value);
            return platformElement;
        }

        public static bool GetUseFontResources(this IPlatformElementConfiguration<Xamarin.Forms.PlatformConfiguration.Android, View> platformElement)
        {
            return GetUseFontResources(platformElement);
        }

        private static void OnUseFontResourcesChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if ((bool)newValue)
            {
                AttachEffect((View)bindable);
            }
            else
            {
                DetachEffect((View)bindable);
            }
        }

        private static void DetachEffect(View bindable)
        {
            IElementController elementController = bindable as IElementController;


            if (elementController == null || !elementController.EffectIsAttached(EffectName))
                return;

            var toRemove = bindable.Effects.FirstOrDefault(e => e.ResolveId == Effect.Resolve(EffectName).ResolveId);
            if (toRemove != null)
            {
                bindable.Effects.Remove(toRemove);
            }
        }

        private static void AttachEffect(View bindable)
        {
            IElementController elementController = bindable as IElementController;

            if (elementController == null || elementController.EffectIsAttached(EffectName))
                return;

            bindable.Effects.Add(Effect.Resolve(EffectName));
        }
    }
}
