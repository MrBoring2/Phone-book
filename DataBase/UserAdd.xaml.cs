using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.IO;

namespace DataBase
{
    /// <summary>
    /// Логика взаимодействия для UserAdd.xaml
    /// </summary>
    public partial class UserAdd : Window
    {
        OpenFileDialog openFileDialog = new OpenFileDialog();
        ImageSource standartSource;
        string name, familyName, otchestvo, telephone, category;
        byte[] photoImage;

        public string getName { get { return name; } }
        public string getFamilyName { get { return familyName; } }
        public string getOthcestvo { get { return otchestvo; } }
        public string getCategory { get { return category; } }
        public byte[] getPhotoImage { get { return photoImage; } }
        public string getTelephone { get { return telephone; } }

      

        public UserAdd()
        {
            InitializeComponent();
            categotyText.SelectedItem = categotyText.Items[0];
            standartSource = photo.Source;
            var image = photo.Source as BitmapSource;
            byte[] data;
            BitmapEncoder encoder = new JpegBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(image));
            using (MemoryStream ms = new MemoryStream())
            {
                encoder.Save(ms);
                data = ms.ToArray();
            }
            photoImage = data;


        }
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            openFileDialog.Filter = "Image files (*.PNG, *.JPG)|*.jpg;*.png";
            if (openFileDialog.ShowDialog() == true)
            {
                
                ImageSource img;
                using(var stream = new FileStream(openFileDialog.FileName, FileMode.Open))
                {
                    img = BitmapFrame.Create(stream, BitmapCreateOptions.None, BitmapCacheOption.OnLoad);
                    photo.Source = img;
                }
                
            }
        }
        
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            
            if (photo.Source == standartSource)
            {
                var filePath = "../../Resources/standartPhoto.png";
                name = nameText.Text;
                familyName = familyNameText.Text;
                otchestvo = otchestvoName.Text;
                telephone = telephoneText.Text;
                category = categotyText.Text;
                using (var stream = new FileStream(filePath, FileMode.Open))
                {
                    photoImage = new byte[stream.Length];
                    stream.Read(photoImage, 0, photoImage.Length);
                }
                this.DialogResult = true;
                //BitmapImage img = new BitmapImage(new Uti("Resources/")) 
                //using (var stream = new FileStream(filePath, FileMode.Open))
                //{
                //    var filePath = Resources;

                //    photoImage = new byte[stream.Length];
                //    stream.Read(photoImage, 0, photoImage.Length);
                //}
            }
            else
            {
                var filePath = openFileDialog.FileName;
                var fileName = System.IO.Path.GetFileName(filePath);
                name = nameText.Text;
                familyName = familyNameText.Text;
                otchestvo = otchestvoName.Text;
                telephone = telephoneText.Text;
                category = categotyText.Text; 
                using (var stream = new FileStream(filePath, FileMode.Open))
                {
                    photoImage = new byte[stream.Length];
                    stream.Read(photoImage, 0, photoImage.Length);
                }
                this.DialogResult = true;
            }
        }
    }
}
