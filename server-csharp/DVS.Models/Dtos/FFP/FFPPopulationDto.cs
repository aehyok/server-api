using DVS.Models.Dtos.Village;
using DVS.Models.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace DVS.Models.Dtos.FFP
{
    /// <summary>
    /// 防返贫住户详情
    /// </summary>
    public class FFPPopulationDto
    {

		/// <summary>
		/// 
		/// </summary>
		public int Id { get; set; }

		/// <summary>
		/// 行政代码Id
		/// </summary>
		public virtual int AreaId { get; set; }

		/// <summary>
		/// 户码Id
		/// </summary>
		public virtual int HouseholdId { get; set; }

		/// <summary>
		/// 人口Id
		/// </summary>
		public virtual int PopulationId { get; set; }
		/// <summary>
		/// 姓名
		/// </summary>
		public string RealName { get; set; }

		/// <summary>
		/// 性别 1男，2女
		/// </summary>
		public PopulationGender Sex { get; set; }

		/// <summary>
		/// 民族
		/// </summary>
		public string Nation { get; set; } = "";

		/// <summary>
		/// 出生年月日 1990-04-02
		/// </summary>
		public DateTime Birthday { get; set; }

		/// <summary>
		/// 与户主关系
		/// </summary>
		public string Relationship { get; set; } = "";

		/// <summary>
		/// 身份证号码
		/// </summary>
		public string IdCard { get; set; } = "";

		/// <summary>
		/// 身份证类别 1身份证，2残疾证
		/// </summary>
		public int IdCardType { get; set; } = 1;
		

		/// <summary>
		/// 联系方式
		/// </summary>
		public string Mobile { get; set; }

		/// <summary>
		/// 联系方式简式
		/// </summary>
		public string MobileShort { get; set; }

		/// <summary>
		/// 政治面貌
		/// </summary>
		public string Political { get; set; }

		/// <summary>
		/// 学历
		/// </summary>
		public string Education { get; set; }

		/// <summary>
		/// 婚姻状态
		/// </summary>
		public string Marital { get; set; }

		/// <summary>
		/// 宗教
		/// </summary>
		public string Religion { get; set; }


		/// <summary>
		/// 收入来源
		/// </summary>
		public string Income { get; set; }

		/// <summary>
		/// 头像Id
		/// </summary>
		public string HeadImageId { get; set; }

		/// <summary>
		/// 头像图片路径
		/// </summary>
		public string HeadImageUrl { get; set; }

		/// <summary>
		/// 标签数组
		/// </summary>
		public string Tags { get; set; }

		/// <summary>
		/// 健康状况
		/// </summary>
		public string Health { get; set; }

		/// <summary>
		/// 在校生状况
		/// </summary>
		public string StudentStatus { get; set; }

		/// <summary>
		/// 劳动技能
		/// </summary>
		public string LaborSkill { get; set; }

		/// <summary>
		/// 劳工时间
		/// </summary>
		public double WorkingTimes { get; set; }

		/// <summary>
		/// 是否参与城乡居民（职工）基本医疗保障
		/// </summary>
		public string MedicalInsuranceStatus { get; set; }

		/// <summary>
		/// 是否参加大病保险
		/// </summary>
		public string SeriousIllnessInsuranceStatus { get; set; }

		/// <summary>
		/// 是否参加城乡居民（职工）基本养老保险
		/// </summary>
		public string EndowmentInsuranceStatus { get; set; }

		/// <summary>
		/// 是否享受城乡居民最低生活保障
		/// </summary>
		public string MinLivingSecurityStatus { get; set; }


		/// <summary>
		/// 是否特困供养人员
		/// </summary>
		public string PoorStatus { get; set; }

		/// <summary>
		/// 是否易地扶贫搬迁（同步搬迁）人口
		/// </summary>
		public string MovePeopleStatus { get; set; }

		/// <summary>
		/// 状态 1正常，0禁用
		/// </summary>
		public int Status { get; set; } = 0;

		/// <summary>
		/// 备注
		/// </summary>
		public string Remark { get; set; }


		/// <summary>
		/// 
		/// </summary>
		public ModuleDictionaryDto EducationDto { get; set; }

		/// <summary>
		/// 
		/// </summary>
		public ModuleDictionaryDto IncomeDto { get; set; }

		/// <summary>
		/// 
		/// </summary>
		public ModuleDictionaryDto HealthDto { get; set; }
		/// <summary>
		/// 
		/// </summary>

		public ModuleDictionaryDto LaborSkillDto { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public ModuleDictionaryDto MaritalDto { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public ModuleDictionaryDto MedicalInsuranceStatusDto { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public ModuleDictionaryDto MinLivingSecurityStatusDto { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public ModuleDictionaryDto MovePeopleStatusDto { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public ModuleDictionaryDto NationDto { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public ModuleDictionaryDto PoliticalDto { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public ModuleDictionaryDto PoorStatusDto { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public ModuleDictionaryDto RelationshipDto { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public ModuleDictionaryDto ReligionDto { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public ModuleDictionaryDto StudentStatusDto { get; set; }


		/// <summary>
		/// 务工地区
		/// </summary>
		public PopulationAddressDto WorkingAreaAddressInfo { get; set; }
	}
}
