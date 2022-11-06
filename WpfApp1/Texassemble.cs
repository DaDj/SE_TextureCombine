using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;

namespace ShittyMaterialCreator
{
    class Texassemble
    {
        public static Process StartProcess(string executable, string commandline)
        {
            try
            {
                ProcessStartInfo sInfo = new ProcessStartInfo();
                var myProcess = new Process();
                myProcess.StartInfo = sInfo;
                sInfo.CreateNoWindow = true;
                sInfo.FileName = executable;
                sInfo.Arguments = commandline;
                myProcess.StartInfo.UseShellExecute = false;
                myProcess.StartInfo.RedirectStandardOutput = true;
                myProcess.OutputDataReceived += (sender, args) =>
                {
                    //Console.WriteLine(args.Data); lock (consoleBuffer) { consoleBuffer.Enqueue(args.Data); }
                };
                myProcess.Start();
                myProcess.BeginOutputReadLine();
                return myProcess;
            }
            catch { Console.WriteLine("Failed to start process"); }
            return null;
        }


        static string strExeFilePath = System.Reflection.Assembly.GetExecutingAssembly().Location;
        static string strWorkPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);

        static public void TextAssembleOne(string Output, string Img1, string Img2, string RGBAmask)
        {
            string cmdArgs = string.Format("merge -o {0}  -f R8G8B8A8_UNORM  -swizzle {1}  {2} {3}", Output, RGBAmask, Img1, Img2);


            if (System.IO.File.Exists(Output))
                System.IO.File.Delete(Output);

            var newProcess = StartProcess(strWorkPath + "\\texassemble.exe", cmdArgs);
            if (newProcess != null)
            {
                newProcess.WaitForExit();
            }


        }

        public enum RGBAMASK
        {
            _cm,
            _ng,
            _add1,
            _add2
        }

        static void Assemble_CM(SEMaterial TheMaterial, string Output)
        {
            string Mask = "rgbR";

            TheMaterial.Color.TheTexture.Save(strWorkPath + "\\Temp" + @"\Color.tif", System.Drawing.Imaging.ImageFormat.Tiff);
            TheMaterial.Color.Path = strWorkPath + "\\Temp" + @"\Color.tif";

            TheMaterial.Metalness.TheTexture.Save(strWorkPath + "\\Temp" + @"\Metalness.tif", System.Drawing.Imaging.ImageFormat.Tiff);
            TheMaterial.Metalness.Path = strWorkPath + "\\Temp" + @"\Metalness.tif";

            TextAssembleOne(Output +  @"_cm.tif", TheMaterial.Color.Path, TheMaterial.Metalness.Path, Mask);
        }


        static void Assemble_NG(SEMaterial TheMaterial, string Output)
        {
            string Mask = "rgbR";

            TheMaterial.NG.TheTexture.Save(strWorkPath + "\\Temp" + @"\Normal.tif", System.Drawing.Imaging.ImageFormat.Tiff);
            TheMaterial.Gloss.Path = strWorkPath + "\\Temp" + @"\Normal.tif";
            TheMaterial.Gloss.TheTexture.Save(strWorkPath + "\\Temp" + @"\Gloss.tif", System.Drawing.Imaging.ImageFormat.Tiff);
            TheMaterial.Gloss.Path = strWorkPath + "\\Temp" + @"\Gloss.tif";

            TextAssembleOne(Output + @"_ng.tif", TheMaterial.NG.Path, TheMaterial.Gloss.Path, Mask);
           
        }

        static void Assemble_ADD(SEMaterial TheMaterial, string Output)
        {
            string Mask = "rG00";

            TheMaterial.AO.TheTexture.Save(strWorkPath + "\\Temp" + @"\AO.tif", System.Drawing.Imaging.ImageFormat.Tiff);
            TheMaterial.AO.Path = strWorkPath + "\\Temp" + @"\AO.tif";

            TheMaterial.Emissive.TheTexture.Save(strWorkPath + "\\Temp" + @"\Emissive.tif", System.Drawing.Imaging.ImageFormat.Tiff);
            TheMaterial.Emissive.Path = strWorkPath + "\\Temp" + @"\Emissive.tif";

            TextAssembleOne(Output + @"_add.tif", TheMaterial.AO.Path, TheMaterial.Emissive.Path, Mask);

        }


        public static void Generate_Material(SEMaterial TheMaterial)
        {
            string ThePath = strWorkPath + "\\" + TheMaterial.Name;

            if (!(System.IO.Directory.Exists(strWorkPath + "\\Temp")))
                System.IO.Directory.CreateDirectory(strWorkPath + "\\Temp");

            Assemble_CM(TheMaterial, ThePath);
            Assemble_NG(TheMaterial, ThePath);
            Assemble_ADD(TheMaterial, ThePath);
        }


    }
}
