using Microsoft.WindowsAzure.MobileServices;
using System.Linq;
using Windows.UI.Xaml.Controls;

namespace BouleDeNeige
{
    public sealed partial class MainPage : Page
    {
        private async void btnTest_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            // Création d'une référence sur la table des joueurs
            IMobileServiceTable<Joueur> jTable = App.MobileService.GetTable<Joueur>();
            // Requête pour rechercher les joueurs
            var j1 = (await jTable.CreateQuery().Where(j => j.Nom == "Joueur 1").ToListAsync()).FirstOrDefault();
            var j2 = (await jTable.CreateQuery().Where(j => j.Nom == "Joueur 2").ToListAsync()).FirstOrDefault();
            // Pour chaque joueur manquant on les créés
            if (j1 == null)
            {
                j1 = new Joueur()
                {
                    Nom = "Joueur 1",
                };
                await jTable.InsertAsync(j1);
            }
            if(j2== null)
            {
                j2 = new Joueur()
                {
                    Nom = "Joueur 2"
                };
                await jTable.InsertAsync(j2);
            }
            System.Diagnostics.Debug.WriteLine(j1.Id);
            System.Diagnostics.Debug.WriteLine(j2.Id);
        }
    }
}
