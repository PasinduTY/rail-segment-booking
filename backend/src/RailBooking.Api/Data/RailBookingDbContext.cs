namespace RailBooking.Api.Data;

using Microsoft.EntityFrameworkCore;
using RailBooking.Domain.Entities;

public class RailBookingDbContext(DbContextOptions<RailBookingDbContext> options) : DbContext(options)
{
    public DbSet<Station> Stations => Set<Station>();
    public DbSet<Train> Trains => Set<Train>();
    public DbSet<Coach> Coaches => Set<Coach>();
    public DbSet<Seat> Seats => Set<Seat>();
    public DbSet<TripDeparture> TripDepartures => Set<TripDeparture>();
    public DbSet<Booking> Bookings => Set<Booking>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        ConfigureStation(modelBuilder);
        ConfigureTrainAndCoach(modelBuilder);
        ConfigureSeat(modelBuilder);
        ConfigureTripDeparture(modelBuilder);
        ConfigureBooking(modelBuilder);

        modelBuilder.Entity<Station>().HasData(SeedData.GetStations());
        modelBuilder.Entity<Train>().HasData(SeedData.GetTrains());
        modelBuilder.Entity<Coach>().HasData(SeedData.GetCoaches());
        modelBuilder.Entity<Seat>().HasData(SeedData.GetSeats());
    }

    private static void ConfigureStation(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Station>(entity =>
        {
            entity.HasIndex(s => s.SequenceNumber).IsUnique();
            entity.Property(s => s.Name).IsRequired().HasMaxLength(100);
            entity.Property(s => s.DistanceKm).HasColumnType("numeric(8,2)");
        });
    }

    private static void ConfigureTrainAndCoach(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Train>(entity =>
        {
            entity.Property(t => t.Name).IsRequired().HasMaxLength(100);
        });

        modelBuilder.Entity<Coach>(entity =>
        {
            entity.HasIndex(c => new { c.TrainId, c.CoachNumber }).IsUnique();
            entity.Property(c => c.Type).HasConversion<string>().HasMaxLength(20);

            entity.HasOne<Train>()
                .WithMany(t => t.Coaches)
                .HasForeignKey(c => c.TrainId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }

    private static void ConfigureSeat(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Seat>(entity =>
        {
            entity.HasIndex(s => new { s.CoachId, s.SeatNumber }).IsUnique();

            entity.HasOne<Coach>()
                .WithMany(c => c.Seats)
                .HasForeignKey(s => s.CoachId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }

    private static void ConfigureTripDeparture(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TripDeparture>(entity =>
        {
            entity.HasIndex(td => new { td.TrainId, td.ServiceDate }).IsUnique();

            entity.HasOne<Train>()
                .WithMany()
                .HasForeignKey(td => td.TrainId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }

    private static void ConfigureBooking(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Booking>(entity =>
        {
            // Segment is a computed convenience wrapper over OriginSequence/
            // DestinationSequence for use in C# - it has no column of its
            // own. The actual range column the database uses for the
            // exclusion constraint ("segment", int4range) is added via raw
            // SQL in a later migration as a GENERATED column derived from
            // OriginSequence/DestinationSequence, so EF never needs to read
            // or write it.
            entity.Ignore(b => b.Segment);

            entity.Property(b => b.PassengerName).IsRequired().HasMaxLength(200);
            entity.Property(b => b.FareAmount).HasColumnType("numeric(10,2)");
            entity.Property(b => b.Status).HasConversion<string>().HasMaxLength(20);

            entity.HasOne<Seat>()
                .WithMany()
                .HasForeignKey(b => b.SeatId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne<TripDeparture>()
                .WithMany()
                .HasForeignKey(b => b.TripDepartureId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne<Station>()
                .WithMany()
                .HasForeignKey(b => b.OriginStationId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne<Station>()
                .WithMany()
                .HasForeignKey(b => b.DestinationStationId)
                .OnDelete(DeleteBehavior.Restrict);

            // Application-level lookup for availability checks (all bookings
            // for a given seat on a given trip). The real correctness
            // guarantee against concurrent overlapping inserts is the
            // Postgres exclusion constraint added in a later migration, not
            // this index - this just makes the common read fast.
            entity.HasIndex(b => new { b.SeatId, b.TripDepartureId });
        });
    }
}
