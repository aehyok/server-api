namespace Lychee.Music.Abstractions
{
    public class MusicBoard
    {
        public MusicBoard(string id, string name, string boardId)
        {
            this.Id = id;
            this.Name = name;
            this.BoardId = boardId;
        }

        public string Id { get; set; }

        public string Name { get; set; }

        public string BoardId { get; set; }
    }
}