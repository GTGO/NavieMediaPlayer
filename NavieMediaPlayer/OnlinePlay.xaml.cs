using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.Storage.Pickers;
using System.Threading.Tasks;
using Windows.Media;
using Windows.Storage;
using Windows.Web.Http;
using Windows.Storage.Streams;
using Windows.Media.Core;


// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace NavieMediaPlayer
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class OnlinePlay : Page
    {
        public OnlinePlay()
        {
            this.InitializeComponent();
        }

        private void OnlinePlayButton_Click(object sender, RoutedEventArgs e)
        {
            Uri uri = new Uri(MyBox.Text);
            MyMediaPlayer.Source = uri;
        }

        private async void DownLoadButton_Click(object sender, RoutedEventArgs e)
        {
            Uri uri = new Uri(MyBox.Text);
            MyMediaPlayer.Source = uri;
            string FileName = System.IO.Path.GetFileName(uri.LocalPath);

            
            var httpClient = new HttpClient();
            var httpResponse = new Windows.Web.Http.HttpResponseMessage();
            var file = await KnownFolders.MusicLibrary.CreateFileAsync(FileName, CreationCollisionOption.ReplaceExisting);
            string httpResponseBody = "";
            try
            {
                httpResponse = await httpClient.GetAsync(uri);
                httpResponse.EnsureSuccessStatusCode();
                httpResponseBody = await httpResponse.Content.ReadAsStringAsync();
                var sourceStream = await httpResponse.Content.ReadAsInputStreamAsync();
                using (var destinationStream = await file.OpenAsync(FileAccessMode.ReadWrite))
                {
                    using (var destinationOutputStream = destinationStream.GetOutputStreamAt(0))
                    {
                        await RandomAccessStream.CopyAndCloseAsync(sourceStream, destinationStream);
                    }
                }
                var stream = await file.OpenAsync(Windows.Storage.FileAccessMode.Read);
                MyMediaPlayer.SetSource(stream, file.ContentType);
                MyMediaPlayer.Play();

            }
            catch (Exception ex)
            {
                httpResponseBody = "Error: " + ex.HResult.ToString("X") + " Message: " + ex.Message;
            }

        }
    }
}
