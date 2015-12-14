using System;
using System.Collections.Generic;
using System.Text;
using Windows.UI.Xaml.Data;

namespace BouleDeNeige.Converters
{
    public class LancerHistoriqueToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var hist = value as LancerHistorique;
            if (hist == null) return String.Empty;
            if (hist.Success)
                return String.Format("{0} à touché {1}", hist.LanceurNom, hist.CibleNom);
            else
                return String.Format("{0} à manqué {1}", hist.LanceurNom, hist.CibleNom);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
