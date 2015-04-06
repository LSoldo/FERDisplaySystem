using System;
using System.Collections.Generic;

namespace DAL.Utils
{
    public class PageBuilder
    {
        public string BuildVarArray(List<string> values, string varName)
        {
            return "var " + varName + " = [" + string.Join(", ", values) + "];" + Environment.NewLine;
        }

        public string BuildVarArray(string content, string varName)
        {
            return "var " + varName + " = [" + content + "];" + Environment.NewLine;
        }

        public string RemoveItemInLocalStorage(string key)
        {
            return string.Format("localStorage.removeItem(\"{0}\");", key) + Environment.NewLine;
        }

        public string RemoveEntireLocalStorage()
        {
            return "localStorage.clear();" + Environment.NewLine;
        }

        public string AddLine(string line)
        {
            return line + Environment.NewLine;
        }

        public string AddLines(List<string> lines)
        {
            return string.Join(Environment.NewLine, lines);
        }

        public string AddCharset(string encoding = "Windows-1250")
        {
            return string.Format("<meta http-equiv=\"Content-Type\" content=\"text/html; charset={0}\" />", encoding);
        }

        public string AddCss(string css, string media = "", string charset = "utf-8")
        {
            string mediaOption = string.IsNullOrEmpty(media) ? "" : media;
            string cssLayout = "<link rel=\"stylesheet\" href=\"{0}\" type=\"text/css\" {1} charset=\"{2}\" />";
            return string.Format(cssLayout + Environment.NewLine, css, mediaOption, charset);
        }

        public List<string> AddCss(List<string> cssArray, string media = "", string charset = "utf-8")
        {
            List<string> cssBuilder = new List<string>();
            foreach(var css in cssArray)
                cssBuilder.Add(AddCss(css, media, charset));
            return cssBuilder;
        }

        public string AddJs(string js, string charset = "utf-8")
        {
            string jsLayout = "<script src=\"{0}\" type=\"text/javascript\" charset=\"{1}\"></script>";
            return string.Format(jsLayout + Environment.NewLine, js, charset);
        }

        public List<string> AddJs(List<string> jsArray, string charset = "utf-8")
        {
            List<string> jsBuilder = new List<string>();
            foreach (var js in jsArray)
                jsBuilder.Add(AddJs(js, charset));
            return jsBuilder;
        }

        public string AddJsScript(string content, string charset = "utf-8")
        {
            return
                string.Format("<script type=\"text/javascript\" charset=\"{0}\">", charset) +
                Environment.NewLine +
                content +
                Environment.NewLine +
                "</script>" +
                Environment.NewLine;
        }

        public List<string> AddImg(List<string> urls)
        {
            List<string> imgBuilder = new List<string>();
            foreach (var url in urls)
                imgBuilder.Add(AddImg(url));
            return imgBuilder;
        }

        public string AddImg(string url, string width = null, string height = null)
        {

            string dimensions = width == null && height == null
                ? "width=\"1400\" height=\"1050\""
                : string.Format("width=\"{0}\" height=\"{1}\"", width, height);
            return string.Format("<img src=\"{0}\" {1} />" + Environment.NewLine, url, dimensions);
        }

        public string AddDiv(string content)
        {
            return "<div>" + Environment.NewLine + content + "</div>" + Environment.NewLine;
        }
        public string AddDivWithId(string content, string id)
        {
            return string.Format("<div id=\"{0}\">", id) + Environment.NewLine + content + "</div>" + Environment.NewLine;
        }


        public string AddEmptyDivWithId(string id)
        {
            return string.Format("<div id=\"{0}\"></div>", id) + Environment.NewLine;
        }

        public string AddBody(string content)
        {
            return "<body>" + Environment.NewLine + content + "</body>" + Environment.NewLine;
        }
        public string AddHead(string content)
        {
            return "<head>" + Environment.NewLine + content + "</head>" + Environment.NewLine;
        }

        public string AddToListOfFunctions(List<string> content)
        {
            string outContent = "";
            foreach (var function in content)
                outContent += CreateFunction(function);

            return "[" + outContent + "]" + Environment.NewLine;
        }

        public string CreateFunction(string content)
        {
            return "function(){" + Environment.NewLine + content + Environment.NewLine + "}" + Environment.NewLine;
        }

        public string CreatejQueryOnDocumentReadyFunction(string content)
        {
            return "$(function(){" + Environment.NewLine + content + Environment.NewLine + "});" + Environment.NewLine;
        }

        public string AddToArray(List<string> content)
        {
            return "[" + string.Join("," + Environment.NewLine, content) + "]" + Environment.NewLine;
        }
        public string AddToArray(string content)
        {
            return "[" + content + Environment.NewLine + "]" + Environment.NewLine;
        }

        public string AddHtml5Declaration()
        {
            return "<!DOCTYPE html>" + Environment.NewLine;
        }

        public string AddHtmlTags(string content)
        {
            return "<html class='no-js' lang='en'>"+ Environment.NewLine + content + "</html>" + Environment.NewLine;
        }

        public string BuildHeadContent(List<string> cssPathList, List<string> jsPathList, string encoding = "Windows-1250")
        {
            string outputContent = "";

            outputContent += AddHead(
                AddLines(
                new List<string>(){
                    AddCharset(encoding),
                    string.Join(Environment.NewLine, AddJs(jsPathList)),
                    string.Join(Environment.NewLine, AddCss(cssPathList))
                }
                ));

            return outputContent;
        }
      
    }
}
