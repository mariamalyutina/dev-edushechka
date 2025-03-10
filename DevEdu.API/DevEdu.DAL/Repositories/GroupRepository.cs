﻿using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using DevEdu.Core;
using DevEdu.DAL.Enums;
using DevEdu.DAL.Models;
using Microsoft.Extensions.Options;

namespace DevEdu.DAL.Repositories
{
    public class GroupRepository : BaseRepository, IGroupRepository
    {
        private const string _groupInsertProcedure = "dbo.Group_Insert";
        private const string _groupDeleteProcedure = "dbo.Group_Delete";
        private const string _groupSelectByIdProcedure = "dbo.Group_SelectById";
        private const string _groupSelectAllProcedure = "dbo.Group_SelectAll";
        private const string _groupUpdateByIdProcedure = "dbo.Group_UpdateById";
        private const string _groupUpdateGroupStatusProcedure = "dbo.Group_UpdateGroupStatus";
        private const string _groupSelectAllByMaterialIdProcedure = "dbo.Group_SelectByMaterialId";
        private const string _groupSelectByCourseProcedure = "dbo.Group_SelectByCourseId";
        private const string _groupSelectAllByTaskIdProcedure = "dbo.Group_SelectAllByTaskId";
        private const string _groupSelectGroupsByUserIdProcedure = "dbo.Group_SelectAllByUserId";
        private const string _groupSelectGroupsByLessonIdProcedure = "dbo.Group_SelectAllByLessonId";
        private const string _groupSelectPresentGroupForStudentByUserIdProcedure = "dbo.Group_SelectPresentGroupForStudentByUserId";

        private const string _groupLessonInsertProcedure = "dbo.Group_Lesson_Insert";
        private const string _groupLessonDeleteProcedure = "dbo.Group_Lesson_Delete";

        private const string _groupMaterialInsertProcedure = "dbo.Group_Material_Insert";
        private const string _groupMaterialDeleteProcedure = "dbo.Group_Material_Delete";

        private const string _groupTaskDeleteProcedure = "dbo.Group_Task_Delete";

        private const string _userGroupInsertProcedure = "dbo.User_Group_Insert";
        private const string _userGroupDeleteProcedure = "dbo.User_Group_Delete";

        public GroupRepository(IOptions<DatabaseSettings> options) : base(options) { }

        public async Task<int> AddGroup(GroupDto groupDto)
        {
            return await _connection.QuerySingleAsync<int>
            (
                _groupInsertProcedure,
                new
                {
                    groupDto.Name,
                    CourseId = groupDto.Course.Id,
                    groupDto.StartDate,
                    groupDto.Timetable,
                    groupDto.PaymentPerMonth,
                },
                commandType: CommandType.StoredProcedure
            );
        }

        public async Task DeleteGroup(int id)
        {
            await _connection.ExecuteAsync
            (
                _groupDeleteProcedure,
                new { Id = id },
                commandType: CommandType.StoredProcedure
            );
        }

        public async Task<GroupDto> GetGroup(int id)
        {
            var answer = await _connection
            .QueryAsync<GroupDto, CourseDto, GroupDto>
            (
                _groupSelectByIdProcedure,
                (group, course) =>
                {
                    GroupDto dto = group;
                    group.Course = course;
                    group.Students = new List<UserDto>();

                    return dto;
                },
                new { id },
                splitOn: "Id",
                commandType: CommandType.StoredProcedure
            );
            return answer.FirstOrDefault();
        }

        public async Task<List<GroupDto>> GetGroups()
        {
            return (List<GroupDto>)await _connection
            .QueryAsync<GroupDto, CourseDto, GroupDto>
            (
                _groupSelectAllProcedure,
                (group, course) =>
                {
                    GroupDto groupDto;
                    groupDto = group;
                    groupDto.Course = course;

                    return groupDto;
                },
                splitOn: "Id",
                commandType: CommandType.StoredProcedure
            );
        }

        public async Task<GroupDto> UpdateGroup(GroupDto groupDto)
        {
            return await _connection
            .QuerySingleAsync<GroupDto>
            (
                _groupUpdateByIdProcedure,
                new
                {
                    groupDto.Id,
                    groupDto.Name,
                    CourseId = groupDto.Course.Id,
                    GroupStatusId = (int)groupDto.GroupStatus,
                    groupDto.StartDate,
                    groupDto.Timetable,
                    groupDto.PaymentPerMonth
                },
                commandType: CommandType.StoredProcedure
            );
        }

