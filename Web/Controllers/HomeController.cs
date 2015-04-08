using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BL;
using DAL;
using DAL.Factories;
using DAL.Interfaces;
using DAL.Model;
using Newtonsoft.Json;

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
            var sceneFactory = new SceneFactory();
            var sequenceFactory = new SequenceFactory();

            var rss = sceneFactory.GetScene(DataDefinition.SceneType.Clock);
            rss.Init("ime", "opis", new List<string>() { "" }, true);

            var video = sceneFactory.GetScene(DataDefinition.SceneType.Video);
            video.Init("ime", "opis", new List<string>() { "http://clips.vorwaerts-gmbh.de/big_buck_bunny.mp4" }, true);

            var sequenceScene = new List<SequenceScene>
            {
                new SequenceScene() {Duration = TimeSpan.FromMilliseconds(10000), Scene = rss},
            };

            var sequence = sequenceFactory.GetSequence(DataDefinition.SequenceType.MaxImage);
            sequence.Init("Sekvenca", "opis", sequenceScene);

            return Content(sequence.HtmlContent);
        }

        public ActionResult Calendar()
        {
            var manager = new DisplaySettingsManager();
            DisplaySetting setting;
            var data = manager.CreateDisplayTimes(DateTime.Now, DateTime.Now.AddHours(2), TimeSpan.FromHours(3), 5,
                out setting);

            ViewBag.CalendarEvents = from d in data
                                     select new { start = d.TimeFrom, end = d.TimeTo, title = "Name" };
            return View();
        }
    }
}