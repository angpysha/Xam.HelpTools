using Xamarin.Forms;

namespace Xam.HelpTools.Effects.BrushedtextColor
{
    public class ImageBrush : Brush
    {
        public override bool IsEmpty { get; }

        public static readonly BindableProperty SourceProperty = BindableProperty.Create(nameof(Source),
            typeof(ImageSource),
            typeof(ImageBrush),
            default);

        public ImageSource Source
        {
            get => GetValue(SourceProperty) as ImageSource;
            set => SetValue(SourceProperty, value);
        }
    }
}
