using projekat.Models;
using projekat.Podaci;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.SessionState;

namespace projekat
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            BazaPodataka.UcitajPodatke(); // Ucitavanje podataka pri prvom pokretanju
            Task.Run(() => ProveraZavrsetkaLetovaPeriodicno()); // Periodicna provera da li je neki aktivan let gotov
        }

        // Periodcna provera da li je aktivan let dosao do destinacije
        async Task ProveraZavrsetkaLetova()
        {
            DateTime sada = DateTime.Now;
            List<Let> zavrseniLetovi = BazaPodataka.Letovi.FindAll(x => x.StatusLeta == StatusLeta.AKTIVAN &&
                                                                       x.DatumVremeDolaska <= sada &&
                                                                       x.StatusLeta != StatusLeta.OTKAZAN);

            zavrseniLetovi.ForEach(let => let.StatusLeta = StatusLeta.ZAVRSEN);
            BazaPodataka.SacuvajPodatke();
            await Task.Delay(15000);
        }

        async Task ProveraZavrsetkaLetovaPeriodicno()
        {
            while (true)
            {
                await ProveraZavrsetkaLetova();
                await Task.Delay(15000);
            }
        }

        // Podrška za sesije u okviru poziva rest servisa
        public override void Init()
        {
            PostAuthenticateRequest += ApiRequestHandler;
            base.Init();
        }

        // Sesije, url zahteva /api/ kao pocetni parametar
        void ApiRequestHandler(object sender, EventArgs e)
        {
            if (HttpContext.Current.Request.Url.AbsolutePath.StartsWith("/api/"))
            {
                HttpContext.Current.SetSessionStateBehavior(SessionStateBehavior.Required);
            }
        }
    }
}
