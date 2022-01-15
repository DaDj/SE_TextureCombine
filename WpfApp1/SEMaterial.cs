using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Drawing;


namespace WpfApp1
{
    public enum ChannelState
    {
        none,
        on,
        off
    }

    public enum ChannelType
    {
        real,
        fake
    }

    public enum IMGTypes
    {
        Color,
        NG,
        Alpha,
        Paint,
        AO,
        Metalness,
        Gloss,
        Emissive
    }

    public class SEMaterialChannel
    {
        public String Path = "";
        public Bitmap TheTexture;
        public ChannelState State = ChannelState.off;
        public ChannelType Type = ChannelType.real;
    }

    public class SEMaterial
    {
        public SEMaterialChannel Color = new SEMaterialChannel();
        public SEMaterialChannel NG = new SEMaterialChannel();
        public SEMaterialChannel Metalness = new SEMaterialChannel();
        public SEMaterialChannel Gloss = new SEMaterialChannel();
        public SEMaterialChannel AO = new SEMaterialChannel();
        public SEMaterialChannel Paint = new SEMaterialChannel();
        public SEMaterialChannel Emissive = new SEMaterialChannel();
        public SEMaterialChannel Alpha = new SEMaterialChannel();

        public string SearchforChannelImage(string Dir, IMGTypes TheType)
        {
            List<String> ThePattern = new List<String> { "", "", "" }; ;
            switch (TheType)
            {
                case IMGTypes.Color:
                    ThePattern = new List<String> { "Basecolor", "Color", "Albedo" };
                    break;
                case IMGTypes.NG:
                    ThePattern = new List<String> { "Normalmap", "Normal" };
                    break;
                case IMGTypes.Alpha:
                    ThePattern = new List<String> { "Alpha", "Mask", "Alphamask", "Opacity" };
                    break;
                case IMGTypes.Paint:
                    ThePattern = new List<String> { "Paintable", "Paint", "Colorable" };
                    break;
                case IMGTypes.AO:
                    ThePattern = new List<String> { "_AO", "AmbientOcclusion" };
                    break;
                case IMGTypes.Metalness:
                    ThePattern = new List<String> { "Metallic", "Metalness" };
                    break;
                case IMGTypes.Gloss:
                    ThePattern = new List<String> { "Roughness","Gloss","Rough"};
                    break;
                case IMGTypes.Emissive:
                    ThePattern = new List<String> { "Emissive", "Emmissivity" };
                    break;
                default:
                    break;

            }

            string TheImagePath = "";

            List<String> fileEntries = new List<String>();
            foreach (var item in (Directory.GetFiles(Dir)))
            {
                fileEntries.Add(Path.GetFileName(item));
            }

            bool b;

            foreach (var item in fileEntries)
            {
                b = ThePattern.Any(s => item.IndexOf(s, StringComparison.OrdinalIgnoreCase) >= 0);
                if (b)
                {

                    TheImagePath = Dir + "//" + item;
                    break;
                }
            }
            return TheImagePath;
        }
        public void TryFindImages(string Texturepath)
        {
            //Find other images which could fit in folder. By the name
            string Basedir = Path.GetDirectoryName(Texturepath);

            if (Color.Path == "")
                Color.Path = SearchforChannelImage(Basedir, IMGTypes.Color);


            if (NG.Path == "")
                NG.Path = SearchforChannelImage(Basedir, IMGTypes.NG);

            if (Metalness.Path == "")
                Metalness.Path = SearchforChannelImage(Basedir, IMGTypes.Metalness);

            if (Gloss.Path == "")
                Gloss.Path = SearchforChannelImage(Basedir, IMGTypes.Gloss);

            if (AO.Path == "")
                AO.Path = SearchforChannelImage(Basedir, IMGTypes.AO);
            if (Alpha.Path == "")
                Alpha.Path = SearchforChannelImage(Basedir, IMGTypes.Alpha);

            if (Paint.Path == "")
                Paint.Path = SearchforChannelImage(Basedir, IMGTypes.Paint);
        }

        public Bitmap CreateFakeMap(int Size, SEMaterialChannel Channel)
        {
            Bitmap bmp = new Bitmap(Size, Size);
            if (Channel.State == ChannelState.on) 
                using (Graphics g = Graphics.FromImage(bmp)) { g.Clear(System.Drawing.Color.White); }
            else 
                using (Graphics g = Graphics.FromImage(bmp)) { g.Clear(System.Drawing.Color.Black); }

            Channel.TheTexture = bmp;
            return bmp;
        }
        public void Reset()
        {
            Color = new SEMaterialChannel();
            NG= new SEMaterialChannel();
            Metalness = new SEMaterialChannel();
            Metalness.State = ChannelState.off;
            Gloss = new SEMaterialChannel();
            Gloss.State = ChannelState.off;
            AO = new SEMaterialChannel();
            Paint = new SEMaterialChannel();
            Paint.State = ChannelState.off;
            Emissive = new SEMaterialChannel();
            Emissive.State = ChannelState.off;
        }
    }



}
