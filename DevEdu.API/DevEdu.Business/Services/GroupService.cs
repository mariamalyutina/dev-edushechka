﻿using DevEdu.Business.IdentityInfo;
using DevEdu.Business.ValidationHelpers;
using DevEdu.DAL.Enums;
using DevEdu.DAL.Models;
using DevEdu.DAL.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DevEdu.Business.Services
{
    public class GroupService : IGroupService
    {
        private readonly IGroupRepository _groupRepository;
        private readonly IUserRepository _userRepository;
        private readonly IGroupValidationHelper _groupHelper;
        private readonly ILessonValidationHelper _lessonHelper;
        private readonly IMaterialValidationHelper _materialHelper;
        private readonly IUserValidationHelper _userHelper;
        private readonly ITaskValidationHelper _taskHelper;
        private readonly ILessonRepository _lessonRepository;
        private readonly IMaterialRepository _materialRepository;
        private readonly ITaskRepository _taskRepository;

        public GroupService
        (
            IGroupRepository groupRepository,
            IGroupValidationHelper groupHelper,
            IUserRepository userRepository,
            IUserValidationHelper userHelper,
            ILessonRepository lessonRepository,
            ILessonValidationHelper lessonHelper,
            IMaterialRepository materialRepository,
            IMaterialValidationHelper materialHelper,
            ITaskRepository taskRepository,
            ITaskValidationHelper taskHelper
        )
        {
            _groupRepository = groupRepository;
            _userRepository = userRepository;
            _groupHelper = groupHelper;
            _lessonHelper = lessonHelper;
            _materialHelper = materialHelper;
            _userHelper = userHelper;
            _taskHelper = taskHelper;
            _lessonRepository = lessonRepository;
            _materialRepository = materialRepository;
            _taskRepository = taskRepository;
        }

        public async Task<int> AddGroup(GroupDto groupDto) => await _groupRepository.AddGroup(groupDto);

        public async Task DeleteGroup(int id)
        {
            await _groupHelper.CheckGroupExistence(id);

            await _groupRepository.DeleteGroup(id);
        }

        public async Task<GroupDto> GetGroup(int groupId, UserIdentityInfo userInfo)
        {
            await _groupHelper.CheckGroupExistence(groupId);
            _groupHelper.CheckAccessGetGroupMembers(groupId, userInfo);

            var dto = await _groupRepository.GetGroup(groupId);
            if (dto != null)
            {
                var tasks = new List<Task>
                {
                    Task.Run(() => { dto.Students =
                        _userRepository.GetUsersByGroupIdAndRole(groupId, (int)Role.Student);}),
                    Task.Run(() => { dto.Tutors =
                        _userRepository.GetUsersByGroupIdAndRole(groupId, (int)Role.Tutor);}),
                    Task.Run(() => { dto.Teachers =
                        _userRepository.GetUsersByGroupIdAndRole(groupId, (int)Role.Teacher);})
                };
                await Task.WhenAll(tasks);
            }
            return dto;
        }

        public async Task<List<GroupDto>> GetGroups() => await _groupRepository.GetGroups();

        public async Task<int> AddGroupLesson(int groupId, int lessonId)
        {
            await _groupHelper.CheckGroupExistence(groupId);
            _lessonHelper.CheckLessonExistence(lessonId);

            return await _groupRepository.AddGroupToLesson(groupId, lessonId);
        }

        public async Task RemoveGroupLesson(int groupId, int lessonId)
        {
            await _groupHelper.CheckGroupExistence(groupId);
            _lessonHelper.CheckLessonExistence(lessonId);

            await _groupRepository.RemoveGroupFromLesson(groupId, lessonId);
        }

        public async Task<GroupDto> UpdateGroup(int id, GroupDto groupDto, UserIdentityInfo userInfo)
        {
            await _groupHelper.CheckGroupExistence(id);
            _groupHelper.CheckAccessGroup(userInfo, id);

            groupDto.Id = id;
            return await _groupRepository.UpdateGroup(groupDto);
        }

        public async Task<GroupDto> ChangeGroupStatus(int groupId, GroupStatus statusId)
        {
            await _groupHelper.CheckGroupExistence(groupId);

            return await _groupRepository.ChangeGroupStatus(groupId, (int)statusId);
        }

        public async Task<int> AddGroupMaterialReference(int groupId, int materialId, UserIdentityInfo userInfo)
        {
            await _groupHelper.CheckGroupExistence(groupId);
            _materialHelper.CheckMaterialExistence(materialId);
            _groupHelper.CheckAccessGroupAndMaterial(userInfo, groupId, materialId);

            return await _groupRepository.AddGroupMaterialReference(groupId, materialId);
        }

        public async Task<int> RemoveGroupMaterialReference(int groupId, int materialId, UserIdentityInfo userInfo)
        {
            await _groupHelper.CheckGroupExistence(groupId);
            _materialHelper.CheckMaterialExistence(materialId);
            _groupHelper.CheckAccessGroupAndMaterial(userInfo, groupId, materialId);

            return await _groupRepository.RemoveGroupMaterialReference(groupId, materialId);
        }

        public async Task<int> AddGroupToLesson(int groupId, int lessonId, UserIdentityInfo userInfo)
        {
            await _groupHelper.CheckGroupExistence(groupId);
            _lessonHelper.CheckLessonExistence(lessonId);
            _groupHelper.CheckAccessGroupAndLesson(userInfo, groupId, lessonId);

            return await _groupRepository.AddGroupToLesson(groupId, lessonId);
        }

        public async Task RemoveGroupFromLesson(int groupId, int lessonId, UserIdentityInfo userInfo)
        {
            await _groupHelper.CheckGroupExistence(groupId);
            _lessonHelper.CheckLessonExistence(lessonId);
            _groupHelper.CheckAccessGroupAndLesson(userInfo, groupId, lessonId);

            await _groupRepository.RemoveGroupFromLesson(groupId, lessonId);
        }

        public async Task AddUserToGroup(int groupId, int userId, Role roleId, UserIdentityInfo userInfo)
        {
            await _groupHelper.CheckGroupExistence(groupId);
            _userHelper.CheckUserExistence(userId);
            _groupHelper.CheckAccessGroupAndUser(userInfo, groupId, userId);


            await _groupRepository.AddUserToGroup(groupId, userId, (int)roleId);
        }

        public async Task DeleteUserFromGroup(int userId, int groupId, UserIdentityInfo userInfo)
        {
            await _groupHelper.CheckGroupExistence(groupId);
            _userHelper.CheckUserExistence(userId);
            _groupHelper.CheckAccessGroupAndUser(userInfo, groupId, userId);

            await _groupRepository.DeleteUserFromGroup(userId, groupId);
        }


        public async Task<int> AddTaskToGroup(int groupId, int taskId, GroupTaskDto dto, UserIdentityInfo userInfo)
        {
            await _groupHelper.CheckGroupExistence(groupId);
            _groupHelper.CheckAccessGroupAndTask(userInfo, groupId, taskId);
            _taskHelper.CheckTaskExistence(taskId);

            dto.Group = new GroupDto { Id = groupId };
            dto.Task = new TaskDto { Id = taskId };
            return await _groupRepository.AddTaskToGroup(dto);
        }

        public async Task DeleteTaskFromGroup(int groupId, int taskId, UserIdentityInfo userInfo)
        {
            await _groupHelper.CheckGroupExistence(groupId);
            _groupHelper.CheckAccessGroupAndTask(userInfo, groupId, taskId);
            _taskHelper.CheckTaskExistence(taskId);

            await _groupRepository.DeleteTaskFromGroup(groupId, taskId);
        }

        public async Task<List<GroupTaskDto>> GetTasksByGroupId(int groupId, UserIdentityInfo userInfo)
        {
            await _groupHelper.CheckGroupExistence(groupId);
            _groupHelper.CheckAccessGroup(userInfo, groupId);

            return await _groupRepository.GetTaskGroupByGroupId(groupId);
        }

        public async Task<GroupTaskDto> GetGroupTask(int groupId, int taskId, UserIdentityInfo userInfo)
        {
            await _groupHelper.CheckGroupExistence(groupId);
            _groupHelper.CheckAccessGroupAndTask(userInfo, groupId, taskId);
            _taskHelper.CheckTaskExistence(taskId);

            return await _groupRepository.GetGroupTask(groupId, taskId);
        }

        public async Task<GroupTaskDto> UpdateGroupTask(int groupId, int taskId, GroupTaskDto dto, UserIdentityInfo userInfo)
        {
            var userId = userInfo.UserId;
            var roles = userInfo.Roles;

            if (!userInfo.IsAdmin()) { }

            await _groupHelper.CheckGroupExistence(groupId);
            _groupHelper.CheckAccessGroupAndTask(userInfo, groupId, taskId);
            _taskHelper.CheckTaskExistence(taskId);

            dto.Group = new GroupDto { Id = groupId };
            dto.Task = new TaskDto { Id = taskId };
            await _groupRepository.UpdateGroupTask(dto);
            return await _groupRepository.GetGroupTask(groupId, taskId);
        }

        private void CheckUserAccessByRoleAndId(UserIdentityInfo userInfo, GroupDto dto)
        {
            var userId = userInfo.UserId;
            if (userInfo.IsAdmin())
            {
                return;
            }

            CheckUserAccessToGroupData(dto, userId);

            if (userInfo.IsStudent())
            {

            }
        }

        private void CheckUserAccessToGroupData(GroupDto dto, int userId)
        {
            if (dto.Course != default)
            {
            }
            else
            {
            }
        }
    }
}