using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.MobileServices;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// To add offline sync support, add the NuGet package Microsoft.WindowsAzure.MobileServices.SQLiteStore
// to your project. Then, uncomment the lines marked // offline sync
// For more information, see: http://aka.ms/addofflinesync
//using Microsoft.WindowsAzure.MobileServices.SQLiteStore;  // offline sync
//using Microsoft.WindowsAzure.MobileServices.Sync;         // offline sync

namespace BouleDeNeige
{
    sealed partial class MainPage: Page
    {
        public MainPage()
        {
            this.InitializeComponent();
            UpdateJoueurInfos(null);
        }

        void UpdateJoueurInfos(Joueur joueur)
        {
            if (joueur == null)
            {
                tbNomJoueur.IsEnabled = true;
                tbNomJoueur.IsReadOnly = false;
                btnCreerJoueur.Visibility = Visibility.Visible;
                grdJoueurInfos.Visibility = Visibility.Collapsed;
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
            }
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            await App.ServiceClient.Initialize();
            UpdateJoueurInfos(App.ServiceClient.JoueurEnCours);
        }

        private async void btnCreerJoueur_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            try
            {
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
