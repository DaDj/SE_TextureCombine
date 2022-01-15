using System;
using System.Windows;
using System.Windows.Media.Imaging;
using System.IO;

namespace WpfApp1
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
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
                ImgColor.Source = GetBitmapfromPath(TheMaterial.Color.Path);
                img = System.Drawing.Image.FromFile(TheMaterial.Color.Path);
            }
            else
                ImgColor.Source = TheImage;


            if (File.Exists(TheMaterial.NG.Path))
            {
                ImgNormal.Source = GetBitmapfromPath(TheMaterial.NG.Path);
                img = System.Drawing.Image.FromFile(TheMaterial.Color.Path);
            }
            else
                ImgNormal.Source = TheImage;


          



            if (File.Exists(TheMaterial.AO.Path))
            {
                ImgAO.Source = GetBitmapfromPath(TheMaterial.AO.Path);
                img = System.Drawing.Image.FromFile(TheMaterial.Color.Path);
            }
            else
                ImgAO.Source = TheImage;

            int size = 1024;
            if (img != null)
                size = img.Width;

            if (File.Exists(TheMaterial.Gloss.Path))
                ImgRoughness.Source = GetBitmapfromPath(TheMaterial.Gloss.Path);
            else
                ImgRoughness.Source = BitmapToImageSource(TheMaterial.CreateFakeMap(size, TheMaterial.Gloss));

            if (File.Exists(TheMaterial.Metalness.Path))
                ImgMetal.Source = GetBitmapfromPath(TheMaterial.Metalness.Path);
            else
                ImgMetal.Source = BitmapToImageSource(TheMaterial.CreateFakeMap(size, TheMaterial.Metalness));

            if (File.Exists(TheMaterial.Alpha.Path))
                ImgAlpha.Source = GetBitmapfromPath(TheMaterial.Alpha.Path);
            else
                ImgAlpha.Source = BitmapToImageSource(TheMaterial.CreateFakeMap(size, TheMaterial.Alpha));

            if (File.Exists(TheMaterial.Emissive.Path))
                ImgEmissive.Source = GetBitmapfromPath(TheMaterial.Emissive.Path);
            else
                ImgEmissive.Source = BitmapToImageSource(TheMaterial.CreateFakeMap(size, TheMaterial.Emissive));

            if (File.Exists(TheMaterial.Paint.Path))
                ImgPaint.Source = GetBitmapfromPath(TheMaterial.Paint.Path);
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
            UpdateUIImages();
        }

        public void PerformImageDrop(DragEventArgs e,IMGTypes TheType)
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
            TheMaterial.Paint.State = ChannelState.off;
            UpdateUIImages();
        }
        private void Btn_SetPaintable_Click(object sender, RoutedEventArgs e)
        {
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
            TheMaterial.Emissive.State = ChannelState.off;
            UpdateUIImages();
        }

        private void Btn_SetEmissive_Click(object sender, RoutedEventArgs e)
        {
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
        }
    }
}
