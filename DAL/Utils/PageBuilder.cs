using System;
using System.Collections.Generic;
using System.Linq;

namespace DAL.Utils
{
    public class PageBuilder
    {
        public string AddVarArray(List<string> values, string varName)
        {
            return "var " + varName + " = [" + string.Join(", ", values) + "];" + Environment.NewLine;
        }

        public string AddVarArray(string value, string varName)
        {
            return "var " + varName + " = [" + value + "];" + Environment.NewLine;
        }

        public string AddVar(string value, string varName)
        {
            return "var " + varName + " = " + value + ";" + Environment.NewLine;
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
            var mediaOption = string.IsNullOrEmpty(media) ? "" : media;
            var cssLayout = "<link rel=\"stylesheet\" href=\"{0}\" type=\"text/css\" {1} charset=\"{2}\" />";
            return string.Format(cssLayout + Environment.NewLine, css, mediaOption, charset);
        }

        public List<string> AddCss(List<string> cssArray, string media = "", string charset = "utf-8")
        {
            return cssArray.Select(css => AddCss(css, media, charset)).ToList();
        }

        public string AddJs(string js, string charset = "utf-8")
        {
            var jsLayout = "<script src=\"{0}\" type=\"text/javascript\" charset=\"{1}\"></script>";
            return string.Format(jsLayout + Environment.NewLine, js, charset);
        }

        public List<string> AddJs(List<string> jsArray, string charset = "utf-8")
        {
            return jsArray.Select(js => AddJs(js, charset)).ToList();
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
            return urls.Select(url => AddImg(url)).ToList();
        }

        public string AddImg(string url, string width = null, string height = null)
        {

            string dimensions = width == null && height == null
                ? "width=\"1400\" height=\"1050\""
                : string.Format("width=\"{0}\" height=\"{1}\"", width, height);
            return string.Format("<img src=\"{0}\" {1} />" , url, dimensions);
        }

        public string AddDiv(string content)
        {
            return "<div>"  + content + "</div>" ;
        }
        public string AddDivWithId(string content, string id)
        {
            return string.Format("<div id=\"{0}\">", id)  + content + "</div>" ;
        }

        public string AddDivWithStyle(string content, string style)
        {
            return string.Format("<div style=\"{0}\">", style)  + content + "</div>" ;
        }

        public string AddDivWithIdAndStyle(string content, string id, string style)
        {
            return string.Format("<div id=\"{0}\" style=\"{1}\">", id, style) + content + "</div>";
        }

        public string AddEmptyDivWithId(string id)
        {
            return string.Format("<div id=\"{0}\"></div>", id) ;
        }

        public string AddH1(string content)
        {
            return "<h1>"  + content + "</h1>" ;
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
            var outContent = content.Aggregate("", (current, function) => current + CreateFunction(function));

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
            return "[" + string.Join(",", content) + "]" + Environment.NewLine;
        }
        public string AddToArray(string content)
        {
            return "[" + content + "]" + Environment.NewLine;
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
            var outputContent = "";

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
