using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.OData;
using Microsoft.Azure.Mobile.Server;
using BouleDeNeigeService.DataObjects;
using BouleDeNeigeService.Models;

namespace BouleDeNeigeService.Controllers
{
    public class JoueurController : TableController<Joueur>
    {
        protected override void Initialize(HttpControllerContext controllerContext)
        {
            base.Initialize(controllerContext);
            BouleDeNeigeContext context = new BouleDeNeigeContext();
            DomainManager = new EntityDomainManager<Joueur>(context, Request);
        }

        // GET tables/Joueur
        public IQueryable<Joueur> GetAllJoueur()
        {
            return Query(); 
        }

        // GET tables/Joueur/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public SingleResult<Joueur> GetJoueur(string id)
        {
            return Lookup(id);
        }

        // PATCH tables/Joueur/48D68C86-6EA6-4C25-AA33-223FC9A27959
        //public Task<Joueur> PatchJoueur(string id, Delta<Joueur> patch)
        //{
        //     return UpdateAsync(id, patch);
        //}

        // POST tables/Joueur
        public async Task<IHttpActionResult> PostJoueur(Joueur item)
        {
            // Force les valeurs du nouveau joueur
            item.BoulesLancees = 0;
            item.BoulesRecues = 0;
            item.BoulesRestantes = 5;

            Joueur current = await InsertAsync(item);
            return CreatedAtRoute("Tables", new { id = current.Id }, current);
        }

        // DELETE tables/Joueur/48D68C86-6EA6-4C25-AA33-223FC9A27959
        //public Task DeleteJoueur(string id)
        //{
        //     return DeleteAsync(id);
        //}
    }
}
