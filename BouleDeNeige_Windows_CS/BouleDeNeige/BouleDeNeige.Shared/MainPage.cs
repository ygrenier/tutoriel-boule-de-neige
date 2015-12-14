using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.MobileServices;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using System.Linq;

// To add offline sync support, add the NuGet package Microsoft.WindowsAzure.MobileServices.SQLiteStore
// to your project. Then, uncomment the lines marked // offline sync
// For more information, see: http://aka.ms/addofflinesync
//using Microsoft.WindowsAzure.MobileServices.SQLiteStore;  // offline sync
//using Microsoft.WindowsAzure.MobileServices.Sync;         // offline sync

namespace BouleDeNeige
{
    sealed partial class MainPage: Page
    {
        DispatcherTimer timer;

        public MainPage()
        {
            this.InitializeComponent();
            this.tbAutreJoueurs.SelectionChanged += tbAutreJoueurs_SelectionChanged;
            UpdateJoueurInfos(null);
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(5);
            timer.Tick += async (s, e) =>
            {
                try {
                    if (App.ServiceClient.JoueurEnCours != null)
                    {
                        App.ServiceClient.DefinirJoueurEnCours(await App.ServiceClient.RechercheJoueur(App.ServiceClient.JoueurEnCours.Id));
                        await UpdateHistorique();
                    }
                }catch(Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex.GetBaseException().Message);
                }
            };
        }

        void UpdateJoueurInfos(Joueur joueur)
        {
            if (joueur == null)
            {
                tbNomJoueur.IsEnabled = true;
                tbNomJoueur.IsReadOnly = false;
                btnCreerJoueur.Visibility = Visibility.Visible;
                grdJoueurInfos.Visibility = Visibility.Collapsed;
                grdAutresJoueurs.Visibility = Visibility.Collapsed;
                grdHistorique.Visibility = Visibility.Collapsed;
            }
            else
            {
                tbNomJoueur.Text = joueur.Nom;
                tbNomJoueur.IsEnabled = false;
                tbNomJoueur.IsReadOnly = true;
                btnCreerJoueur.Visibility = Visibility.Collapsed;
                grdJoueurInfos.Visibility = Visibility.Visible;
                tbPoints.Text = joueur.Points.ToString();
                tbRestantes.Text = joueur.BoulesRestantes.ToString();
                tbRecues.Text = joueur.BoulesRecues.ToString();
                tbLancees.Text = joueur.BoulesLancees.ToString();
                grdAutresJoueurs.Visibility = Visibility.Visible;
                grdHistorique.Visibility = Visibility.Visible;
            }
        }

        async Task UpdateAutresJoueurs()
        {
            // On actualise les joueurs ?
            if (App.ServiceClient.JoueurEnCours != null)
            {
                tbAutreJoueurs.ItemsSource = (await App.ServiceClient.ListeAutreJoueurs())
                    .OrderByDescending(jo => jo.Points)
                    .ThenByDescending(jo => jo.BoulesRestantes)
                    .ToList()
                    ;
            }
        }

        async Task UpdateHistorique()
        {
            // Actualisation de l'historique
            if (App.ServiceClient.JoueurEnCours != null)
            {
                var historique = (await App.ServiceClient.Historique());
                tbHistorique.ItemsSource = historique;
                foreach (var lancer in historique.Where(l => l.Avertir))
                {
                    String msg = lancer.Success 
                        ? String.Format("{0} vous a touché le {1}", lancer.LanceurNom, lancer.Date)
                        : String.Format("{0} vous a manqué le {1}", lancer.LanceurNom, lancer.Date);
                    var dlg = new MessageDialog(msg);
                    dlg.Commands.Add(new UICommand("OK"));
                    await dlg.ShowAsync();
                }
            }
        }

        async Task<Lancer> LancerUneBoule(Joueur cible)
        {
            try
            {
                timer.Stop();
                // On demande confirmation
                var dlg = new MessageDialog(String.Format("Voulez-vous lancer une boule de neige à {0} ?", cible.Nom), "Confirmation");
                dlg.Commands.Add(new UICommand("Oui"));
                dlg.Commands.Add(new UICommand("Non"));
                dlg.CancelCommandIndex = 1;
                dlg.DefaultCommandIndex = 0;
                var cmd = await dlg.ShowAsync();
                if (cmd.Label == "Non") return null;

                // Provoque le lancer
                var result = await App.ServiceClient.LancerUneBoule(cible);
                // On actualise les informations
                UpdateJoueurInfos(App.ServiceClient.JoueurEnCours);
                await UpdateAutresJoueurs();
                await UpdateHistorique();

                // Affiche le résultat
                if (result.Succes)
                {
                    dlg = new MessageDialog("Vous avez touché votre cible !");
                    dlg.Commands.Add(new UICommand("OK"));
                    await dlg.ShowAsync();
                }
                else
                {
                    dlg = new MessageDialog("Vous avez manqué votre cible !");
                    dlg.Commands.Add(new UICommand("OK"));
                    await dlg.ShowAsync();
                }

                return result;
            }
            catch (Exception ex)
            {
                var dlg = new MessageDialog(ex.GetBaseException().Message);
                dlg.Commands.Add(new UICommand("OK"));
                await dlg.ShowAsync();
            }
            finally
            {
                // On annule la sélection
                tbAutreJoueurs.SelectedIndex = -1;
                timer.Start();
            }
            return null;
        }

