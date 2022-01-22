namespace DVS.Models.Dtos.Village.Statistics
{
    public class StatisticsPopulationTypeDto
    {
        public long populationType { get; set; } = 0;

        /// <summary>
        /// 人口类型
        /// </summary>
        public string Name { get; set; } = "";

        /// <summary>
        /// 男数量
        /// </summary>
        public long MaleValue { get; set; }

        /// <summary>
        /// 女数量
        /// </summary>
        public long FemaleVaule { get; set; }
    }
}