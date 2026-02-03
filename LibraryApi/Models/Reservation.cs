namespace LibraryApi.Models;

public class Reservation
{
    public int Id { get; set; }
    public int MemberId { get; set; }
    public Member? Member { get; set; }
    public int BookId { get; set; }
    public Book? Book { get; set; }
    public DateTime ReservedAt { get; set; }
    public ReservationStatus Status { get; set; } = ReservationStatus.Active;
}

public enum ReservationStatus
{
    Active = 0,
    Cancelled = 1,
    Fulfilled = 2
}
