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
        public List<string> Css { get; private set; }
        public List<string> Js { get; private set; }
        public string JavascriptFunctions { get; private set; }
        public string Type { get; private set; }
        public List<SequenceScene> Scenes { get; set; }

        public string GenerateHtml(string groupId)
        {
            using (var r = new StreamReader(DataDefinition.SequenceDefinition.Path))
            {
                ClearData();
                string htmlContent = "";
                var builder = new PageBuilder();
                var json = r.ReadToEnd();
                dynamic definition = JObject.Parse(json);

                this.Css = definition.maximage.css.ToObject<List<string>>();
                this.Js = definition.maximage.js.ToObject<List<string>>();

                htmlContent += builder.AddHtml5Declaration();
                var headContent = builder.BuildHeadContent(this.Css, this.Js);
                var bodyContent = BuildBodyContent(this.Scenes, groupId);
                htmlContent += builder.AddHtmlTags(headContent + bodyContent);

                return htmlContent;
            }
        }

        private string BuildBodyContent(List<SequenceScene> scenes, string groupId)
        {
            var htmlDefinitionsForScenes = scenes.Select(s => s.Scene.HtmlContent).ToList();
            var sceneIntervals = scenes.Select(scene => (long) scene.Duration.TotalMilliseconds).ToList();

            var outputContent = "";
            var builder = new PageBuilder();

            var group = builder.AddVar("\"" + groupId + "\"", DataDefinition.SequenceDefinition.GroupId);
            var sequenceId = builder.AddVar("\"" + this.Id + "\"", DataDefinition.SequenceDefinition.SequenceId);

            var content =
                builder.AddVarArray(
                    string.Join(",", htmlDefinitionsForScenes.ConvertAll(i => "'" + i + "'")),
                    DataDefinition.SequenceDefinition.Content);

            //setting interval to max value so the page won't refresh very often
            if (sceneIntervals.Count == 1) sceneIntervals[0] = DataDefinition.Duration.FiveMinutes;
            var intervals = builder.AddVarArray(sceneIntervals.Select(i => i.ToString()).ToList(),
                DataDefinition.SequenceDefinition.Intervals);

            var sceneDedicatedFunctions =
                builder.AddVarArray(
                    string.Join(",", scenes.Select(scene => builder.AddToArray(scene.Scene.JavascriptFunctions.ConvertAll(i => "'" + i + "'"))).ToList()),
                    DataDefinition.SequenceDefinition.CurrentFunctions);

            var cssPaths =
                builder.AddVarArray(
                    string.Join(",", scenes.Select(scene => builder.AddToArray(scene.Scene.Css.ConvertAll(i => "'" + i + "'"))).ToList()),
                    DataDefinition.SequenceDefinition.CssPathsArray);

            var jsPaths =
                builder.AddVarArray(
                    string.Join(",", scenes.Select(scene => builder.AddToArray(scene.Scene.Js.ConvertAll(i => "'" + i + "'"))).ToList()),
                    DataDefinition.SequenceDefinition.JsPathsArray);

            var sequenceMainFunction = builder.AddJsScript(content + group + sequenceId + intervals + jsPaths + cssPaths + 
                                                           sceneDedicatedFunctions);

            var emptyDivForSceneChange = builder.AddEmptyDivWithId(DataDefinition.SequenceDefinition.StageDivName);

            outputContent += builder.AddBody(emptyDivForSceneChange + sequenceMainFunction);

            return outputContent;
        }

        public void ClearData()
        {
            this.Css = null;
            this.Js = null;
            this.JavascriptFunctions = "";
        }

        public void Init(string name, string description, List<SequenceScene> scenes)
        {
            this.Name = name;
            this.Description = description;
            this.Scenes = scenes;
            this.Type = DataDefinition.SequenceType.MaxImage;
        }

        public void Add(SequenceScene scene)
        {
            this.Scenes.Add(scene);
        }
    }
}
