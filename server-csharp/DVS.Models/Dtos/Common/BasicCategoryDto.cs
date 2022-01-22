using System;
using System.Collections.Generic;
using System.Text;

namespace DVS.Models.Dtos.Common
{
    public class BasicCategoryDto
    {
        public int Id { get; set; }

        public string CategoryName { get; set; }

        /// <summary>
        /// 类目详细描述
        /// </summary>
        public string CategoryDetail { get; set; }

        /// <summary>
        /// 类目图片文件id
        /// </summary>
        public int CategoryPicId { get; set; }

        /// <summary>
        /// 类目图片相对地址
        /// </summary>
        public string CategoryPicUrl { get; set; }

        public List<SunFileInfoDto> ImageFiles { get; set; }

    }

    public class BasicCategoryModel
    {
        public int Id { get; set; }
        /// <summary>
        /// 上级类目id
        /// </summary>
        public int ParentId { get; set; }

        /// <summary>
        /// 类目名称
        /// </summary>
        public string CategoryName { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        public int Sequence { get; set; }

        /// <summary>
        /// 本级及所有下级的id
        /// </summary>
        public List<int> Ids { get; set; } = new List<int>();
        public List<BasicCategoryModel> Children { get; set; }
    }
}
