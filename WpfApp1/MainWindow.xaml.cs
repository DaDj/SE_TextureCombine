using System;
using System.Windows;
using System.Windows.Media.Imaging;
using System.IO;
using System.Drawing;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace ShittyMaterialCreator
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static IEnumerable<string> LongestCommonSubstrings(List<string> strings)
        {
            var firstString = strings.FirstOrDefault();

            var allSubstrings = new List<string>();
            for (int substringLength = firstString.Length - 1; substringLength > 0; substringLength--)
            {
                for (int offset = 0; (substringLength + offset) < firstString.Length; offset++)
                {
                    string currentSubstring = firstString.Substring(offset, substringLength);
                    if (!System.String.IsNullOrWhiteSpace(currentSubstring) && !allSubstrings.Contains(currentSubstring))
                    {
                        allSubstrings.Add(currentSubstring);
                    }
                }
            }

            return allSubstrings.OrderBy(subStr => subStr).ThenByDescending(subStr => subStr.Length).Where(subStr => strings.All(currentString => currentString.Contains(subStr)));
        }

        private BitmapImage BitmapToImageSource(System.Drawing.Bitmap bitmap)
        {
            using (MemoryStream memory = new MemoryStream())
            {
                bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Bmp);
                memory.Position = 0;
                BitmapImage bitmapimage = new BitmapImage();
                bitmapimage.BeginInit();
                bitmapimage.StreamSource = memory;
                bitmapimage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapimage.EndInit();
                return bitmapimage;
            }
        }


        public static SEMaterial TheMaterial;


        public MainWindow()
        {
            TheMaterial = new SEMaterial();
            InitializeComponent();

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

        public BitmapImage GetBitmapfromPath(string Path)
        {
            return new BitmapImage(new Uri(Path));
        }
        public void UpdateUIImages()
        {
            BitmapImage TheImage = new BitmapImage(new Uri("ressources/icons8-drag-and-drop-100.png", UriKind.Relative)); ;

            System.Drawing.Image img = null;

            //= System.Drawing.Image.FromFile(@"c:\ggs\ggs Access\images\members\1.jpg");



            if (File.Exists(TheMaterial.Color.Path))
            {
                ImgColor.Source = BitmapToImageSource(TheMaterial.CreateBitmapfromFile(TheMaterial.Color));
                img = System.Drawing.Image.FromFile(TheMaterial.Color.Path);
            }
            else
                ImgColor.Source = TheImage;


            if (chbx_Flipnormalgreen.IsChecked == true)
                TheMaterial.NG.Invert = Inverttype.G;
            else
                TheMaterial.NG.Invert = Inverttype.none;

            if (File.Exists(TheMaterial.NG.Path))
            {
                TheMaterial.CreateBitmapfromFile(TheMaterial.NG);
                if (chbx_Flipnormalgreen.IsChecked == true)
                    TheMaterial.NG.TheTexture = TheMaterial.Transform(TheMaterial.NG.TheTexture, "G");
                ImgNormal.Source = BitmapToImageSource(TheMaterial.NG.TheTexture);
                img = System.Drawing.Image.FromFile(TheMaterial.NG.Path);
            }
            else
                ImgNormal.Source = TheImage;



            int size = 1024;
            if (img != null)
                size = img.Width;

           if (chb_InvertAO.IsChecked == true)
                TheMaterial.AO.Invert = Inverttype.RGB;
            else
                TheMaterial.AO.Invert = Inverttype.none;

            if (File.Exists(TheMaterial.AO.Path))
            {
                TheMaterial.CreateBitmapfromFile(TheMaterial.AO);

                if (chb_InvertAO.IsChecked == true)
                   TheMaterial.AO.TheTexture = TheMaterial.Transform(TheMaterial.AO.TheTexture, "RGBA");
                ImgAO.Source = BitmapToImageSource(TheMaterial.AO.TheTexture);
            }
              
            else
            {
                TheMaterial.AO.State = ChannelState.on;
                ImgAO.Source = BitmapToImageSource(TheMaterial.CreateFakeMap(size, TheMaterial.AO));
            }



            if (chbx_InvertGloss.IsChecked == true)
                TheMaterial.Gloss.Invert = Inverttype.RGB;
            else
                TheMaterial.Gloss.Invert = Inverttype.none;

            if (File.Exists(TheMaterial.Gloss.Path))
            {
                TheMaterial.CreateBitmapfromFile(TheMaterial.Gloss);

                if (chbx_InvertGloss.IsChecked == true)
                    TheMaterial.Gloss.TheTexture = TheMaterial.Transform(TheMaterial.Gloss.TheTexture,"RGBA");
                ImgRoughness.Source = BitmapToImageSource(TheMaterial.Gloss.TheTexture);
            }
            else
                ImgRoughness.Source = BitmapToImageSource(TheMaterial.CreateFakeMap(size, TheMaterial.Gloss));





            if (File.Exists(TheMaterial.Metalness.Path))
                ImgMetal.Source = BitmapToImageSource(TheMaterial.CreateBitmapfromFile(TheMaterial.Metalness));
            else
                ImgMetal.Source = BitmapToImageSource(TheMaterial.CreateFakeMap(size, TheMaterial.Metalness));

            if (File.Exists(TheMaterial.Alpha.Path))
                ImgAlpha.Source = BitmapToImageSource(TheMaterial.CreateBitmapfromFile(TheMaterial.Alpha));
            else
                ImgAlpha.Source = BitmapToImageSource(TheMaterial.CreateFakeMap(size, TheMaterial.Alpha));

            if (File.Exists(TheMaterial.Emissive.Path))
                ImgEmissive.Source = BitmapToImageSource(TheMaterial.CreateBitmapfromFile(TheMaterial.Emissive));
            else
                ImgEmissive.Source = BitmapToImageSource(TheMaterial.CreateFakeMap(size, TheMaterial.Emissive));

            if (File.Exists(TheMaterial.Paint.Path))
                ImgPaint.Source = BitmapToImageSource(TheMaterial.CreateBitmapfromFile(TheMaterial.Paint));
            else
                ImgPaint.Source = BitmapToImageSource(TheMaterial.CreateFakeMap(size, TheMaterial.Paint));

        }

        public void TryParseImage(string TheDropped, IMGTypes TheType)
        {

            switch (TheType)
            {
                case IMGTypes.Color:
                    TheMaterial.Color.Path = TheDropped;
                    break;
                case IMGTypes.NG:
                    TheMaterial.NG.Path = TheDropped;
                    break;
                case IMGTypes.Alpha:
                    TheMaterial.Alpha.Path = TheDropped;
                    break;
                case IMGTypes.Paint:
                    TheMaterial.Paint.Path = TheDropped;
                    break;
                case IMGTypes.AO:
                    TheMaterial.AO.Path = TheDropped;
                    break;
                case IMGTypes.Metalness:
                    TheMaterial.Metalness.Path = TheDropped;
                    break;
                case IMGTypes.Gloss:
                    TheMaterial.Gloss.Path = TheDropped;
                    break;
                case IMGTypes.Emissive:
                    TheMaterial.Emissive.Path = TheDropped;
                    break;
                default:
                    break;
            }



            TheMaterial.TryFindImages(TheDropped);
            string Basedir = Path.GetDirectoryName(TheDropped);

            List<String> fileEntries = new List<String>();
            foreach (var item in (Directory.GetFiles(Basedir)))
            {
                fileEntries.Add(Path.GetFileName(item));
            }

            List<String> thenames  = new List<String>();
            thenames = LongestCommonSubstrings(fileEntries).OrderBy(x => x.Length).ToList();
           
            TheMaterial.Name = Regex.Replace(thenames.Last(), "[^A-Za-z0-9 ]", "");
            TextBox_TheMaterialT.Text = TheMaterial.Name;
            UpdateUIImages();
        }

        public void PerformImageDrop(DragEventArgs e, IMGTypes TheType)
        {
            var files = (string[])e.Data.GetData(DataFormats.FileDrop);

            if (Chbox_ResetatDrop.IsChecked == true)
            {
                TheMaterial.Reset();
            }

            TryParseImage(files[0], TheType);
        }

        private void Btn_SetNOTPaintable_Click(object sender, RoutedEventArgs e)
        {
            TheMaterial.Paint.Path = "";
            TheMaterial.Paint.State = ChannelState.off;
            UpdateUIImages();
        }
        private void Btn_SetPaintable_Click(object sender, RoutedEventArgs e)
        {
            TheMaterial.Paint.Path = "";
            TheMaterial.Paint.State = ChannelState.on;
            UpdateUIImages();
        }
        private void Btn_resetMaps_Click(object sender, RoutedEventArgs e)
        {
            TheMaterial.Reset();
            UpdateUIImages();
        }


        private void Btn_SetNotEmissive_Click(object sender, RoutedEventArgs e)
        {
            TheMaterial.Emissive.Path = "";
            TheMaterial.Emissive.State = ChannelState.off;
            UpdateUIImages();
        }

        private void Btn_SetEmissive_Click(object sender, RoutedEventArgs e)
        {
            TheMaterial.Emissive.Path = "";
            TheMaterial.Emissive.State = ChannelState.on;
            UpdateUIImages();
        }

        //Image Drop EVENTS 
        private void ImgColor_Drop(object sender, DragEventArgs e)
        {
            PerformImageDrop(e, IMGTypes.Color);
        }
        private void ImgNormal_Drop(object sender, DragEventArgs e)
        {
            PerformImageDrop(e, IMGTypes.NG);
        }
        private void ImgAO_Drop(object sender, DragEventArgs e)
        {
            PerformImageDrop(e, IMGTypes.AO);
        }
        private void ImgPaint_Drop(object sender, DragEventArgs e)
        {
            PerformImageDrop(e, IMGTypes.Paint);
        }
        private void ImgMetal_Drop(object sender, DragEventArgs e)
        {
            PerformImageDrop(e, IMGTypes.Metalness);
        }
        private void ImgRoughness_Drop(object sender, DragEventArgs e)
        {
            PerformImageDrop(e, IMGTypes.Gloss);
        }
        private void ImgAlpha_Drop(object sender, DragEventArgs e)
        {
            PerformImageDrop(e, IMGTypes.Alpha);
        }
        private void ImgEmissive_Drop(object sender, DragEventArgs e)
        {
            PerformImageDrop(e, IMGTypes.Emissive);
        }

        private void Btn_CreateMaterial_Click(object sender, RoutedEventArgs e)
        {
            Texassemble.Generate_Material(TheMaterial);
            UpdateUIImages();
        }


        private void chbx_Flipnormalgreen_Click(object sender, RoutedEventArgs e)
        {
            if(chbx_Flipnormalgreen.IsChecked == true)
            TheMaterial.NG.Invert = Inverttype.G;
            else
            TheMaterial.NG.Invert = Inverttype.none;
            UpdateUIImages();
        
        }

        private void chbx_InvertGloss_Click(object sender, RoutedEventArgs e)
        {
            if(chbx_InvertGloss.IsChecked == true)
                TheMaterial.Gloss.Invert = Inverttype.RGB;
            else
                TheMaterial.Gloss.Invert = Inverttype.none;
            UpdateUIImages();
        }

        private void chb_InvertAO_Click(object sender, RoutedEventArgs e)
        {
            if (chb_InvertAO.IsChecked == true)
                TheMaterial.AO.Invert = Inverttype.RGB;
            else
                TheMaterial.AO.Invert = Inverttype.none;
                UpdateUIImages();
        }

        private void TextBox_TheMaterialT_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            TheMaterial.Name = TextBox_TheMaterialT.Text;
        }
    }
}
