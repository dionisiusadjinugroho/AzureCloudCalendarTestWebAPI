using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AzureCloudCalendarTestWebAPI.Models;
using System.Reflection;

namespace AzureCloudCalendarTestWebAPI.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class CalendarsController : ControllerBase
    {
        private readonly AzureCloudDBContext _context;

        public CalendarsController(AzureCloudDBContext context)
        {
            _context = context;
        }

        // GET: api/Calendars
        // GET: api/Calendars?from={startdate}?to={enddate}
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Calendar>>> GetCalendar([FromQuery(Name = "from")] DateTime startdate,[FromQuery(Name = "to")] DateTime enddate)
        {
            if(startdate != DateTime.MinValue && enddate != DateTime.MinValue)
            {
                var getcalendarrange = _context.Calendar.Where(x => x.CalendarEventStartDate >= startdate && x.CalendarEventEndDate <= enddate).ToListAsync();
                return await getcalendarrange;
            }
            else
            {
                return await _context.Calendar.ToListAsync();
            }
        }


        // POST: api/Calendars/CheckConflict
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpGet("CheckConflict/{name}")]
        public async Task<ActionResult<List<Calendar>>> CheckConflictCalendar(string name)
        {
            var listcalendaruser = await _context.Calendar.Where(x => x.CreatedBy == name).ToListAsync();
            List<Calendar> listconflict = new List<Calendar>();
            foreach (var item in listcalendaruser)
            {
                var findoverlap = listcalendaruser.Where(x => x.CalendarId != item.CalendarId &&
                (
                (x.CalendarEventStartDate <= item.CalendarEventStartDate && x.CalendarEventEndDate >= item.CalendarEventEndDate)
                || (x.CalendarEventStartDate >= item.CalendarEventStartDate && x.CalendarEventStartDate < item.CalendarEventEndDate)
                || (x.CalendarEventEndDate > item.CalendarEventStartDate && x.CalendarEventEndDate <= item.CalendarEventEndDate) )).FirstOrDefault();
                if(findoverlap != null)
                {
                    listconflict.Add(findoverlap);
                }
            }
            return listconflict;
        }

        // PUT: api/Calendars/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCalendar(Guid id, Calendar calendar)
        {
            if (!CalendarExists(id))
            {
                return NotFound();
            }

            if (id != calendar.CalendarId)
            {
                return BadRequest();
            }

            _context.Entry(calendar).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CalendarExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Ok();
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> PatchCalendar(Guid id,Calendar calendartoPatch)
        {
            var calendarfromDB = _context.Calendar.Where(x => x.CalendarId == id).FirstOrDefault();
            if(calendarfromDB == null)
            {
                return NotFound();
            }

            foreach (PropertyInfo pi in calendartoPatch.GetType().GetProperties())
            {
                object value = pi.GetValue(calendartoPatch);
                if (Object.ReferenceEquals(value, null))
                    continue;
                    

                var type = value.GetType();
                if (type.IsValueType && Object.Equals(value, Activator.CreateInstance(type)) == true)
                    continue;

                calendarfromDB.GetType().GetProperty(pi.Name).SetValue(calendarfromDB, value);
            }

            calendarfromDB.LastUpdatedDate = DateTime.Now;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CalendarExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return Ok(calendarfromDB);
        }

        // POST: api/Calendars
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<Calendar>> PostCalendar(Calendar calendar)
        {
            calendar.CalendarId = Guid.NewGuid();
            calendar.Status = true;
            calendar.CreatedDate = DateTime.Now;
            

            _context.Calendar.Add(calendar);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (CalendarExists(calendar.CalendarId))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetCalendar", new { id = calendar.CalendarId }, calendar);
        }


        // DELETE: api/Calendars/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Calendar>> DeleteCalendar(Guid id)
        {
            var calendar = await _context.Calendar.FindAsync(id);
            if (calendar == null)
            {
                return NotFound();
            }

            _context.Calendar.Remove(calendar);
            await _context.SaveChangesAsync();

            return calendar;
        }

        private bool CalendarExists(Guid id)
        {
            return _context.Calendar.Any(e => e.CalendarId == id);
        }
    }
}
