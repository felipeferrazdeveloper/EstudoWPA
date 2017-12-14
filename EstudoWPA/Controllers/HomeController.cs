using Estudo.DB.Database;
using Estudo.DB.Database.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EstudoWPA.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Estabelecimento()
        {
            var estabelecimentos = DbConfig.Instance.EstabelecimentoRepository.FindAll();
            return View(estabelecimentos);
        }

        public ActionResult Quarto()
        {
            var quartos = DbConfig.Instance.QuartoRepository.FindAll();
            return View(quartos);
        }

        public ActionResult Locacao()
        {
            var locacoes = DbConfig.Instance.LocacaoRepository.FindAll();
            return View(locacoes);
        }

        public ActionResult CreateEstabelecimento()
        {
            var est = new Estabelecimento();
            return View(est);
        }

        public ActionResult CreateQuarto()
        {
            var qua = new Quarto();
            return View(qua);
        }

        public ActionResult CreateLocacao()
        {
            var loc = new Locacao();
            return View(loc);
        }


    }
}