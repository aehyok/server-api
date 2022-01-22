using Lychee.Music.Abstractions;
using System;

namespace Lychee.Music.Wangyi
{
    public class WangyiBoardService : IBoardService
    {
        private readonly MusicBoard[] boards = new MusicBoard[]
        {
              new MusicBoard(  "wy__19723756", boardId: "19723756", name: "云音乐飙升榜"),
              new MusicBoard(  "wy__3778678", boardId: "3778678", name: "云音乐热歌榜"),
              new MusicBoard(  "wy__3779629", boardId: "3779629", name: "云音乐新歌榜"),
              new MusicBoard(  "wy__2884035", boardId: "2884035", name: "云音乐原创榜"),
              new MusicBoard(  "wy__2250011882", boardId: "2250011882", name: "抖音排行榜"),
              new MusicBoard(  "wy__1978921795", boardId: "1978921795", name: "云音乐电音榜"),
              new MusicBoard(  "wy__4395559", boardId: "4395559", name: "华语金曲榜"),
              new MusicBoard(  "wy__71384707", boardId: "71384707", name: "云音乐古典音乐榜"),
              new MusicBoard(  "wy__10520166", boardId: "10520166", name: "云音乐国电榜"),
              new MusicBoard(  "wy__2006508653", boardId: "2006508653", name: "电竞音乐榜"),
              new MusicBoard(  "wy__991319590", boardId: "991319590", name: "云音乐说唱榜"),
              new MusicBoard(  "wy__180106", boardId: "180106", name: "UK排行榜周榜"),
              new MusicBoard(  "wy__60198", boardId: "60198", name: "美国Billboard周榜"),
              new MusicBoard(  "21845217", boardId: "21845217", name: "KTV嗨榜"),
              new MusicBoard(  "wy__11641012", boardId: "11641012", name: "iTunes榜"),
              new MusicBoard(  "wy__120001", boardId: "120001", name: "Hit FM Top榜"),
              new MusicBoard(  "wy__60131", boardId: "60131", name: "日本Oricon周榜"),
              new MusicBoard(  "wy__3733003", boardId: "3733003", name: "韩国Melon排行榜周榜"),
              new MusicBoard(  "wy__60255", boardId: "60255", name: "韩国Mnet排行榜周榜"),
              new MusicBoard(  "wy__46772709", boardId: "46772709", name: "韩国Melon原声周榜"),
              new MusicBoard(  "wy__64016", boardId: "64016", name: "中国TOP排行榜(内地榜)"),
              new MusicBoard(  "wy__112504", boardId: "112504", name: "中国TOP排行榜(港台榜)"),
              new MusicBoard(  "wy__3112516681", boardId: "3112516681", name: "中国新乡村音乐排行榜"),
              new MusicBoard(  "wy__10169002", boardId: "10169002", name: "香港电台中文歌曲龙虎榜"),
              new MusicBoard(  "wy__27135204", boardId: "27135204", name: "法国 NRJ EuroHot 30周榜"),
              new MusicBoard(  "wy__1899724", boardId: "1899724", name: "中国嘻哈榜"),
              new MusicBoard(  "wy__112463", boardId: "112463", name: "台湾Hito排行榜"),
              new MusicBoard(  "wy__3812895", boardId: "3812895", name: "Beatport全球电子舞曲榜"),
              new MusicBoard(  "wy__2617766278", boardId: "2617766278", name: "新声榜"),
              new MusicBoard(  "wy__745956260", boardId: "745956260", name: "云音乐韩语榜"),
              new MusicBoard(  "wy__2847251561", boardId: "2847251561", name: "说唱TOP榜"),
              new MusicBoard(  "wy__2023401535", boardId: "2023401535", name: "英国Q杂志中文版周榜"),
              new MusicBoard(  "wy__2809513713", boardId: "2809513713", name: "云音乐欧美热歌榜"),
              new MusicBoard(  "wy__2809577409", boardId: "2809577409", name: "云音乐欧美新歌榜"),
              new MusicBoard(  "wy__71385702", boardId: "71385702", name: "云音乐ACG音乐榜"),
              new MusicBoard(  "wy__3001835560", boardId: "3001835560", name: "云音乐ACG动画榜"),
              new MusicBoard(  "wy__3001795926", boardId: "3001795926", name: "云音乐ACG游戏榜"),
              new MusicBoard(  "wy__3001890046", boardId: "3001890046", name: "云音乐ACG VOCALOID榜")
        };

        public void Filter()
        {
            throw new NotImplementedException();
        }

        public void Get()
        {
            throw new NotImplementedException();
        }

        public MusicBoard[] GetAll()
        {
            return this.boards;
        }

        public void GetSinger()
        {
            throw new NotImplementedException();
        }
    }
}