﻿using DevEdu.DAL.Enums;
using System;

namespace DevEdu.DAL.Models
{
    public class StudentHomeworkDto
    {
        public int Id { get; set; }
        public HomeworkDto Homework { get; set; }
        public UserDto User { get; set; }
        public TaskStatus TaskStatus { get; set; }
        public string Answer { get; set; }
        public DateTime? CompletedDate { get; set; }
        public bool IsDeleted { get; set; }
    }
}