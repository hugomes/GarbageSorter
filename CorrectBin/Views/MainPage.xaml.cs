using CorrectBin.Model;
using CorrectBin.Service;
using Newtonsoft.Json;
using Plugin.Media;
using Plugin.Media.Abstractions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace CorrectBin.Views
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();

            cameraButton.Clicked += CameraButton_Clicked;

            CameraButton_Clicked(new object(), new EventArgs());

        }

        private async void CameraButton_Clicked(object sender, EventArgs e)
        {
            cameraButton.IsEnabled = false;

            info.Text = "Getting image.";

            EnableActivityIndicator(activityIndicator, true);

            await CrossMedia.Current.Initialize();

            if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
            {
                await DisplayAlert("No Camera", ":( No camera available.", "OK");
                return;
            }

            var file = await CrossMedia.Current.TakePhotoAsync(new StoreCameraMediaOptions
            {
                Directory = "Sample",
                Name = "test.jpg",
                PhotoSize = PhotoSize.Medium
            });

            if (file == null)
                return;

            string result = "";
            if (file != null)
            {
                using (var memoryStream = new MemoryStream())
                {
                    info.Text = "Analyzing image.";
                    file.GetStream().CopyTo(memoryStream);
                    var retorno = await AzureCognitiveService.MakeAnalysisRequest(memoryStream.ToArray());
                    result = CompareData(retorno);
                    File.Delete(file.Path);
                }
            }

            EnableActivityIndicator(activityIndicator, false);

            info.Text = "Put in "+result+" bin.";

            cameraButton.IsEnabled = true;
        }

        private void EnableActivityIndicator(ActivityIndicator activityIndicator, bool enabled)
        {
            activityIndicator.IsRunning = enabled;
            activityIndicator.IsEnabled = enabled;
            activityIndicator.IsVisible = enabled;
        }

        private string CompareData(ImageInfo imageInfo) {
            //DisplayAlert("Tags", string.Join(", ", imageInfo.Description.Tags), "OK");

            string retorno = "";

            List<string> compostList = LoadData("compost");
            List<string> landfillList = LoadData("landfill");
            List<string> recycleList = LoadData("recycle");

            int countCompost = 0;
            int countRecycle = 0;
            int countLandfill = 0;

            if (compostList != null)
                countCompost = imageInfo.Description.Tags.Where(c => compostList.Contains(c)).Count();
            if (recycleList != null)
                countRecycle = imageInfo.Description.Tags.Where(c => recycleList.Contains(c)).Count();
            if (landfillList != null)
                countLandfill = imageInfo.Description.Tags.Where(c => landfillList.Contains(c)).Count();

            if (countCompost > countRecycle)
                retorno = "Compost";
            else if (countRecycle > countCompost)
                retorno = "Recycle";
            else
                retorno = "Landfill";

            return retorno;

        }

        private List<string> LoadData(string fileName)
        {
            var assembly = IntrospectionExtensions.GetTypeInfo(typeof(MainPage)).Assembly;
            Stream stream = assembly.GetManifestResourceStream("CorrectBin.Data."+fileName+".json");
            string text = "";
            using (var reader = new System.IO.StreamReader(stream))
            {
                text = reader.ReadToEnd();
            }
            return JsonConvert.DeserializeObject<List<string>>(text);
        }
    }
}