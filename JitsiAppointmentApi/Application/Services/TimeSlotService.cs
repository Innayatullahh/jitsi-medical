using JitsiAppointmentApi.Application.DTOs;
using JitsiAppointmentApi.Application.Interfaces;
using JitsiAppointmentApi.Core.Entities;
using JitsiAppointmentApi.Core.Interfaces;

namespace JitsiAppointmentApi.Application.Services
{
    public class TimeSlotService : ITimeSlotService
    {
        private readonly ITimeSlotRepository _timeSlotRepository;

        public TimeSlotService(ITimeSlotRepository timeSlotRepository)
        {
            _timeSlotRepository = timeSlotRepository;
        }

        public async Task<CreateTimeSlotResponse> CreateTimeSlotAsync(CreateTimeSlotRequest request)
        {
            // Validate request based on recurring vs non-recurring
            if (!request.IsRecurring)
            {
                // Non-recurring: Date is mandatory
                if (string.IsNullOrEmpty(request.Date))
                {
                    return new CreateTimeSlotResponse
                    {
                        Status = "error",
                        Message = "Date is required for non-recurring time slots"
                    };
                }
                
                // Validate date format
                if (!DateTime.TryParse(request.Date, out _))
                {
                    return new CreateTimeSlotResponse
                    {
                        Status = "error",
                        Message = "Invalid date format. Use yyyy-MM-dd format"
                    };
                }
            }
            else
            {
                // Recurring: RecurringDays is mandatory, Date is optional
                if (request.RecurringDays == null || request.RecurringDays.Count == 0)
                {
                    return new CreateTimeSlotResponse
                    {
                        Status = "error",
                        Message = "Recurring days are required for recurring time slots"
                    };
                }
            }
            
            // Generate a unique ID
            var timeSlotId = Guid.NewGuid().ToString("N").Substring(0, 8);
            
            // Convert recurring days to JSON
            var recurringDaysJson = System.Text.Json.JsonSerializer.Serialize(request.RecurringDays);

            // Parse date if provided
            DateTime? parsedDate = null;
            if (!string.IsNullOrEmpty(request.Date) && DateTime.TryParse(request.Date, out var date))
            {
                parsedDate = DateTime.SpecifyKind(date.Date, DateTimeKind.Utc);
            }

            var timeSlot = new TimeSlot
            {
                Id = timeSlotId,
                StartTime = request.StartTime,
                EndTime = request.EndTime,
                IsVirtual = request.IsVirtual,
                IsRecurring = request.IsRecurring,
                RecurringDaysJson = recurringDaysJson,
                Date = parsedDate
            };

            var createdTimeSlot = await _timeSlotRepository.AddAsync(timeSlot);

            return new CreateTimeSlotResponse
            {
                Status = "success",
                Message = "Time slot created successfully",
                Data = MapToTimeSlotData(createdTimeSlot)
            };
        }

        public async Task<TimeSlotsByDayResponse> GetTimeSlotsByDayAsync(string date)
        {
            DateTime parsedDate;
            
            // Try common date formats automatically
            string[] commonFormats = { "yyyy-MM-dd", "MM/dd/yyyy", "dd-MM-yyyy", "yyyy/MM/dd", "dd/MM/yyyy" };
            if (!DateTime.TryParseExact(date, commonFormats, null, System.Globalization.DateTimeStyles.None, out parsedDate))
            {
                return new TimeSlotsByDayResponse
                {
                    Status = "error",
                    Data = new List<TimeSlotResponse>()
                };
            }

            // Ensure the date is in UTC
            var utcDate = DateTime.SpecifyKind(parsedDate.Date, DateTimeKind.Utc);
            var dayOfWeek = utcDate.DayOfWeek.ToString().ToLower();
            
            var timeSlots = await _timeSlotRepository.GetByDateAsync(utcDate);
            var recurringTimeSlots = await _timeSlotRepository.GetRecurringByDayOfWeekAsync(dayOfWeek);
            
            var allTimeSlots = timeSlots.Concat(recurringTimeSlots).Distinct().ToList();

            return new TimeSlotsByDayResponse
            {
                Status = "success",
                Data = allTimeSlots.Select(MapToTimeSlotResponse).ToList()
            };
        }

