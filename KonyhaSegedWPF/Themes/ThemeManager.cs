using System;
using System.Linq;
using System.Windows;

namespace KonyhaSegedWPF.Themes
{
    public enum Theme { Dark, Light }

    public static class ThemeManager
    {
        private static readonly Uri DarkUri = new Uri("Themes/Dark.xaml", UriKind.Relative);
        private static readonly Uri LightUri = new Uri("Themes/Light.xaml", UriKind.Relative);

        public static Theme Current { get; private set; } = Theme.Dark;

        /// <summary>
        /// Kicseréli az aktuális téma szótárat a megadottra.
        /// </summary>
        public static void Apply(Theme theme)
        {
            var dicts = Application.Current.Resources.MergedDictionaries;

            // A korábbi téma szótár eltávolítása (ha van)
            var existing = dicts.FirstOrDefault(d =>
                d.Source != null &&
                (d.Source.OriginalString.EndsWith("Dark.xaml") ||
                 d.Source.OriginalString.EndsWith("Light.xaml")));

            if (existing != null)
                dicts.Remove(existing);

            dicts.Add(new ResourceDictionary
            {
                Source = theme == Theme.Dark ? DarkUri : LightUri
            });

            Current = theme;
        }
    }
}