﻿using System.Collections.Generic;

namespace DevEdu.Business.ValidationHelpers
{
    public interface IGroupValidationHelper
    {
        void CheckGroupExistence(int groupId);
    }
}