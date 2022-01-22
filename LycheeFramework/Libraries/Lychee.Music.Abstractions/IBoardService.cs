namespace Lychee.Music.Abstractions
{
    public interface IBoardService
    {
        void Get();

        MusicBoard[] GetAll();

        void GetSinger();

        void Filter();
    }
}