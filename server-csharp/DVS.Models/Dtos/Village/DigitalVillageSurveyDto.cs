using System;

namespace DVS.Models.Dtos.Village
{
    public class DigitalVillageSurveyDto
    {
        /// <summary>
        /// id
        /// </summary>
        public Int64 Id { get; set; } = 0;

        /// <summary>
        /// 区域id
        /// </summary>
        public Int64 VillageId { get; set; } = 0;

        /// <summary>
        /// 区域编码
        /// </summary>
        public Int64 AreaCode { get; set; }
        
        /// <summary>
        /// 数据年份
        /// </summary>
        public int DataYear { get; set; }

        /// <summary>
        /// 人口数
        /// </summary>
        public int Population { get; set; }

        /// <summary>
        /// 户数
        /// </summary>
        public int Tenement { get; set; }

        /// <summary>
        /// 村面积
        /// </summary>
        public decimal Area { get; set; }
    }
}