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
using DAL.Utils;
using Microsoft.AspNet.SignalR;
using Newtonsoft.Json;

namespace Web.Controllers
{
    public class DisplayController : Controller
    {
        private IHubContext context;

        public DisplayController()
        {
            this.context = GlobalHost.ConnectionManager.GetHubContext<ConnectionHub>();
        }
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Terminal(string id)
        {
            var sceneFactory = new SceneGeneratorFactory();
            var sequenceFactory = new SequenceGeneratorFactory();

            //new List<string>() { "http://www.fer.unizg.hr/feed/rss.php?url=/"}
            var rss = sceneFactory.GetScene(DataDefinition.SceneType.Rss);
            rss.Init();
            rss.Id = 1;

            var video = sceneFactory.GetScene(DataDefinition.SceneType.Video);
            video.Init();

            var clock = sceneFactory.GetScene(DataDefinition.SceneType.Clock);
            clock.Init();

            var scene = new Scene("video", video.Type, new List<DataSource>() {new DataSource(){Path = "http://clips.vorwaerts-gmbh.de/big_buck_bunny.mp4"}});
            var sequenceScene = new List<SequenceScene>
            {
                new SequenceScene(scene, TimeSpan.FromMilliseconds(20000), true)
            };

            var sequence = sequenceFactory.GetSequence(DataDefinition.SequenceType.MaxImage);
            sequence.Init();
            sequence.Id = 1;

            return Content(sequence.GenerateHtml(sequenceScene, id, "1"));
        }

        public ActionResult Calendar()
        {
            var manager = new BLDisplaySettingsManager();
            DisplaySetting setting;
            var data = manager.CreateDisplayTimes(DateTime.Now, DateTime.Now.AddHours(2), TimeSpan.FromHours(3), 5,
                out setting);

            ViewBag.CalendarEvents = from d in data
                                     select new { start = d.TimeFrom, end = d.TimeTo, title = "Name" };
            return View();
        }
    }
}