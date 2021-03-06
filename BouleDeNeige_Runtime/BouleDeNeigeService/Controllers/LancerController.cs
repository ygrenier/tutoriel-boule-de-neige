﻿using BouleDeNeigeService.DataObjects;
using Microsoft.Azure.Mobile.Server.Config;
using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;

namespace BouleDeNeigeService.Controllers
{
    [MobileAppController]
    public class LancerController : ApiController
    {
        public class LancerResult
        {
            public DateTimeOffset Date { get; set; }
            public String Cible { get; set; }
            public String Lanceur { get; set; }
            public bool Succes { get; set; }
        }

        public class LancerHistoriqueResult
        {
            public DateTimeOffset Date { get; set; }
            public String LanceurId { get; set; }
            public String LanceurNom { get; set; }
            public String CibleId { get; set; }
            public String CibleNom { get; set; }
            public bool Success { get; set; }
        }

        // GET api/Lancer
        //public string Get()
        //{
        //    return "Hello from custom controller!";
        //}

        // Get api/Lancer/LancerUneBoule
        [HttpGet]
        public async Task<IHttpActionResult> LancerUneBoule(string lanceur, string cible)
        {
            try
            {
                using (Models.BouleDeNeigeContext context = new Models.BouleDeNeigeContext())
                {
                    // Recherche le joueur lanceur
                    var jLanceur = await context.Joueurs.FirstOrDefaultAsync(j => j.Id == lanceur);
                    if (jLanceur == null)
                        throw new System.InvalidOperationException("Lanceur inconnu");
                    // Recherche le joueur cible
                    var jCible = await context.Joueurs.FirstOrDefaultAsync(j => j.Id == cible);
                    if (jCible == null)
                        throw new System.ArgumentException("Cible inconnue");
                    // Vérifie que le lanceur à encore des boules
                    if (jLanceur.BoulesRestantes <= 0)
                        throw new System.InvalidOperationException("Vous n'avez plus de boules à lancer.");
                    // Test la réussite du lancer (une chance sur deux)
                    Random rnd = new Random();
                    bool succes = (rnd.Next(1000000) % 2) == 0;
                    // Création du lancer
                    var lancer = new Lancer
                    {
                        Cible = jCible,
                        Lanceur = jLanceur,
                        Date = DateTimeOffset.Now,
                        Success = succes
                    };
                    context.Lancers.Add(lancer);
                    // Recalcul le nombre boules du lanceur
                    jLanceur.BoulesRestantes--;
                    jLanceur.BoulesLancees++;
                    // Recalcul le nombre boules de la cible
                    if (succes)
                    {
                        // La cible à reçue une boule et elle perd un point
                        jCible.BoulesRecues++;
                        jCible.Points--;
                        // Le lanceur gagne un point
                        jLanceur.Points++;
                    }
                    else
                    {
                        // La cible n'est pas touchée, elle gagne une boule
                        jCible.BoulesRestantes++;
                    }
                    // Valide toutes les modifications
                    await context.SaveChangesAsync();
                    // Retourne le résultat
                    return Ok(new LancerResult
                    {
                        Date = lancer.Date,
                        Lanceur = lancer.LanceurId,
                        Cible = lancer.CibleId,
                        Succes = lancer.Success
                    });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.GetBaseException().Message);
            }
        }

        [HttpGet]
        public async Task<IHttpActionResult> LancerHistorique()
        {
            try
            {
                using (Models.BouleDeNeigeContext context = new Models.BouleDeNeigeContext())
                {
                    // Extraction des données
                    return Ok<LancerHistoriqueResult[]>(await context.Lancers
                        .Include(l => l.Cible)
                        .Include(l => l.Lanceur)
                        .OrderByDescending(l => l.Date)
                        .Select(l => new LancerHistoriqueResult
                        {
                            Date = l.Date,
                            CibleId = l.CibleId,
                            CibleNom = l.Cible.Nom,
                            LanceurId = l.LanceurId,
                            LanceurNom = l.Lanceur.Nom,
                            Success = l.Success
                        })
                        .ToArrayAsync()
                    );
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.GetBaseException().Message);
            }
        }

    }
}
