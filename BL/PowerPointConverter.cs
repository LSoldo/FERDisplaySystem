using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Office.Interop.PowerPoint;
using Microsoft.Office.Core;

namespace BL
{
    public class PowerPointConverter
    {
        public string GetVideoFromPpt(string inputPath, string outputPath)
        {
            var app = new Microsoft.Office.Interop.PowerPoint.Application();
            var presentation = app.Presentations.Open(inputPath, MsoTriState.msoFalse, MsoTriState.msoFalse, MsoTriState.msoFalse);

            var fileName = Guid.NewGuid() + ".mp4";
            var fullpath = Path.Combine(outputPath, fileName);

            try
            {
                presentation.CreateVideo(fullpath);
                while (presentation.CreateVideoStatus == PpMediaTaskStatus.ppMediaTaskStatusInProgress)
                    Thread.Sleep(100);

                //presentation.SaveCopyAs(fullpath, PpSaveAsFileType.ppSaveAsMP4, MsoTriState.msoCTrue);               
            }
            catch (COMException)
            {
                fileName = null;
            }
            finally
            {
                presentation.Close();
                app.Quit();
            }
            return fileName;
        }
    }
}