        public async Task<GroupDto> ChangeGroupStatus(int groupId, int statusId)
        {
            return await _connection
            .QuerySingleAsync<GroupDto>
            (
                _groupUpdateGroupStatusProcedure,
                new
                {
                    groupId,
                    statusId
                },
                commandType: CommandType.StoredProcedure
            );
        }

        public async Task<int> AddGroupToLesson(int groupId, int lessonId)
        {
            return await _connection.ExecuteAsync
            (
                _groupLessonInsertProcedure,
                new
                {
                    groupId,
                    lessonId
                },
                commandType: CommandType.StoredProcedure
            );
        }

        public async Task RemoveGroupFromLesson(int groupId, int lessonId)
        {
            await _connection.ExecuteAsync
            (
                 _groupLessonDeleteProcedure,
                 new
                 {
                     groupId,
                     lessonId
                 },
                 commandType: CommandType.StoredProcedure
             );
        }

        public async Task<int> AddGroupMaterialReference(int groupId, int materialId)
        {
            return await _connection.ExecuteAsync
            (
                _groupMaterialInsertProcedure,
                new
                {
                    groupId,
                    materialId
                },
                commandType: CommandType.StoredProcedure
            );
        }

        public async Task<int> RemoveGroupMaterialReference(int groupId, int materialId)
        {
            return await _connection.ExecuteAsync
            (
                _groupMaterialDeleteProcedure,
                new
                {
                    groupId,
                    materialId
                },
                commandType: CommandType.StoredProcedure
            );
        }

        public async Task<int> AddUserToGroup(int groupId, int userId, int roleId)
        {
            return await _connection.ExecuteAsync
            (
                _userGroupInsertProcedure,
                new
                {
                    groupId,
                    userId,
                    roleId
                },
                commandType: CommandType.StoredProcedure
            );
        }

        public async Task<int> DeleteUserFromGroup(int userId, int groupId)
        {
            return await _connection.ExecuteAsync
            (
                _userGroupDeleteProcedure,
                new
                {
                    userId,
                    groupId
                },
                commandType: CommandType.StoredProcedure
            );
        }

        public async Task DeleteTaskFromGroup(int groupId, int taskId)
        {
            await _connection.ExecuteAsync
            (
                _groupTaskDeleteProcedure,
                new
                {
                    groupId,
                    taskId
                },
                commandType: CommandType.StoredProcedure
            );
        }

        public List<GroupDto> GetGroupsByMaterialId(int id)
        {
            return _connection.Query<GroupDto>
            (
                _groupSelectAllByMaterialIdProcedure,
                new { id },
                commandType: CommandType.StoredProcedure
            ).ToList();
        }

        public async Task<int> GetPresentGroupForStudentByUserId(int userId)
        {
            return await _connection.QuerySingleAsync<int>
            (
                _groupSelectPresentGroupForStudentByUserIdProcedure,
                new { Id = userId },
                commandType: CommandType.StoredProcedure
            );
        }

        public List<GroupDto> GetGroupsByCourseId(int courseId)
        {
            return _connection.Query<GroupDto>(
                    _groupSelectByCourseProcedure,
                    new { courseId },
                    commandType: CommandType.StoredProcedure)
                .ToList();
        }

        public List<GroupDto> GetGroupsByTaskId(int taskId)
        {
            GroupDto result;
            return _connection
                .Query<GroupDto, GroupStatus, GroupDto>(
                    _groupSelectAllByTaskIdProcedure,
                    (group, groupStatus) =>
                    {
                        result = group;
                        result.GroupStatus = groupStatus;
                        return result;
                    },
                    new { taskId },
                    splitOn: "Id",
                    commandType: CommandType.StoredProcedure
                )
                .ToList();
        }

        public List<GroupDto> GetGroupsByLessonId(int lessonId)
        {
            GroupDto result;
            return _connection
                .Query<GroupDto, GroupStatus, GroupDto>(
                    _groupSelectGroupsByLessonIdProcedure,
                    (group, groupStatus) =>
                    {
                        result = group;
                        result.GroupStatus = groupStatus;
                        return result;
                    },
                    new { lessonId },
                    splitOn: "Id",
                    commandType: CommandType.StoredProcedure
                )
                .ToList();
        }

        public List<GroupDto> GetGroupsByUserId(int userId)
        {
            GroupDto result;
            return _connection
                .Query<GroupDto, GroupStatus, CourseDto, GroupDto>(
                    _groupSelectGroupsByUserIdProcedure,
                    (group, groupStatus, course) =>
                    {
                        result = group;
                        result.GroupStatus = groupStatus;
                        result.Course = course;
                        return result;
                    },
                    new { userId },
                    splitOn: "Id",
                    commandType: CommandType.StoredProcedure
                )
                .ToList();
        }
    }
}