using Microsoft.WindowsAzure.MobileServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using Windows.UI.Xaml.Controls;

namespace BouleDeNeige
{
    public sealed partial class MainPage : Page
    {
        /*
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

   try
   {
       Dictionary<string, string> args = new Dictionary<string, string>();
       args["lanceur"] = j1.Id;
       args["cible"] = j2.Id;
       var lancer = await App.MobileService.InvokeApiAsync<Lancer>("lancer/LancerUneBoule", HttpMethod.Get, args);
   }
   catch(MobileServiceInvalidOperationException mex)
   {
       var jValue = Newtonsoft.Json.Linq.JToken.Parse(await mex.Response.Content.ReadAsStringAsync());
       var s = (String)jValue["message"];
       System.Diagnostics.Debug.WriteLine(s ?? mex.GetBaseException().Message);
   }
   catch (Exception ex)
   {
       System.Diagnostics.Debug.WriteLine(ex.GetBaseException().Message);
   }
}
*/
    }
}
