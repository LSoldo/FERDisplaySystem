﻿using System;
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
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Terminal(string id)
        {
            var sceneFactory = new SceneFactory();
            var sequenceFactory = new SequenceFactory();

            var rss = sceneFactory.GetScene(DataDefinition.SceneType.Rss);
            rss.Init("ime", "opis", new List<string>() { "http://www.fer.unizg.hr/feed/rss.php?url=/" }, true);
            rss.Id = 1;

            var video = sceneFactory.GetScene(DataDefinition.SceneType.Video);
            video.Init("ime", "opis", new List<string>() { "http://clips.vorwaerts-gmbh.de/big_buck_bunny.mp4" }, true);

            var clock = sceneFactory.GetScene(DataDefinition.SceneType.Clock);
            clock.Init("ime", "opis", new List<string>() { "" }, false);

            var sequenceScene = new List<SequenceScene>
            {
                new SequenceScene(video, TimeSpan.FromMilliseconds(20000), true)
            };

            var sequence = sequenceFactory.GetSequence(DataDefinition.SequenceType.MaxImage);
            sequence.Init("Sekvenca", "opis", sequenceScene);
            sequence.Id = 1;

            //var converter = new PowerPointConverter();
            //converter.GetVideoFromPpt(@"C:\Users\Luka\Downloads\flip.ppt", @"C:\Users\Luka\Desktop");

            return Content(sequence.GenerateHtml(id));
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