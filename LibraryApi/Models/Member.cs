namespace LibraryApi.Models;

public class Member
{
    public int Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public DateTime JoinDate { get; set; }
    public List<Loan> Loans { get; set; } = new();
    public List<Reservation> Reservations { get; set; } = new();
}
