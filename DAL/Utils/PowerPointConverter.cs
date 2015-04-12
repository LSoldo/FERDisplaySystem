using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Office.Interop.PowerPoint;
using Microsoft.Office.Core;

namespace DAL.Utils
{
    public class PowerPointConverter
    {
        public string GetVideoFromPpt(string inputPath, string outputPath)
        {
            var app = new Microsoft.Office.Interop.PowerPoint.Application();
            var presentation = app.Presentations.Open(inputPath, MsoTriState.msoTrue, MsoTriState.msoTrue, MsoTriState.msoFalse);

            var mp4FileName = Guid.NewGuid() + ".wmv";
            var fullpath = Path.Combine(outputPath, Path.GetFileName(inputPath));

            try
            {
                presentation.CreateVideo(mp4FileName);
                presentation.SaveCopyAs(fullpath, PpSaveAsFileType.ppSaveAsWMV, MsoTriState.msoCTrue);
            }
            catch (COMException ex)
            {
                mp4FileName = null;
                throw new Exception("A message occured: " + ex.Message);
            }
            finally
            {
                app.Quit();
            }

            return mp4FileName;
        }
    }
}
