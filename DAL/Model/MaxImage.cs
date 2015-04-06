using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DAL.Interfaces;
using DAL.Utils;
using Newtonsoft.Json.Linq;

namespace DAL.Model
{
    public class MaxImage : ISequence
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string HtmlContent { get; private set; }
        public List<string> Css { get; private set; }
        public List<string> Js { get; private set; }
        public string JavascriptFunctions { get; private set; }
        public string Type { get; private set; }
        public List<IScene> Scenes { get; set; }
        public List<TimeInterval> TimeIntervals { get; set; }

        public void Calculate()
        {
            using (StreamReader r = new StreamReader(DataDefinition.SequenceDefinition.Path))
            {
                ClearData();
                PageBuilder builder = new PageBuilder();
                string json = r.ReadToEnd();
                dynamic definition = JObject.Parse(json);

                this.Css = definition.maximage.css.ToObject<List<string>>();
                this.Js = definition.maximage.js.ToObject<List<string>>();
                var javascriptFunctionsFromJsonFile =
                    this.JavascriptFunctions = string.Join(Environment.NewLine, definition.maximage.javascriptFunctions);
                this.JavascriptFunctions = string.Format(javascriptFunctionsFromJsonFile, DataDefinition.SequenceDefinition.StageDivName);

                //Adding css and js from scenes also
                this.Scenes.ForEach(s => s.Css.ForEach(this.Css.Add));
                this.Scenes.ForEach(s => s.Js.ForEach(this.Js.Add));
                this.Css = this.Css.Distinct().ToList();
                this.Js = this.Js.Distinct().ToList();

                this.HtmlContent += builder.AddHtml5Declaration();

                string headContent = builder.BuildHeadContent(this.Css, this.Js);

                var bodyContent = BuildBodyContent(this.JavascriptFunctions,
                    this.Scenes.Select(s => s.HtmlContent).ToList(),
                    this.Scenes.Select(s => s.JavascriptFunctions).ToList(),
                    new List<long>() { 50000, 4000 });

                this.HtmlContent += builder.AddHtmlTags(headContent + bodyContent);
            }
        }

        private string BuildBodyContent(string mainJavascriptFunction,
            List<string> htmlDefinitionsForScenes,
            List<string> sceneJavascriptFunctions,
            List<long> sceneIntervals)
        {
            string outputContent = "";
            PageBuilder builder = new PageBuilder();

            var content =
                builder.BuildVarArray(
                    string.Join("," + Environment.NewLine, htmlDefinitionsForScenes.ConvertAll(i => "'" + i + "'")),
                    DataDefinition.SequenceDefinition.Content);
            var intervals = builder.BuildVarArray(sceneIntervals.Select(i => i.ToString()).ToList(),
                DataDefinition.SequenceDefinition.Intervals);

            var formattedFunctions =
                sceneJavascriptFunctions.Select(function => builder.AddToArray(builder.CreateFunction(function)))
                    .ToList();

            var sceneDedicatedFunctions =
                builder.BuildVarArray(string.Join("," + Environment.NewLine, formattedFunctions),
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

        public void Init(string name, string description, List<IScene> scenes)
        {
            this.Name = name;
            this.Description = description;
            this.Scenes = scenes;
            this.Type = DataDefinition.SequenceType.MaxImage;

            Calculate();
        }


        public void Add(IScene scene)
        {
            this.Scenes.Add(scene);
        }
    }
}
