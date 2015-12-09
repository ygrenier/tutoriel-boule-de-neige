using Microsoft.WindowsAzure.MobileServices;
using System;
using System.Collections.Generic;
using System.Text;

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

        /// <summary>
        /// Création d'un nouveau client de service
        /// </summary>
        public ServiceClient()
        {
            MobileService = new MobileServiceClient(appUrl);
        }

        /// <summary>
        /// Service mobile
        /// </summary>
        public MobileServiceClient MobileService { get; private set; }
    }
}
