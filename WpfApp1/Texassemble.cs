using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;

namespace WpfApp1
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
       static  string strWorkPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);

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

            if (TheMaterial.Metalness.Path == "")
            {
                TheMaterial.Metalness.TheTexture.Save(strWorkPath + @"\Metalnessfake.tif", System.Drawing.Imaging.ImageFormat.Tiff);
                TheMaterial.Metalness.Path = strWorkPath + @"\Metalnessfake.tif";
            }


            TextAssembleOne(strWorkPath + @"\test_cm.tif", TheMaterial.Color.Path, TheMaterial.Metalness.Path, Mask);
        }


        static void  Assemble_NG(SEMaterial TheMaterial, string Output)
        {
            string Mask =  "rgbR";


            if (TheMaterial.Gloss.Path == "")
            {
                TheMaterial.Gloss.TheTexture.Save(strWorkPath + @"\Glossfake.tif", System.Drawing.Imaging.ImageFormat.Tiff);
                TheMaterial.Gloss.Path = strWorkPath + @"\Glossfake.tif";
            }


            TextAssembleOne(strWorkPath + @"\test_ng.tif", TheMaterial.NG.Path, TheMaterial.Gloss.Path, Mask);
            // on NG do texconv + -inverty:  to flip green channel
        }



        public static void Generate_Material(SEMaterial TheMaterial)
        {
            string ThePath = "C:\\";
            Assemble_CM(TheMaterial,ThePath);
            Assemble_NG(TheMaterial, ThePath);
        }


        public string getRGBAMask(RGBAMASK TheMask, SEMaterial TheMaterial)
        {
            string TheResult = "";

            if (TheMask == RGBAMASK._cm)
            {
                if (TheMaterial.Metalness.Path != "")
                    TheResult = "rgbR";
                else if (TheMaterial.Metalness.State == ChannelState.on)
                    TheResult = "rgb1";
                else
                    TheResult = "rgb0";
            }

            if (TheMask == RGBAMASK._ng)
            {
                if (TheMaterial.Gloss.Path != "")
                    TheResult = "rgbR";
                else if (TheMaterial.Gloss.State == ChannelState.on)
                    TheResult = "rgb1";
                else
                    TheResult = "rgb0";
            }

            if (TheMask == RGBAMASK._add1)
            {
                if (TheMaterial.AO.Path != "")
                    TheResult = "r";
                else
                    TheResult = "0";

                if (TheMaterial.Emissive.Path != "")
                    TheResult = TheResult + "R00";
                else if (TheMaterial.Emissive.State == ChannelState.on)
                    TheResult = TheResult + "100";
                else
                    TheResult = TheResult + "000";
            }

            if (TheMask == RGBAMASK._add2)
            {
                if (TheMaterial.AO.Path != "")
                    TheResult = "r";
                else
                    TheResult = "0";

                if (TheMaterial.Emissive.Path != "")
                    TheResult = TheResult + "r0";
                else if (TheMaterial.Emissive.State == ChannelState.on)
                    TheResult = TheResult + "10";
                else
                    TheResult = TheResult + "00";

                if (TheMaterial.Paint.Path != "")
                    TheResult = TheResult + "R";
                else if (TheMaterial.Paint.State == ChannelState.on)
                    TheResult = TheResult + "1";
                else
                    TheResult = TheResult + "0";

            }


            return TheResult;
        }

    }
}
