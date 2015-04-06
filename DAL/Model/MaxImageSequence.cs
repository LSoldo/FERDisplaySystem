using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using DAL.Interfaces;
using DAL.Utils;
using Newtonsoft.Json.Linq;

namespace DAL.Model
{
    public class MaxImageSequence : ISequence
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string HtmlContent { get; private set; }
        public List<string> Css { get; private set; }
        public List<string> Js { get; private set; }
        public string JavascriptFunctions { get; private set; }
        public string Type { get; private set; }
        public List<SequenceScene> Scenes { get; set; }

        public void Calculate()
        {
            using (var r = new StreamReader(DataDefinition.SequenceDefinition.Path))
            {
                ClearData();
                var builder = new PageBuilder();
                var json = r.ReadToEnd();
                dynamic definition = JObject.Parse(json);

                this.Css = definition.maximage.css.ToObject<List<string>>();
                this.Js = definition.maximage.js.ToObject<List<string>>();
                var javascriptFunctionsFromJsonFile =
                    this.JavascriptFunctions = string.Join(Environment.NewLine, definition.maximage.javascriptFunctions);
                this.JavascriptFunctions = string.Format(javascriptFunctionsFromJsonFile, DataDefinition.SequenceDefinition.StageDivName);

                //Adding css and js from scenes also
                this.Scenes.ForEach(s => s.Scene.Css.ForEach(this.Css.Add));
                this.Scenes.ForEach(s => s.Scene.Js.ForEach(this.Js.Add));
                this.Css = this.Css.Distinct().ToList();
                this.Js = this.Js.Distinct().ToList();

                this.HtmlContent += builder.AddHtml5Declaration();

                var headContent = builder.BuildHeadContent(this.Css, this.Js);

                var bodyContent = BuildBodyContent(this.JavascriptFunctions,
                    this.Scenes.Select(s => s.Scene.HtmlContent.Replace(Environment.NewLine, "")).ToList(),
                    this.Scenes.Select(s => s.Scene.JavascriptFunctions).ToList(),
                    this.Scenes.Select(scene => (long)scene.Duration.TotalMilliseconds).ToList() );

                this.HtmlContent += builder.AddHtmlTags(headContent + bodyContent);
            }
        }

        private static string BuildBodyContent(string mainJavascriptFunction,
            List<string> htmlDefinitionsForScenes,
            List<string> sceneJavascriptFunctions,
            List<long> sceneIntervals)
        {
            var outputContent = "";
            var builder = new PageBuilder();

            var content =
                builder.BuildVarArray(
                    string.Join(",", htmlDefinitionsForScenes.ConvertAll(i => "'" + i + "'")),
                    DataDefinition.SequenceDefinition.Content);

            //setting interval to max value so the page won't refresh very often
            if (sceneIntervals.Count == 1) sceneIntervals[0] = long.MaxValue;
            var intervals = builder.BuildVarArray(sceneIntervals.Select(i => i.ToString()).ToList(),
                DataDefinition.SequenceDefinition.Intervals);

            var sceneFunctions =
                sceneJavascriptFunctions.Select(function => builder.AddToArray(builder.CreateFunction(function)))
                    .ToList();

            var sceneDedicatedFunctions =
                builder.BuildVarArray(string.Join("," + Environment.NewLine, sceneFunctions),
                    DataDefinition.SequenceDefinition.CurrentFunctions);
            var jQueryOnDocumentReadyFunction =
                builder.CreatejQueryOnDocumentReadyFunction(content + intervals + sceneDedicatedFunctions +
                                                            mainJavascriptFunction);
            var sequenceMainFunction = builder.AddJsScript(jQueryOnDocumentReadyFunction);

            var emptyDivForSceneChange = builder.AddEmptyDivWithId(DataDefinition.SequenceDefinition.StageDivName);

            outputContent += builder.AddBody(emptyDivForSceneChange + sequenceMainFunction);

            return outputContent;
        }

        public void ClearData()
        {
            this.Css = null;
            this.Js = null;
            this.HtmlContent = "";
            this.JavascriptFunctions = "";
        }

        public void Init(string name, string description, List<SequenceScene> scenes)
        {
            this.Name = name;
            this.Description = description;
            this.Scenes = scenes;
            this.Type = DataDefinition.SequenceType.MaxImage;

            Calculate();
        }


        public void Add(SequenceScene scene)
        {
            this.Scenes.Add(scene);
        }
    }
}
