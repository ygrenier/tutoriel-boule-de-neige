using Microsoft.WindowsAzure.MobileServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace BouleDeNeige
{
    /// <summary>
    /// Classe de gestion du service
    /// </summary>
    public class ServiceClient
    {
#if DEBUG
        const String appUrl = "http://localhost:59487/";
#else
        const String appUrl = "https://bouledeneige.azurewebsites.net";
#endif

        private IMobileServiceTable<Joueur> joueurTable;

        /// <summary>
        /// Création d'un nouveau client de service
        /// </summary>
        public ServiceClient()
        {
            MobileService = new MobileServiceClient(appUrl);
            joueurTable = MobileService.GetTable<Joueur>();
        }

        /// <summary>
        /// Recherche un joueur
        /// </summary>
        public Task<Joueur> RechercheJoueur(String id)
        {
            return joueurTable.LookupAsync(id);
        }

        /// <summary>
        /// Recherche un jour par son nom
        /// </summary>
        public async Task<Joueur> RechercheJoueurParNom(String name)
        {
            return (await joueurTable.CreateQuery().Where(j => j.Nom == name).ToListAsync()).FirstOrDefault();
        }

        /// <summary>
        /// Initialisation du client
        /// </summary>
        public async Task<Joueur> Initialize()
        {
            // Reset le système
            JoueurEnCours = null;

            // Recherche l'ID du joueur enregistré de cette application dans les settings
            var settings = Windows.Storage.ApplicationData.Current.RoamingSettings;
            var jId = (String)settings.Values["joueur"];

            // Si l'id est null ou vide on n'a jamais enregistré de joueur
            if (String.IsNullOrWhiteSpace(jId)) return null;

            // On recherche l'état du joueur
            JoueurEnCours = await RechercheJoueur(jId);
            if (JoueurEnCours == null)
            {
                // Supprime l'ID du joueur qui n'existe plus apparemment
                settings.Values.Remove("joueur");
                settings.Values.Remove("dernier-historique");
                return null;
            }

            // Retourne le joueur
            return JoueurEnCours;
        }

        /// <summary>
        /// Définition du joueur en cours
        /// </summary>
        public void DefinirJoueurEnCours(Joueur joueur)
        {
            this.JoueurEnCours = joueur;

            // Enregistrement de l'id
            var settings = Windows.Storage.ApplicationData.Current.RoamingSettings;
            if (this.JoueurEnCours== null)
            {
                settings.Values.Remove("joueur");
                settings.Values.Remove("dernier-historique");
            }
            else
            {
                settings.Values["joueur"] = this.JoueurEnCours.Id;
            }
        }

        /// <summary>
        /// Création d'un nouveau joueur
        /// </summary>
        public async Task<Joueur> CreerJoueur(string nom)
        {
            var result = new Joueur
            {
                Nom = nom
            };
            await joueurTable.InsertAsync(result);
            // On défini le joueur en cours
            DefinirJoueurEnCours(result);
            // Retourne le résultat
            return result;
        }

        /// <summary>
        /// Retourne la liste des autres joueurs
        /// </summary>
        public async Task<IEnumerable<Joueur>> ListeAutreJoueurs()
        {
            if (JoueurEnCours == null) return null;
            return await joueurTable.CreateQuery().Where(j => j.Id != JoueurEnCours.Id).ToListAsync();
        }

        /// <summary>
        /// Lancer d'une boule à une cible
        /// </summary>
        public async Task<Lancer> LancerUneBoule(Joueur cible)
        {
            if (JoueurEnCours == null)
                throw new InvalidOperationException("Aucun joueur n'est enregistré");
            if (cible == null) throw new ArgumentNullException("cible");
            if (cible.Id == JoueurEnCours.Id)
                throw new ArgumentException("Impossible de se lancer une boule à soi-même");
            if (JoueurEnCours.BoulesRestantes <= 0)
                throw new InvalidOperationException("Vous n'avez plus de boules à lancer");
            try
            {
                // Provoque le lancer
                Dictionary<string, string> args = new Dictionary<string, string>();
                args["lanceur"] = JoueurEnCours.Id;
                args["cible"] = cible.Id;
                var result = await MobileService.InvokeApiAsync<Lancer>("lancer/LancerUneBoule", HttpMethod.Get, args);
                // Actualise le joueur
                DefinirJoueurEnCours(await RechercheJoueur(JoueurEnCours.Id));

                return result;
            }
            catch (MobileServiceInvalidOperationException mex)
            {
                var jValue = Newtonsoft.Json.Linq.JToken.Parse(await mex.Response.Content.ReadAsStringAsync());
                var s = (String)jValue["message"];
                if (!String.IsNullOrWhiteSpace(s))
                    throw new Exception(s);
                throw;
            }
        }

        /// <summary>
        /// Retourne l'historique
        /// </summary>
        public async Task<LancerHistorique[]> Historique()
        {
            if (JoueurEnCours == null) return null;
            try
            {
                // Récupération de la date du dernier historique
                var settings = Windows.Storage.ApplicationData.Current.RoamingSettings;
                DateTimeOffset? dernierHistorique = (DateTimeOffset?)settings.Values["dernier-historique"];

                // Récupération de l"historique
                var result = await MobileService.InvokeApiAsync<LancerHistorique[]>("lancer/LancerHistorique", HttpMethod.Get, null);

                // Mise à jour du résultat
                foreach (var lancer in result)
                {
                    // On averti sur ce lancer si on a une date d'historique, et que le joueur est la cible du lancer
                    lancer.Avertir = dernierHistorique.HasValue && lancer.CibleId == JoueurEnCours.Id && dernierHistorique.Value <= lancer.Date;
                }

                // Enregistre la date du dernier historique
                settings.Values["dernier-historique"] = (DateTimeOffset?)DateTimeOffset.Now;

                // Retourne le résultat
                return result;
            }
            catch (MobileServiceInvalidOperationException mex)
            {
                var jValue = Newtonsoft.Json.Linq.JToken.Parse(await mex.Response.Content.ReadAsStringAsync());
                var s = (String)jValue["message"];
                if (!String.IsNullOrWhiteSpace(s))
                    throw new Exception(s);
                throw;
            }
        }

        /// <summary>
        /// Service mobile
        /// </summary>
        public MobileServiceClient MobileService { get; private set; }

        /// <summary>
        /// Joueur en cours
        /// </summary>
        public Joueur JoueurEnCours { get; private set; }

    }
}
