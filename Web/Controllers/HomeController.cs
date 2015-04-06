using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BL;
using DAL.Factories;
using DAL.Interfaces;
using DAL.Model;

namespace Web.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult Terminal()
        {
            SceneFactory sceneFactory = new SceneFactory();
            SequenceFactory sequenceFactory = new SequenceFactory();

            IScene rss = sceneFactory.GetScene(DataDefinition.SceneType.Clock);
            rss.Init("ime", "opis", new List<string>() { "" }, true);

            IScene video = sceneFactory.GetScene(DataDefinition.SceneType.Video);
            video.Init("ime", "opis", new List<string>() { "http://clips.vorwaerts-gmbh.de/big_buck_bunny.mp4" }, true);

            var sequenceScene = new List<SequenceScene>
            {
                new SequenceScene() {Duration = TimeSpan.FromMilliseconds(10000), Scene = rss},
            };

            var sequence = sequenceFactory.GetSequence(DataDefinition.SequenceType.MaxImage);
            sequence.Init("Sekvenca", "opis", sequenceScene);

            return Content(sequence.HtmlContent);
        }
    }
}