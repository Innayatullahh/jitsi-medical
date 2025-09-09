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
                var activeTemplates = allTemplates.Where(t => t.IsActive).OrderByDescending(t => t.CreatedAt).ToList();

                // Flatten all slots from all active templates
                var allSlots = activeTemplates
                    .Where(t => t.TimeRanges != null)
                    .SelectMany(template => template.TimeRanges
                        .Where(tr => tr.TimeSlots != null)
                        .SelectMany(timeRange => timeRange.TimeSlots
                            .Select(slot => (template, timeRange, slot, (int)template.Id))
                        )
                    )
                    .ToList();

                var totalCount = allSlots.Count;
                if (totalCount == 0)
                {
                    return StatusCode(503, new { status = "error", message = "No slots available" });
                }

                var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);
                var skip = (page - 1) * pageSize;

                var paginatedSlots = allSlots.Skip(skip).Take(pageSize).ToList();

                // Group paginated slots back into templates for response
                var groupedTemplates = paginatedSlots
                    .GroupBy(s => s.template.Id)
                    .Select(g =>
                    {
                        var template = g.First().template;
                        var timeRanges = template.TimeRanges
                            .Where(tr => g.Any(s => s.timeRange == tr))
                            .Select(tr =>
                            {
                                var slots = tr.TimeSlots.Where(slot => g.Any(s => s.timeRange == tr && s.slot == slot)).ToList();
                                // Clone the time range and assign only paginated slots
                                var clonedTimeRange = new TimeRangeWithSlots
                                {
                                    TimeSlots = slots,
                                    Start = tr.Start,
                                    End = tr.End
                                };
                                return clonedTimeRange;
                            }).ToList();

                        // Clone the template and assign only paginated time ranges
                        var clonedTemplate = new ConsultationTemplateResponse
                        {
                            Id = template.Id,
                            StartDate = template.StartDate,
                            EndDate = template.EndDate,
                            Interval = template.Interval,
                            TimeRanges = timeRanges,
                            IsActive = template.IsActive,
                            CreatedAt = template.CreatedAt,
                            UpdatedAt = template.UpdatedAt
                        };
                        return clonedTemplate;
                    })
                    .ToList();

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
                    Data = groupedTemplates,
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