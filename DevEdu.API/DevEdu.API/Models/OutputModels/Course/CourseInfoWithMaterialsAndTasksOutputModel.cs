﻿using System.ComponentModel;

namespace DevEdu.API.Models.OutputModels
{
    public class CourseInfoWithMaterialsAndTasksOutputModel : CourseSimpleInfoOutputModel
    {
        //public List<MaterialDto> Materials { get; set; }
        //public List<TaskDto> Tasks { get; set; }
        [DefaultValue(false)]
        public bool IsDeleted { get; set; }
    }
}