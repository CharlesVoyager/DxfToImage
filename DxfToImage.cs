using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Threading.Tasks;

namespace DxfToImage
{
    public class DxfToImg
    {
        private const string SOURCE_X = " 10";
        private const string SOURCE_Y = " 20";
        private const string DEST_X = " 11";
        private const string DEST_Y = " 21";

        public static float Width { get; set; }
        public static float Height { get; set; }

        public Bitmap ConvertedImage;

        public DxfToImg()
        {
        }

        // EX: dxfToImg.Convert(fi, System.Drawing.Imaging.ImageFormat.Png, Tools.GetBuildwareLogsPath());
        //     Layer0.dxf ==> Layer0.png
        public void Convert(string dxfFullName, System.Drawing.Imaging.ImageFormat imageFormat, string DestFolder)
        {
            int imgWidth = 310;
            int imgHeight = 328;

            ConvertedImage = new Bitmap(imgWidth, imgHeight);

            var lines = File.ReadAllLines(dxfFullName);
            string prevString = "";

            float centerX = 155.0f;
            float centerY = 164.0f;
            float sourceX = 0.0f;
            float sourceY = 0.0f;
            float destX = 0.0f;
            float destY = 0.0f;
            using (Graphics g = Graphics.FromImage(ConvertedImage))
            {
                g.DrawLine(new Pen(Color.Black, 2), 0, 0, 0, imgHeight);
                g.DrawLine(new Pen(Color.Black, 2), 0, 0, imgWidth, 0);
                g.DrawLine(new Pen(Color.Black, 2), 0, imgHeight, imgWidth, imgHeight);
                g.DrawLine(new Pen(Color.Black, 2), imgWidth, 0, imgWidth, imgHeight);

                foreach (var line in lines)
                {
                    if (prevString != SOURCE_X && prevString != SOURCE_Y
                        && prevString != DEST_X && prevString != DEST_Y)
                    {
                        prevString = line;
                        continue;
                    }

                    if (prevString == SOURCE_X)
                    {
                        sourceX = centerX + float.Parse(line, System.Globalization.CultureInfo.InvariantCulture);
                    }
                    else if (prevString == SOURCE_Y)
                    {
                        sourceY = centerY - float.Parse(line, System.Globalization.CultureInfo.InvariantCulture);
                    }
                    else if (prevString == DEST_X)
                    {
                        destX = centerX + float.Parse(line, System.Globalization.CultureInfo.InvariantCulture);
                    }
                    else if (prevString == DEST_Y)
                    {
                        destY = centerY - float.Parse(line, System.Globalization.CultureInfo.InvariantCulture);

                        g.DrawLine(new Pen(Color.Blue), sourceX, sourceY, destX, destY);
                    }

                    prevString = line;
                }

                if (DestFolder != "")
                {
                    string newFile = DestFolder + Path.GetFileNameWithoutExtension(dxfFullName) + ".png";
                    if (File.Exists(newFile))
                        File.Delete(newFile);
                    ConvertedImage.Save(newFile, imageFormat);
                }
            }
        }

        public void Convert(FileInfo dxfFile, System.Drawing.Imaging.ImageFormat imageFormat, string DestFolder)
        {
            Convert(dxfFile.FullName, imageFormat, DestFolder);
        }

        private void deleteDirectory(string path)
        {
            string[] files = Directory.GetFiles(path);

            foreach (string file in files)
            {
                System.GC.Collect();
                System.GC.WaitForPendingFinalizers();
                File.SetAttributes(file, FileAttributes.Normal);
                File.Delete(file);
            }
            try
            {
                Directory.Delete(path, false);
            }
            catch
            {
                deleteDirectory(path);
            }
        }

        public void ConvertMultipleDXF(string folder)
        {
            string ConvertedDXFFolder = @"D:\ConvertedDXF\";
            if (!Directory.Exists(ConvertedDXFFolder))
            {
                Directory.CreateDirectory(ConvertedDXFFolder);
            }
            else
            {
                deleteDirectory(ConvertedDXFFolder);
                Directory.CreateDirectory(ConvertedDXFFolder);
            }

            DxfToImg dxfConversion = new DxfToImg();
            var fileList = new DirectoryInfo(folder).GetFiles("*.dxf", SearchOption.AllDirectories);
            int count = fileList.Count();
            Thread thread;
            if (fileList.Count() > 0)
            {
                thread = new System.Threading.Thread(() =>
                {
                    int i = 1;
                    foreach (var file in fileList)
                    {
                        dxfConversion.Convert(file, System.Drawing.Imaging.ImageFormat.Png, ConvertedDXFFolder);
                        i++;
                    }
                });
                thread.Start();
                while (thread.IsAlive) { Application.DoEvents(); }
            }
        }

        public static void RemoveImagesFolder(string folder)
        {
            if (Directory.Exists(folder))
            {
                Thread thread1;
                thread1 = new Thread(() =>
                {
                    Directory.Delete(folder, true);
                });
                thread1.Start();
                while (thread1.IsAlive) { Application.DoEvents(); }
            }
        }
    }
}
