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
            
            if (!string.IsNullOrEmpty(result.Message) && result.Message.ToLower().Contains("invalid") || result.Message.ToLower().Contains("must be"))
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
        [ProducesResponseType(typeof(PaginatedTemplatesResponse), 200)]
        public async Task<ActionResult<PaginatedTemplatesResponse>> GetListTemplates([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            if (page < 1)
            {
                return BadRequest(new { status = "error", message = "Page number must be greater than 0" });
            }

            if (pageSize < 1 || pageSize > 50)
            {
                return BadRequest(new { status = "error", message = "Page size must be between 1 and 50" });
            }
            try
            {
                var result = await _templateService.GetAllTemplatesAsync();
                var allTemplates = result.Data ?? new List<ConsultationTemplateResponse>();

                // Only consider active templates
                var activeTemplates = allTemplates
                    .Where(t => t.IsActive)
                    .OrderByDescending(t => t.CreatedAt)
                    .ToList();

                var totalCount = activeTemplates.Count;
                if (totalCount == 0)
                {
                    return StatusCode(503, new { status = "error", message = "No templates available" });
                }

                var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);
                var skip = (page - 1) * pageSize;

                var paginatedTemplates = activeTemplates.Skip(skip).Take(pageSize).ToList();

                var pagination = new PaginationMetadata
                {
                    CurrentPage = page,
                    PageSize = pageSize,
                    TotalCount = totalCount,
                    TotalPages = totalPages,
                    HasNext = page < totalPages,
                    HasPrevious = page > 1
                };

                var response = new PaginatedTemplatesResponse
                {
                    Status = "success",
                    Data = paginatedTemplates,
                    Pagination = pagination
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { status = "error", message = "Internal server error", details = ex.Message });
            }
        }
    }
}