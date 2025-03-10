﻿using System;
using AutoMapper;
using DevEdu.API.Common;
using DevEdu.Business.Services;
using DevEdu.DAL.Enums;
using DevEdu.API.Models;
using DevEdu.DAL.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.ComponentModel;
using DevEdu.API.Configuration.ExceptionResponses;

namespace DevEdu.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TopicController : Controller
    {
        private readonly ITopicService _topicService;
        private readonly IMapper _mapper;

        public TopicController(IMapper mapper, ITopicService topicService)
        {
            _topicService = topicService;
            _mapper = mapper;
        }

        //  api/topic/{id}
        [AuthorizeRoles(Role.Manager, Role.Methodist)]
        [HttpGet("{id}")]
        [Description("Get topic by id")]
        [ProducesResponseType(typeof(TopicOutputModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status404NotFound)]
        public TopicOutputModel GetTopicById(int id)
        {
            var output = _topicService.GetTopic(id);
            return _mapper.Map<TopicOutputModel>(output);
        }

        [AuthorizeRoles(Role.Manager, Role.Methodist)]
        [HttpGet]
        [Description("Get all topics")]
        [ProducesResponseType(typeof(List<TopicOutputModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
        public List<TopicOutputModel> GetAllTopics()
        {
            var output = _topicService.GetAllTopics();
            return _mapper.Map<List<TopicOutputModel>>(output);
        }

        //  api/topic
        [AuthorizeRoles(Role.Manager, Role.Methodist)]
        [HttpPost]
        [Description("Add topic")]
        [ProducesResponseType(typeof(TopicOutputModel), (StatusCodes.Status201Created))]
        [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ValidationExceptionResponse), StatusCodes.Status422UnprocessableEntity)]
        public ActionResult<TopicOutputModel> AddTopic([FromBody] TopicInputModel model)
        {
            var dto = _mapper.Map<TopicDto>(model);
            var topicId = _topicService.AddTopic(dto);
            var output = GetTopicById(topicId);
            return Created(new Uri($"api/Topic/{output.Id}", UriKind.Relative), output);
        }

        //  api/topic/{id}
        [AuthorizeRoles(Role.Methodist, Role.Manager)]
        [HttpDelete("{id}")]
        [Description("Delete topic")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status404NotFound)]
        public ActionResult DeleteTopic(int id)
        {
            _topicService.DeleteTopic(id);
            return NoContent();
        }

        //  api/topic/{id}
        [AuthorizeRoles(Role.Methodist, Role.Manager)]
        [HttpPut("{id}")]
        [Description("Update topic")]
        [ProducesResponseType(typeof(TopicOutputModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ValidationExceptionResponse), StatusCodes.Status422UnprocessableEntity)]
        public TopicOutputModel UpdateTopic(int id, [FromBody] TopicInputModel model)
        {
            var dto = _mapper.Map<TopicDto>(model);
            var output = _topicService.UpdateTopic(id, dto);
            return _mapper.Map<TopicOutputModel>(output);
        }

        //  api/topic/{topicId}/tag/{tagId}
        [AuthorizeRoles(Role.Methodist, Role.Teacher)]
        [HttpPost("{topicId}/tag/{tagId}")]
        [Description("Add tag to topic")]
        [ProducesResponseType(typeof(int), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status404NotFound)]
        public ActionResult AddTagToTopic(int topicId, int tagId)
        {
            _topicService.AddTagToTopic(topicId, tagId);
            return NoContent();
        }

        //  api/topic/{topicId}/tag/{tagId}
        [AuthorizeRoles(Role.Methodist, Role.Teacher)]
        [HttpDelete("{topicId}/tag/{tagId}")]
        [Description("Delete tag from topic")]
        [ProducesResponseType(typeof(int), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status404NotFound)]
        public ActionResult DeleteTagFromTopic(int topicId, int tagId)
        {
            _topicService.DeleteTagFromTopic(topicId, tagId);
            return NoContent();
        }
    }
}