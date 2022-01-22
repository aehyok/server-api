using Lychee.Music.Abstractions;
using System;

namespace Lychee.Music.Xiami
{
    public partial class XiamiBoardService : IBoardService
    {
        private readonly MusicBoard[] Boards = new[]
        {
            new MusicBoard( "xm__102",  "新歌榜",  "102"),
            new MusicBoard( "xm__103",  "热歌榜",  "103"),
            new MusicBoard( "xm__104",  "原创榜",  "104"),
            new MusicBoard( "xm__306",  "K歌榜",  "306"),
            new MusicBoard( "xm__332",  "抖音热歌榜",  "332"),
            new MusicBoard( "xm__305",  "歌单收录榜",  "305"),
            new MusicBoard( "xm__327",  "趴间热歌榜",  "327"),
            new MusicBoard( "xm__324",  "影视原声榜",  "324"),
            new MusicBoard( "xm__204",  "美国Billboard单曲榜",  "204"),
            new MusicBoard( "xm__206",  "韩国MNET音乐排行榜",  "206"),
            new MusicBoard( "xm__201",  "Hito 中文排行榜",  "201"),
            new MusicBoard( "xm__203",  "英国UK单曲榜",  "203"),
            new MusicBoard( "xm__205",  "oricon公信单曲榜",  "205"),
            new MusicBoard( "xm__328",  "美国iTunes榜",  "328"),
            new MusicBoard( "xm__329",  "Beatport电音榜",  "329"),
            new MusicBoard( "xm__330",  "香港商业电台榜",  "330")
        };

        public void Filter()
        {
            throw new NotImplementedException();
        }

        public void Get()
        {
            throw new NotImplementedException();
        }

        public virtual MusicBoard[] GetAll()
        {
            return this.Boards;
        }

        public void GetSinger()
        {
            throw new NotImplementedException();
        }
    }
}