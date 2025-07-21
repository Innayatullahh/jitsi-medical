using System;
using System.Collections.Generic;
using System.Linq;
using JitsiAppointmentApi.Application.DTOs;
using JitsiAppointmentApi.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JitsiAppointmentApi.Controllers
{
    [ApiController]
    [ApiVersion("2.0")]
    [Route("api/v{version:apiVersion}/templates")]
    [Authorize]
    public class TemplatesController : ControllerBase
    {
        private readonly IConsultationTemplateService _templateService;

        public TemplatesController(IConsultationTemplateService templateService)
        {
            _templateService = templateService;
        }

        /// <summary>
        /// Create a new consultation template
        /// </summary>
        /// <param name="request">Consultation template information</param>
        /// <returns>Created consultation template</returns>
        /// <response code="200">Consultation template created successfully</response>
        /// <response code="400">Invalid request data</response>
        [HttpPost]
        [ProducesResponseType(typeof(CreateConsultationTemplateResponse), 200)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<CreateConsultationTemplateResponse>> CreateTemplate([FromBody] CreateConsultationTemplateRequest request)
        {
            var result = await _templateService.CreateTemplateAsync(request);
            
            if (!string.IsNullOrEmpty(result.Message) && result.Message.Contains("Invalid") || result.Message.Contains("must be"))
            {
                return BadRequest(result);
            }
            
            return Ok(result);
        }

        /// <summary>
        /// Returns a list of saved consultation templates
        /// </summary>
        /// <returns>List of consultation templates</returns>
        /// <response code="200">Returns the list of templates</response>
        [HttpGet]
        [ProducesResponseType(typeof(List<ConsultationTemplateResponse>), 200)]
        public async Task<ActionResult<List<ConsultationTemplateResponse>>> GetListTemplates()
        {
            var result = await _templateService.GetAllTemplatesAsync();
            return Ok(result.Data);
        }
    }
} 