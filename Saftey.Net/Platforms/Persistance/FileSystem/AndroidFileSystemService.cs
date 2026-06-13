#if ANDROID
using Android.Content;
using Android.Provider;

namespace WatchMe.Persistance.Implementations
{
    public class AndroidFileSystemService : BaseFileSystemService
    {
        //Todo, do in an agnostic way that isnt just working on android.

        public AndroidFileSystemService() { }
        public override async Task MoveVideoToGallery(MemoryStream memStream, string fileName)
        {

            var context = Platform.CurrentActivity;
            var resolver = context.ContentResolver;
            var contentValues = new ContentValues();
            contentValues.Put(MediaStore.IMediaColumns.DisplayName, fileName);
            contentValues.Put(MediaStore.Files.IFileColumns.MimeType, "video/mp4");
            contentValues.Put(MediaStore.IMediaColumns.RelativePath, "DCIM/WatchMeVideoCaptures");
            try
            {
                var videoUri = resolver.Insert(MediaStore.Video.Media.ExternalContentUri, contentValues);
                var output = resolver.OpenOutputStream(videoUri);
                output.Write(memStream.ToArray());
                output.Flush();
                output.Close();
                output.Dispose();
            }
            catch (Exception ex)
            {
                Console.Write(ex.ToString());

            }
        }
    }
}
#endif
