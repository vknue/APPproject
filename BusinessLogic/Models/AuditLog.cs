using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Models
{
    public class AuditLog
    {
        public int Id { get; set; }
        public string? UserId { get; set; }
        public string UserName { get; set; } = "Guest";
        public string Action { get; set; } = string.Empty;
        public string Controller { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; } = DateTime.Now;
        public string? IpAddress { get; set; }
    }
}
