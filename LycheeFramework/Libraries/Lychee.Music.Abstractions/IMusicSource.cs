namespace Lychee.Music.Abstractions
{
    public interface IMusicSource
    {
        void SongList();

        void Search();

        void Board();

        void HotSearch();

        void GetMusicUrl();

        void GetLyrci();

        void GetPicture();
    }
}