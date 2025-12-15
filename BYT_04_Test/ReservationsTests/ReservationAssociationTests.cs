using System;
using System.IO;
using System.Linq;
using BYT_04;
using BYT_04_Test.TestUtils;
using BYT_04.Reservations;
using NUnit.Framework;

namespace BYT_04_Test.ReservationsTests;

[TestFixture]
public class ReservationAssociationTests
{
    private Customer _c1;
    private Customer _c2;

    [SetUp]
    public void SetUp()
    {
        ClearList.ClearStaticList<Reservation>("_reservations");
        ClearList.ClearStaticList<Customer>("_customers");

        _c1 = new Customer(
            "One",
            null,
            "Customer",
            DateTime.Today.AddYears(-30),
            "X",
            "000",
            "one@example.com",
            new Address("1", "City", "S", "00000", "C"),
            false,
            0
        );

        _c2 = new Customer(
            "Two",
            null,
            "Customer",
            DateTime.Today.AddYears(-25),
            "Y",
            "111",
            "two@example.com",
            new Address("2", "City", "S", "11111", "C"),
            false,
            0
        );
    }

    [TearDown]
    public void TearDown()
    {
        ClearList.ClearStaticList<Reservation>("_reservations");
        ClearList.ClearStaticList<Customer>("_customers");
    }

    [Test]
    public void ReservationInitialization_AssociatesWithCustomer()
    {
        var res = _c1.CreateReservation(
            1,
            DateTime.Today,
            DateTime.Today.AddDays(1),
            ReservationStatus.Pending,
            10m
        );

        Assert.Multiple(() =>
        {
            Assert.That(res.Customer, Is.EqualTo(_c1));
            Assert.That(_c1.Reservations.Contains(res), Is.True);
            Assert.That(Reservation.Reservations.Contains(res), Is.True);
        });
    }

    [Test]
    public void Reservation_CannotBeSharedBetweenCustomers()
    {
        var res = _c1.CreateReservation(
            2,
            DateTime.Today,
            DateTime.Today.AddDays(2),
            ReservationStatus.Pending,
            20m
        );

        Assert.Throws<InvalidOperationException>(() => _c2.AddReservation(res));
    }

    [Test]
    public void RemovingCustomer_CascadesDeleteReservations()
    {
        var r1 = _c1.CreateReservation(
            3,
            DateTime.Today,
            DateTime.Today.AddDays(3),
            ReservationStatus.Pending,
            30m
        );
        var r2 = _c1.CreateReservation(
            4,
            DateTime.Today,
            DateTime.Today.AddDays(4),
            ReservationStatus.Pending,
            40m
        );

        // Pre-check
        Assert.That(Reservation.Reservations.Count >= 2);

        // Act
        Customer.Remove(_c1);

        // After removal, reservations created by _c1 should be removed from Reservation extent
        Assert.Multiple(() =>
        {
            Assert.That(Reservation.Reservations.Contains(r1), Is.False);
            Assert.That(Reservation.Reservations.Contains(r2), Is.False);
            // _c1 should no longer be in customer extent
            Assert.That(Customer.Customers.Contains(_c1), Is.False);
        });
    }
}