        private async void tbAutreJoueurs_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Recherche le joueur sélectionné
            var joueur = tbAutreJoueurs.SelectedItem as Joueur;
            if (joueur == null) return;

            await LancerUneBoule(joueur);
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            timer.Start();
            await App.ServiceClient.Initialize();
            UpdateJoueurInfos(App.ServiceClient.JoueurEnCours);
            await UpdateAutresJoueurs();
            await UpdateHistorique();
        }

        private async void btnCreerJoueur_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            try
            {
                timer.Stop();
                btnCreerJoueur.IsEnabled = false;
                tbNomJoueur.IsEnabled = false;
                // On vérifie qu'un nom est saisi
                if (String.IsNullOrWhiteSpace(tbNomJoueur.Text))
                {
                    var md = new MessageDialog("Veuillez saisir un nom de joueur.");
                    md.Commands.Add(new UICommand("OK"));
                    await md.ShowAsync();
                    return;
                }

                // On vérifie si le nom existe 
                var j = await App.ServiceClient.RechercheJoueurParNom(tbNomJoueur.Text);
                if (j != null)
                {
                    var md = new MessageDialog("Ce joueur existe déjà, voulez-vous l'utiliser ?");
                    md.Commands.Add(new UICommand("Oui"));
                    md.Commands.Add(new UICommand("Non"));
                    md.CancelCommandIndex = 1;
                    md.DefaultCommandIndex = 0;
                    var cmd = await md.ShowAsync();
                    if (cmd.Label == "Non")
                        return;
                    // Définition du joueur en cours avec le joueur
                    App.ServiceClient.DefinirJoueurEnCours(j);
                }
                else
                {
                    // Création d'un nouveau joueur
                    j = await App.ServiceClient.CreerJoueur(tbNomJoueur.Text);
                }
                // Mise à jour de l'interface
                UpdateJoueurInfos(App.ServiceClient.JoueurEnCours);
                await UpdateAutresJoueurs();
                await UpdateHistorique();
            }
            catch (Exception ex)
            {
                var md = new MessageDialog(ex.GetBaseException().Message, "Erreur");
                md.Commands.Add(new UICommand("OK"));
                await md.ShowAsync();
            }
            finally
            {
                btnCreerJoueur.IsEnabled = true;
                tbNomJoueur.IsEnabled = true;
                timer.Start();
            }
        }

        /*
        private MobileServiceCollection<TodoItem, TodoItem> items;
        private IMobileServiceTable<TodoItem> todoTable = App.MobileService.GetTable<TodoItem>();
        //private IMobileServiceSyncTable<TodoItem> todoTable = App.MobileService.GetSyncTable<TodoItem>(); // offline sync

        private async Task InsertTodoItem(TodoItem todoItem)
        {
            // This code inserts a new TodoItem into the database. When the operation completes
            // and Mobile App backend has assigned an Id, the item is added to the CollectionView.
            await todoTable.InsertAsync(todoItem);
            items.Add(todoItem);

            //await SyncAsync(); // offline sync
        }

        private async Task RefreshTodoItems()
        {
            MobileServiceInvalidOperationException exception = null;
            try
            {
                // This code refreshes the entries in the list view by querying the TodoItems table.
                // The query excludes completed TodoItems.
                items = await todoTable
                    .Where(todoItem => todoItem.Complete == false)
                    .ToCollectionAsync();
            }
            catch (MobileServiceInvalidOperationException e)
            {
                exception = e;
            }

            if (exception != null)
            {
                await new MessageDialog(exception.Message, "Error loading items").ShowAsync();
            }
            else
            {
                ListItems.ItemsSource = items;
                this.ButtonSave.IsEnabled = true;
            }
        }

        private async Task UpdateCheckedTodoItem(TodoItem item)
        {
            // This code takes a freshly completed TodoItem and updates the database. When the service 
            // responds, the item is removed from the list.
            await todoTable.UpdateAsync(item);
            items.Remove(item);
            ListItems.Focus(Windows.UI.Xaml.FocusState.Unfocused);

            //await SyncAsync(); // offline sync
        }

        private async void ButtonRefresh_Click(object sender, RoutedEventArgs e)
        {
            ButtonRefresh.IsEnabled = false;

            //await SyncAsync(); // offline sync
            await RefreshTodoItems();

            ButtonRefresh.IsEnabled = true;
        }

        private async void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            var todoItem = new TodoItem { Text = TextInput.Text };
            await InsertTodoItem(todoItem);
        }

        private async void CheckBoxComplete_Checked(object sender, RoutedEventArgs e)
        {
            CheckBox cb = (CheckBox)sender;
            TodoItem item = cb.DataContext as TodoItem;
            await UpdateCheckedTodoItem(item);
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            //await InitLocalStoreAsync(); // offline sync
            await RefreshTodoItems();
        }

        #region Offline sync

        //private async Task InitLocalStoreAsync()
        //{
        //    if (!App.MobileService.SyncContext.IsInitialized)
        //    {
        //        var store = new MobileServiceSQLiteStore("localstore.db");
        //        store.DefineTable<TodoItem>();
        //        await App.MobileService.SyncContext.InitializeAsync(store);
        //    }
        //
        //    await SyncAsync();
        //}

        //private async Task SyncAsync()
        //{
        //    await App.MobileService.SyncContext.PushAsync();
        //    await todoTable.PullAsync("todoItems", todoTable.CreateQuery());
        //}

        #endregion 
        */
    }
}
