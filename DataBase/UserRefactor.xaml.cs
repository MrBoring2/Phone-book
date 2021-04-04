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
using Microsoft.Win32;
using System.Data;

namespace DataBase
{
    /// <summary>
    /// Логика взаимодействия для UserRefactor.xaml
    /// </summary>
    public partial class UserRefactor : Window
    {
        int id;
        string name, familyName, otchestvo, telephone, category;
        byte[] photoImage;
        ImageSource standartSource;
        OpenFileDialog openFileDialog = new OpenFileDialog();
        public string getName { get { return name; } }
        public string getFamilyName { get { return familyName; } }
        public string getOthcestvo { get { return otchestvo; } }
        public string getTelephone { get { return telephone; } }
        public string getCategory { get { return category; } }

        public byte[] getPhoto { get { return photoImage; } }

        public int getId { get => id; }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            openFileDialog.Filter = "Image files (*.PNG, *.JPG)|*.jpg;*.png";
            if (openFileDialog.ShowDialog() == true)
            {

                ImageSource img;
                using (var stream = new FileStream(openFileDialog.FileName, FileMode.Open))
                {
                    img = BitmapFrame.Create(stream, BitmapCreateOptions.None, BitmapCacheOption.OnLoad);
                    photo.Source = img;
                }

            }
         
        }

        public UserRefactor(int id, string name, string familyName, string otchestvo, string telephone, string category, byte[] photoImage)
        {
            InitializeComponent();
   
            this.id = id;
            this.name = name;
            this.familyName = familyName;
            this.otchestvo = otchestvo;
            this.telephone = telephone;
            this.category = category;
            this.photoImage = photoImage;

            nameText.Text = name;
            familyNameText.Text = familyName;
            otchestvoName.Text = otchestvo;
            telephoneText.Text = telephone;
            
            foreach (ComboBoxItem item in categotyText.Items)
            {
                if (item.Content.Equals(category))
                {
                    categotyText.SelectedItem = item;
                }
            }

            if (photoImage != null)
            {
                MemoryStream ms = new MemoryStream(photoImage);
                photo.Source = BitmapFrame.Create(ms, BitmapCreateOptions.None, BitmapCacheOption.OnLoad);
            }
            standartSource = photo.Source;

        }

        private void save_Click(object sender, RoutedEventArgs e)
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

        private void cancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;

        }
    }
}
