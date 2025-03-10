﻿using System;
using AutoMapper;
using DevEdu.API.Common;
using DevEdu.API.Extensions;
using DevEdu.API.Models;
using DevEdu.Business.Services;
using DevEdu.DAL.Enums;
using DevEdu.DAL.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using DevEdu.API.Configuration.ExceptionResponses;

namespace DevEdu.API.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class GroupController : Controller
    {
        private readonly IMapper _mapper;
        private readonly IGroupService _groupService;

        public GroupController(IMapper mapper, IGroupService service)
        {
            _mapper = mapper;
            _groupService = service;
        }

        //  api/Group
        [HttpPost]
        [Description("Add new Group")]
        [AuthorizeRoles(Role.Manager)]
        [ProducesResponseType(typeof(GroupOutputModel), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ValidationExceptionResponse), StatusCodes.Status422UnprocessableEntity)]
        public async Task<ActionResult<GroupOutputModel>> AddGroup([FromBody] GroupInputModel model)
        {
            var dto = _mapper.Map<GroupDto>(model);
            var result = await _groupService.AddGroup(dto);
            var output = _mapper.Map<GroupOutputModel>(result);
            return Created(new Uri($"api/Group/{output.Id}", UriKind.Relative), output);
        }

        //  api/Group/{id}
        [HttpGet("{id}")]
        [Description("Return Group by id")]
        [ProducesResponseType(typeof(GroupFullOutputModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status404NotFound)]
        public async Task<GroupFullOutputModel> GetGroup(int id)
        {
            var userInfo = this.GetUserIdAndRoles();

            var dto = await _groupService.GetGroup(id, userInfo);
            return _mapper.Map<GroupFullOutputModel>(dto);
        }

        //  api/Group/
        [HttpGet]
        [Description("Get all Groups")]
        [AuthorizeRoles(Role.Manager)]
        [ProducesResponseType(typeof(List<GroupOutputModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
        public async Task<List<GroupOutputModel>> GetAllGroups()
        {
            var list = await _groupService.GetGroups();
            return _mapper.Map<List<GroupOutputModel>>(list);
        }

        //  api/Group
        [HttpDelete("{id}")]
        [Description("Delete Group by Id")]
        [AuthorizeRoles(Role.Manager)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteGroup(int id)
        {
            await _groupService.DeleteGroup(id);
            return NoContent();
        }

        //  api/Group/{Id}
        [HttpPut("{id}")]
        [Description("Update Group by id")]
        [AuthorizeRoles(Role.Manager)]
        [ProducesResponseType(typeof(GroupInfoOutputModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ValidationExceptionResponse), StatusCodes.Status422UnprocessableEntity)]
        public async Task<GroupInfoOutputModel> UpdateGroup(int id, [FromBody] GroupInputModel model)
        {
            var userInfo = this.GetUserIdAndRoles();

            var dto = _mapper.Map<GroupDto>(model);
            var output = await _groupService.UpdateGroup(id, dto, userInfo);
            return _mapper.Map<GroupInfoOutputModel>(output);
        }

        //  api/Group/{groupId}/change-status/{statusId}
        [HttpPut("{groupId}/change-status/{statusId}")]
        [Description("Change group status by id")]
        [AuthorizeRoles(Role.Manager)]
        [ProducesResponseType(typeof(GroupOutputBaseModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status404NotFound)]
        public async Task<GroupOutputBaseModel> ChangeGroupStatus(int groupId, GroupStatus statusId)
        {
            var dto = await _groupService.ChangeGroupStatus(groupId, statusId);
            return _mapper.Map<GroupOutputBaseModel>(dto);
        }

        //add group_lesson relation
        // api/Group/{groupId}/lesson/{lessonId}
        [HttpPost("{groupId}/lesson/{lessonId}")]
        [Description("Add group lesson reference")]
        [AuthorizeRoles(Role.Teacher)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status404NotFound)]
        public async Task<ActionResult> AddGroupToLesson(int groupId, int lessonId)
        {
            var userInfo = this.GetUserIdAndRoles();

            await _groupService.AddGroupToLesson(groupId, lessonId, userInfo);
            return NoContent();
        }

        // api/Group/{groupId}/lesson/{lessonId}
        [HttpDelete("{groupId}/lesson/{lessonId}")]
        [Description("Delete lesson from groupId")]
        [AuthorizeRoles(Role.Teacher)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status404NotFound)]
        public async Task<ActionResult> RemoveGroupFromLesson(int groupId, int lessonId)
        {
            var userInfo = this.GetUserIdAndRoles();

            await _groupService.RemoveGroupFromLesson(groupId, lessonId, userInfo);
            return NoContent();
        }

        // api/Group/{groupId}/material/{materialId}
        [HttpPost("{groupId}/material/{materialId}")]
        [Description("Add material to group")]
        [AuthorizeRoles(Role.Manager, Role.Teacher, Role.Tutor)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status404NotFound)]
        public async Task<ActionResult> AddGroupMaterialReference(int groupId, int materialId)
        {
            var userInfo = this.GetUserIdAndRoles();

            await _groupService.AddGroupMaterialReference(groupId, materialId, userInfo);
            return NoContent();
        }

        // api/Group/{groupId}/material/{materialId}
        [HttpDelete("{groupId}/material/{materialId}")]
        [Description("Remove material from group")]
        [AuthorizeRoles(Role.Manager, Role.Teacher, Role.Tutor)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> RemoveGroupMaterialReference(int groupId, int materialId)
        {
            var userInfo = this.GetUserIdAndRoles();

            await _groupService.RemoveGroupMaterialReference(groupId, materialId, userInfo);
            return NoContent();
        }

        //  api/group/1/user/2/role/1
        [HttpPost("{groupId}/user/{userId}/role/{roleId}")]
        [Description("Add user to group")]
        [AuthorizeRoles(Role.Manager)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> AddUserToGroup(int groupId, int userId, Role roleId)
        {
            var userInfo = this.GetUserIdAndRoles();

            await _groupService.AddUserToGroup(groupId, userId, roleId, userInfo);
            return NoContent();
        }

        //  api/group/1/user/2
        [HttpDelete("{groupId}/user/{userId}")]
        [Description("Delete user from group")]
        [AuthorizeRoles(Role.Manager)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteUserFromGroup(int userId, int groupId)
        {
            var userInfo = this.GetUserIdAndRoles();

            await _groupService.DeleteUserFromGroup(userId, groupId, userInfo);
            return NoContent();
        }

        //  api/group/1/task/1
        [HttpDelete("{groupId}/task/{taskId}")]
        [Description("Delete task from group")]
        [AuthorizeRoles(Role.Teacher)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteTaskFromGroup(int groupId, int taskId)
        {
            var userInfo = this.GetUserIdAndRoles();

            await _groupService.DeleteTaskFromGroup(groupId, taskId, userInfo);
            return NoContent();
        }
    }
}