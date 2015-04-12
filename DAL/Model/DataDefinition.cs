using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL.Utils;

namespace DAL.Model
{
    public static class DataDefinition
    {
        public abstract class TerminalType
        {
            public static string Plasma = "Plasma";
            public static string LCD = "LCD";
            public static string CRT = "CRT";
            public static string LED = "LED";
        }

        public abstract class DisplayPeriod
        {
            public static string Periodically = "P";
            public static string NTimes = "NT";
        }

        public abstract class Resolution
        {
            public static string Standard = "Std";
        }

        public abstract class RepeatTimes
        {
            public static int Once = 1;
            public static int Twice = 2;
            public static int Thrice = 3;
            public static int Ten = 10;
            public static int Month = 30;
            public static int Hundred = 100;
            public static int Year = 365;
        }

        public abstract class Duration
        {
            public static long ThirtySeconds = 30000;
            public static long FiveMinutes = 300000;
        }

        public abstract class SceneType
        {
            public static string Rss = "RSS";
            public static string Html = "HTML";
            public static string Video = "Video";
            public static string Clock = "Clock";
            public static string PowerPoint = "PPT";
            public static string Slideshow = "Slideshow";
            public static string Text = "Text";
        }

        public abstract class SequenceType
        {
            public static string MaxImage = "Maximage";
        }

        public abstract class SceneDefinition
        {
            public static string Path
            {
                get
                {
                    return
                        @"C:\Users\Luka\Documents\Visual Studio 2013\Projects\FERDisplaySystem\DAL\SceneDefinition.json";
                }
            }
        }

        public abstract class SequenceDefinition
        {
            public static string Path
            {
                get
                {
                    return
                        @"C:\Users\Luka\Documents\Visual Studio 2013\Projects\FERDisplaySystem\DAL\CompositionDefinition.json";
                }
            }

            public static string Intervals = "intervals";
            public static string Content = "content";
            public static string CurrentFunctions = "currentFunctions";
            public static string StageDivName = "change";
            public static string RssSequenceDivId = "content-";
            public static string CssPathsArray = "cssPaths";
            public static string JsPathsArray = "jsPaths";
        }
    }
}
