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
    public class MaxImageSequenceGenerator : ISequenceGenerator
    {
        public int Id { get; set; }
        public virtual List<DataSource> Css { get; private set; }
        public virtual List<DataSource> Js { get; private set; }
        public string JavascriptFunctions { get; private set; }
        public string Type { get; private set; }
        public bool IsInitialized { get; set; }

        public MaxImageSequenceGenerator()
        {
            Init();
        }

        public string GenerateHtml(List<SequenceScene> scenes, string groupId, string terminalSequenceId)
        {
            if(!this.IsInitialized)
                Init();

            string htmlContent = "";
            var builder = new PageBuilder();

            htmlContent += builder.AddHtml5Declaration();
            var headContent = builder.BuildHeadContent(this.Css.Select(s => s.Path).ToList(), this.Js.Select(s => s.Path).ToList());
            var bodyContent = BuildBodyContent(scenes, groupId, terminalSequenceId);
            htmlContent += builder.AddHtmlTags(headContent + bodyContent);

            return htmlContent;
        }

        private string BuildBodyContent(List<SequenceScene> scenes, string groupId, string terminalSequenceId)
        {
            var htmlDefinitionsForScenes = scenes.Select(s => s.Setup.HtmlContent).ToList();
            var sceneIntervals = scenes.Select(scene => (long) scene.Duration.TotalMilliseconds).ToList();

            var outputContent = "";
            var builder = new PageBuilder();

            var group = builder.AddVar("\"" + groupId + "\"", DataDefinition.SequenceDefinition.GroupId);
            var sequenceId = builder.AddVar("\"" + terminalSequenceId + "\"", DataDefinition.SequenceDefinition.SequenceId);

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
                    string.Join(",", scenes.Select(scene => builder.AddToArray(scene.Setup.JsFunctionsToCall.ConvertAll(i => "'" + i + "'"))).ToList()),
                    DataDefinition.SequenceDefinition.CurrentFunctions);

            var cssPaths =
                builder.AddVarArray(
                    string.Join(",", scenes.Select(scene => builder.AddToArray(scene.Setup.CssPathList.ConvertAll(i => "'" + i + "'"))).ToList()),
                    DataDefinition.SequenceDefinition.CssPathsArray);

            var jsPaths =
                builder.AddVarArray(
                    string.Join(",", scenes.Select(scene => builder.AddToArray(scene.Setup.JsPathList.ConvertAll(i => "'" + i + "'"))).ToList()),
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

        public void Init()
        {
            this.Type = DataDefinition.SequenceType.MaxImage;

            using (var r = new StreamReader(DataDefinition.SequenceDefinition.Path))
            {
                ClearData();
                var json = r.ReadToEnd();
                dynamic definition = JObject.Parse(json);

                this.Css = TypeConverter.ConvertToDataSource((definition.maximage.css).ToObject<List<string>>());
                this.Js = TypeConverter.ConvertToDataSource((definition.maximage.js).ToObject<List<string>>());
                this.IsInitialized = true;
            }
        }

    }
}
