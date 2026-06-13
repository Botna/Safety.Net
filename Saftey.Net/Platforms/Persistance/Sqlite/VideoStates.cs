namespace WatchMe.Persistance.Sqlite
{
    public enum VideoStates
    {
        Recording, //represents the video being recorded, and more bytes will be available for upload eventually.
        Finished // represents the video is done being recorded and can be deleted after upload.
    }
}
