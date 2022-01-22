using DVS.Core.Domains.Village;
using FluentValidation;

namespace DVS.Application.Validators
{
    public class VillagePopulationValidator : ValidatorBase<VillagePopulation>
    {
        public VillagePopulationValidator()
        {
            RuleFor(a => a.IdCard).NotEmpty().WithMessage("请输入身份证号码");
            RuleFor(a => a.RealName).NotEmpty().WithMessage("请输入姓名");
            //RuleFor(a => a.RegisterOrgCodes).NotEmpty().WithMessage("请选择户籍地");
            //RuleFor(a => a.RegisterAddress).NotEmpty().WithMessage("请输入户籍地详细地址");
            //RuleFor(a => a.LiveOrgCodes).NotEmpty().WithMessage("请选择居住地");
            //RuleFor(a => a.LiveAddress).NotEmpty().WithMessage("请输入居住地详细地址");
            RuleFor(a => a.Income).NotEmpty().WithMessage("请选择主要生活来源");
            RuleFor(a => a.Tags).NotEmpty().WithMessage("请选择人口属性");
            RuleFor(a => a.Birthday).NotEmpty().WithMessage("请选择出生日期");
            RuleFor(a => a.Mobile).NotEmpty().WithMessage("请输入联系方式");
            // RuleFor(a => a.NativePlaceOrgCodes).NotEmpty().WithMessage("请选择籍贯");
            RuleFor(a => a.Marital).NotEmpty().WithMessage("请选择婚姻状态");
            RuleFor(a => a.Political).NotEmpty().WithMessage("请选择政治面貌");
            RuleFor(a => a.Education).NotEmpty().WithMessage("请选择学历");
        }
    }
}