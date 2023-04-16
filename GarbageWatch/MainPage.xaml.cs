using GarbageWatch.Services;

namespace GarbageWatch;

public partial class MainPage : ContentPage
{
    public string filePath;

    public MainPage()
    {
        InitializeComponent();
    }

    /// <summary>
    ///     Captures picture to extract text from
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
	private async void CaptureImageButton_Clicked(object sender, EventArgs e)
    {
        if (MediaPicker.Default.IsCaptureSupported)
        {
            // Capture photo using the device's Camera app
            var photo = await MediaPicker.CapturePhotoAsync();

            // Save photo locally as temporary data if captured successfully
            if (photo != null)
            {
                string localFilePath = Path.Combine(FileSystem.CacheDirectory, photo.FileName);

                using Stream sourceStream = await photo.OpenReadAsync();
                using FileStream localFileStream = File.OpenWrite(localFilePath);

                await sourceStream.CopyToAsync(localFileStream);

                CapturedImage.Source = ImageSource.FromFile(localFilePath);
                filePath = localFilePath;
            }
        }
        else
        {
            await DisplayAlert("MediaPicker", "Not Supported", "Ok");
        }
    }

    private async void DetectGarbageButton_Clicked(object sender, EventArgs e)
    {
        if(String.IsNullOrEmpty(filePath))
        {
            await DisplayAlert("No Image Found", "Please click a picture first", "Ok");
            return;
        }

        GarbageDetectionService garbageDetectionService = new GarbageDetectionService(filePath);

        List<string> detectedItems = await garbageDetectionService.DetectGarbage();
        
        string items = "";
        foreach (var tag in detectedItems)
        {
            items += $"{tag}\n";
        }

        if (detectedItems.Contains("trash") || detectedItems.Contains("garbage"))
        {
            await DisplayAlert("Garbage Detected", $"List of items detected:\n{items}\n\nShare this picture with the concerned municipal authorities", "Ok");

            var location = await LocationService.GetCurrentLocation();

            await Share.Default.RequestAsync(new ShareFileRequest
            {
                Title = $"Garbage detected publicly at coordinates ({location.Latitude},{location.Longitude})\n\nPlease check and have it cleaned up <Tag Local Municipal Authority>",
                File = new ShareFile(filePath),
                
            });

        }
        else
        {
            await DisplayAlert("Garbage Not Detected", $"List of items detected instead:\n\n{items}", "Ok");
        }
    }
}

