using DVS.Core.Domains.Village;
using System;
using System.Collections.Generic;
using System.Text;
using FluentValidation;

namespace DVS.Application.Validators
{
    public class FarmlandValidator : ValidatorBase<VillageFarmland>
    {
        public FarmlandValidator() {
            RuleFor(land => land.TypeId).GreaterThan(0).WithMessage("请选择地块类型");
            RuleFor(land => land.Name).NotEmpty().WithMessage("请填写名称");
        }
    }
}
