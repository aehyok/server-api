using AutoMapper;
using DVS.Common.Services;
using DVS.Core.Domains.Common;
using DVS.Models.Dtos.Common;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DVS.Application.Services.Common
{
    public class BasicCategoryService : ServiceBase<BasicCategory>, IBasicCategoryService
    {
        readonly ISunFileInfoService fileService;
        public BasicCategoryService(DbContext dbContext, IMapper mapper, ISunFileInfoService fileService)
            : base(dbContext, mapper)
        {
            this.fileService = fileService;
        }

		public async Task<BasicCategoryDto> GetBasicCategory(int id)
		{
            var category = await this.GetAsync(a => a.Id == id);
            if (category != null) {
                var categoryInfo = mapper.Map<BasicCategoryDto>(category);
                var picid = category.CategoryPicId;
                var imagelist = await this.fileService.GetSunFileInfoList(picid.ToString());
                categoryInfo.ImageFiles = imagelist;
                return categoryInfo;
            }

            return null;
        }

		public async Task<List<BasicCategoryDto>> GetBasicCategoryList(List<int> ids)
		{
            var list = await this.GetListAsync(a => ids.Contains(a.Id));
            if (list != null)
            {
                var categoryInfolist = mapper.Map<List<BasicCategoryDto>>(list);
                List<string> imageIds = new List<string>();

                foreach (var item in categoryInfolist) {
                    imageIds.Add(item.CategoryPicId.ToString());
                }

                var files = await this.fileService.GetSunFileInfoList(string.Join(",", imageIds));

                foreach (var category in categoryInfolist) {
                    var file = files.FindAll(a => a.Id == category.CategoryPicId);
                    category.ImageFiles = file;
                }
                return categoryInfolist;
            }

            return null;
        }
	}
}
