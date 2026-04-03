namespace EntpFlow.DTOs;

public class UserDto
    {
        public int UserId {get; set; }
        public string? EmployeeId { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? Role { get; set; }
        public string? Department { get; set; }
        public bool IsActive {get; set;}
    }
