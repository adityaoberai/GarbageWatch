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
        await DisplayAlert("Button Pressed", "Works", "Ok");
    }
}