        public async Task<TimeSlotsByWeekResponse> GetTimeSlotsByWeekAsync(string startDate)
        {
            DateTime parsedDate;
            
            // Try common date formats automatically
            string[] commonFormats = { "yyyy-MM-dd", "MM/dd/yyyy", "dd-MM-yyyy", "yyyy/MM/dd", "dd/MM/yyyy" };
            if (!DateTime.TryParseExact(startDate, commonFormats, null, System.Globalization.DateTimeStyles.None, out parsedDate))
            {
                return new TimeSlotsByWeekResponse
                {
                    Status = "error",
                    Data = new Dictionary<string, List<TimeSlotResponse>>()
                };
            }

            // Ensure the start date is in UTC
            var utcStartDate = DateTime.SpecifyKind(parsedDate.Date, DateTimeKind.Utc);
            var endDate = utcStartDate.AddDays(6);
            
            var response = new TimeSlotsByWeekResponse
            {
                Status = "success",
                Data = new Dictionary<string, List<TimeSlotResponse>>()
            };

            for (int i = 0; i < 7; i++)
            {
                var currentDate = utcStartDate.AddDays(i);
                var dayOfWeek = currentDate.DayOfWeek.ToString().ToLower();
                
                var timeSlots = await _timeSlotRepository.GetByDateAsync(currentDate);
                var recurringTimeSlots = await _timeSlotRepository.GetRecurringByDayOfWeekAsync(dayOfWeek);
                
                var allTimeSlots = timeSlots.Concat(recurringTimeSlots).Distinct().ToList();

                response.Data[dayOfWeek] = allTimeSlots.Select(MapToTimeSlotResponse).ToList();
            }

            return response;
        }

        public async Task<TimeSlotsByMonthResponse> GetTimeSlotsByMonthAsync(string month)
        {
            if (!DateTime.TryParse(month + "-01", out var startDate))
            {
                return new TimeSlotsByMonthResponse
                {
                    Status = "error",
                    Data = new Dictionary<string, List<TimeSlotResponse>>()
                };
            }

            // Ensure the start date is in UTC
            var utcStartDate = DateTime.SpecifyKind(startDate.Date, DateTimeKind.Utc);
            var endDate = utcStartDate.AddMonths(1).AddDays(-1);
            
            var response = new TimeSlotsByMonthResponse
            {
                Status = "success",
                Data = new Dictionary<string, List<TimeSlotResponse>>()
            };

            var currentDate = utcStartDate;
            while (currentDate <= endDate)
            {
                var dayOfWeek = currentDate.DayOfWeek.ToString().ToLower();
                var dateKey = currentDate.ToString("yyyy-MM-dd");
                
                var timeSlots = await _timeSlotRepository.GetByDateAsync(currentDate);
                var recurringTimeSlots = await _timeSlotRepository.GetRecurringByDayOfWeekAsync(dayOfWeek);
                
                var allTimeSlots = timeSlots.Concat(recurringTimeSlots).Distinct().ToList();

                response.Data[dateKey] = allTimeSlots.Select(MapToTimeSlotResponse).ToList();

                currentDate = currentDate.AddDays(1);
            }

            return response;
        }

        public async Task<DeleteTimeSlotResponse> DeleteTimeSlotAsync(string id)
        {
            var timeSlot = await _timeSlotRepository.GetByIdAsync(id);
            
            if (timeSlot == null)
            {
                return new DeleteTimeSlotResponse
                {
                    Status = "error",
                    Message = "Time slot not found"
                };
            }

            await _timeSlotRepository.DeleteAsync(id);

            return new DeleteTimeSlotResponse
            {
                Status = "success",
                Message = "Time slot deleted successfully"
            };
        }

        private static TimeSlotResponse MapToTimeSlotResponse(TimeSlot timeSlot)
        {
            return new TimeSlotResponse
            {
                Id = timeSlot.Id,
                StartTime = timeSlot.StartTime,
                EndTime = timeSlot.EndTime,
                IsVirtual = timeSlot.IsVirtual,
                IsBooked = timeSlot.IsBooked
            };
        }

        private static TimeSlotData MapToTimeSlotData(TimeSlot timeSlot)
        {
            // Parse recurring days from JSON
            var recurringDays = System.Text.Json.JsonSerializer.Deserialize<List<string>>(timeSlot.RecurringDaysJson) ?? new List<string>();
            
            return new TimeSlotData
            {
                Id = timeSlot.Id,
                StartTime = timeSlot.StartTime,
                EndTime = timeSlot.EndTime,
                IsVirtual = timeSlot.IsVirtual,
                IsRecurring = timeSlot.IsRecurring,
                RecurringDays = recurringDays,
                Date = timeSlot.Date?.ToString("yyyy-MM-dd")
            };
        }
    }
} 