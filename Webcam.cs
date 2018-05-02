using System;
using System.Threading.Tasks;

using Windows.Media.Capture;
using Windows.Media.MediaProperties;
using Windows.Storage;

namespace SmartDevice
{
    public static class Webcam
    {
        private static MediaCapture mediaCapture;

        private static StorageFolder localFolder = ApplicationData.Current.LocalFolder;

        public static async Task<StorageFile> TakePhoto()
        {
            // initialize webcam ready for use
            mediaCapture = new MediaCapture();
            await mediaCapture.InitializeAsync();

            // create a new photo file in the device's Pictures directory
            StorageFile photoFile = await localFolder.CreateFileAsync("photo.jpg", CreationCollisionOption.GenerateUniqueName);
            // set the file type as a JPEG
            ImageEncodingProperties imageProperties = ImageEncodingProperties.CreateJpeg();

            // capture and store the JPEG photo from the webcam
            await mediaCapture.CapturePhotoToStorageFileAsync(imageProperties, photoFile);

            return photoFile;
        }
    }
}
