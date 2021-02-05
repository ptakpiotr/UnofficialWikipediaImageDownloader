using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using WpfApp1.Model;

namespace WpfApp1
{
    /// <summary>
    /// Logika interakcji dla klasy MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        List<ImageClass> Images = new List<ImageClass>();
        public Color bgColor = Colors.White;
        public MainWindow()
        {
            InitializeComponent();
            if (DateTime.Now.Hour > 20)
            {
                bgColor = Colors.Black;
            }
            MainGrid.Background = new SolidColorBrush(bgColor);

        }
        private void DisplayData(IHtmlDocument d)
        {
            WebClient wb = new WebClient();

            IEnumerable<IHtmlImageElement> images = d.Images.Distinct();

            int k = 1;
            if (Path.IsPathRooted(Destination.Text))
            {
                using (StreamWriter sw = new StreamWriter($"{Destination.Text}\\sources.txt"))
                {
                    try
                    {
                        foreach (var i in images)
                        {
                            string ext = i.Source.Substring(i.Source.LastIndexOf("."));
                            sw.WriteLine($"image{k}{ext}->{i.Source}");
                            if (ext.Length == 4 || ext.Length == 5)
                            {
                                wb.DownloadFile($"{i.Source.Replace("about", "https")}", $"{Destination.Text}\\image{k}{ext}");
                            }
                            k++;
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }
                }
                using (StreamReader sr = new StreamReader($"{Destination.Text}\\sources.txt"))
                {
                    while (sr.ReadLine() != null)
                    {
                        string str = sr.ReadLine();

                        if (str != null && str.Contains("upload"))
                        {

                            Images.Add(new ImageClass { ImageSource = new BitmapImage(new Uri($"{ str.Substring(str.IndexOf(":")).Replace("://", "https://")}")) });
                        }
                    }
                    DownloadedC.ItemsSource = Images.Take(5);
                }
            }
            else
            {
                MessageBox.Show("Please provide the correct path!");
            }
        }
        async private void RequestWeb(string url)
        {
            using (HttpClient client = new HttpClient())
            {
                using (HttpResponseMessage res = await client.GetAsync(url))
                {
                    using (var content = res.Content)
                    {
                        HtmlParser parser = new HtmlParser();

                        var data = await content.ReadAsStreamAsync();

                        IHtmlDocument docu = parser.ParseDocument(data);
                        DisplayData(docu);
                    }
                }
            }
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string src = Page.Text.Contains("wikipedia.org") ? Page.Text : "https://pl.wikipedia.org/wiki/Fiat_Seicento";
            RequestWeb(src);
        }

        private void MenuItemAbout_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show($"Project created by Piotr Ptak. Only for education purposes.\nCommercial use not allowed.\nI do not own any of the images.");
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Scroll.Visibility = Visibility.Hidden;
        }

        private void MenuItemTheme_Click(object sender, RoutedEventArgs e)
        {
            bgColor = bgColor==Colors.White?Colors.Black:Colors.White;
            MainGrid.Background = new SolidColorBrush(bgColor);
        }
    }
}
