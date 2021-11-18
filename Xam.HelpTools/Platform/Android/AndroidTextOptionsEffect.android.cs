using System;
using System.Collections.Generic;
using System.Text;
using Xam.HelpTools.Platform.Android;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using Xamarin.Forms.PlatformConfiguration;
using Xamarin.Forms.Internals;
using AndroidX.Core.Content.Resources;
using Android.Widget;
using System.Diagnostics;

[assembly: ResolutionGroupName("Angpysha")]
[assembly: ExportEffect(typeof(AndroidTextOptionsEffect), "AndroidTextOptionsEffect")]
namespace Xam.HelpTools.Platform.Android
{
    public class AndroidTextOptionsEffect : PlatformEffect
    {
        protected override void OnAttached()
        {
            Debugger.Log(0, "Test", "Effect inited");
            UpdateTextFont();
            Element.PropertyChanged += Element_PropertyChanged;
        }

        private void Element_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            // We do not need to update font in every text changes
            if (e.PropertyName != "Text")
            {

                UpdateTextFont();
            }
        }

        private void UpdateTextFont()
        {
            var useFontResource = (bool)TextOptions.GetUseFontResources(Element);
            if (useFontResource)
            {
                if (Element is IFontElement fontElement)
                {
                    var fontFamily = fontElement.FontFamily;
                    var resID = Control.Context.Resources.GetIdentifier(fontFamily, "font", Control.Context.PackageName);
                    if (resID != 0)
                    {
                        var typeFace = ResourcesCompat.GetFont(Control.Context, resID);
                        if (typeFace != null && Control is TextView textView )
                        {
                            textView.Typeface = typeFace;
                        }
                    }
                }

            }
        }

        protected override void OnDetached()
        {
            Element.PropertyChanged -= Element_PropertyChanged;
        }
    }
}
