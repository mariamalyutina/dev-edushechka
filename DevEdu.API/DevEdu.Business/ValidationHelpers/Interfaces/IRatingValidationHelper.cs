﻿using DevEdu.DAL.Models;

namespace DevEdu.Business.ValidationHelpers
{
    public interface IRatingValidationHelper
    {
        StudentRatingDto CheckRaitingExistence(int raitingId);
    }
}