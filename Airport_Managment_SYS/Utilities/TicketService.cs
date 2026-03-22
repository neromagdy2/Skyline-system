using Airport_Managment_SYS.Models;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace Airport_Managment_SYS.Utilities
{
    public interface ITicketService
    {
        byte[] GenerateTicketPdf(Reservation reservation);
    }

    public class TicketService : ITicketService
    {
        public byte[] GenerateTicketPdf(Reservation reservation)
        {
            if (reservation == null)
                throw new ArgumentNullException(nameof(reservation));

            var trip = reservation.Trip;
            Console.WriteLine($"Generating ticket for reservation {reservation.Id}, trip {trip?.Id}");

            if (trip == null)
            {
                Console.WriteLine("Trip is null - cannot generate ticket");
                throw new InvalidOperationException("Trip data is required for ticket generation");
            }

            QuestPDF.Settings.License = LicenseType.Community;

            try
            {
                var pdfBytes = Document.Create(container =>
                {
                    container.Page(page =>
                    {
                        page.Size(PageSizes.A4);
                        page.Margin(2, Unit.Centimetre);
                        page.DefaultTextStyle(x => x.FontSize(10).FontFamily(Fonts.Calibri));

                        // ── Header ──────────────────────────────────────────────
                        page.Header().Row(row =>
                        {
                            row.RelativeItem().Column(column =>
                            {
                                column.Item().Text(txt =>
                                {
                                    txt.Span("SKYSTREAM").FontSize(20).Bold().FontColor("#3b82f6");
                                });
                                column.Item().Text(txt =>
                                {
                                    txt.Span("Flight Ticket").FontSize(16).FontColor("#666");
                                });
                            });
                        });

                        // ── Content ─────────────────────────────────────────────
                        page.Content().PaddingVertical(1, Unit.Centimetre).Column(column =>
                        {
                            // Flight Information Section
                            column.Item().PaddingBottom(10)
                                .Background("#f8f9fa")
                                .Border(1).BorderColor("#dee2e6")
                                .Padding(15)
                                .Column(contentColumn =>
                                {
                                    contentColumn.Item().Text(txt =>
                                    {
                                        txt.Span("Flight Information").FontSize(14).Bold().FontColor("#333");
                                    });
                                    contentColumn.Item().LineHorizontal(1).LineColor("#dee2e6");

                                    contentColumn.Item().PaddingTop(8).Row(row =>
                                    {
                                        row.RelativeItem().Column(col =>
                                        {
                                            col.Item().Text(txt => txt.Span("Reservation ID:").FontSize(10).FontColor("#666"));
                                            col.Item().Text(txt => txt.Span($"#{reservation.Id:D6}").FontSize(12).Bold());
                                        });

                                        row.RelativeItem().Column(col =>
                                        {
                                            col.Item().Text(txt => txt.Span("Date of Booking:").FontSize(10).FontColor("#666"));
                                            col.Item().Text(txt => txt.Span(DateTime.Now.ToString("MMMM dd, yyyy")).FontSize(12).Bold());
                                        });
                                    });
                                });

                            // Flight Details Section
                            column.Item().PaddingBottom(10)
                                .Background("#ffffff")
                                .Border(1).BorderColor("#dee2e6")
                                .Padding(15)
                                .Column(contentColumn =>
                                {
                                    contentColumn.Item().Text(txt =>
                                    {
                                        txt.Span("Flight Details").FontSize(14).Bold().FontColor("#333");
                                    });
                                    contentColumn.Item().LineHorizontal(1).LineColor("#dee2e6");

                                    contentColumn.Item().PaddingTop(8).Row(row =>
                                    {
                                        row.RelativeItem().Column(col =>
                                        {
                                            col.Item().Text(txt => txt.Span("From:").FontSize(10).FontColor("#666"));
                                            col.Item().Text(txt => txt.Span(trip?.Airport_From?.Name ?? "N/A").FontSize(12).Bold());
                                        });

                                        row.RelativeItem().Column(col =>
                                        {
                                            col.Item().Text(txt => txt.Span("To:").FontSize(10).FontColor("#666"));
                                            col.Item().Text(txt => txt.Span(trip?.Airport_To?.Name ?? "N/A").FontSize(12).Bold());
                                        });
                                    });

                                    contentColumn.Item().PaddingTop(8).Row(row =>
                                    {
                                        row.RelativeItem().Column(col =>
                                        {
                                            col.Item().Text(txt => txt.Span("Departure:").FontSize(10).FontColor("#666"));
                                            col.Item().Text(txt => txt.Span(trip?.DateTime.ToString("MMM dd, yyyy HH:mm") ?? "N/A").FontSize(12).Bold());
                                        });

                                        row.RelativeItem().Column(col =>
                                        {
                                            col.Item().Text(txt => txt.Span("Arrival:").FontSize(10).FontColor("#666"));
                                            col.Item().Text(txt => txt.Span(trip?.ArrivalDateTime.ToString("MMM dd, yyyy HH:mm") ?? "N/A").FontSize(12).Bold());
                                        });
                                    });
                                    contentColumn.Item().PaddingTop(8).Row(row =>
                                    {
                                        row.RelativeItem().Column(col =>
                                        {
                                            col.Item().Text(txt => txt.Span("Plane Id:").FontSize(10).FontColor("#666"));
                                            col.Item().Text(txt => txt.Span(trip?.AirplaneId.ToString() ?? "N/A").FontSize(12).Bold());
                                        });

                                        row.RelativeItem().Column(col =>
                                        {
                                            col.Item().Text(txt => txt.Span("Plane Name:").FontSize(10).FontColor("#666"));
                                            col.Item().Text(txt => txt.Span(trip?.Airplane?.Name ?? "N/A").FontSize(12).Bold());
                                        });
                                    });
                                });

                            // Passenger & Seat Information Section
                            column.Item().PaddingBottom(10)
                                .Background("#ffffff")
                                .Border(1).BorderColor("#dee2e6")
                                .Padding(15)
                                .Column(contentColumn =>
                                {
                                    contentColumn.Item().Text(txt =>
                                    {
                                        txt.Span("Passenger & Seat Information").FontSize(14).Bold().FontColor("#333");
                                    });
                                    contentColumn.Item().LineHorizontal(1).LineColor("#dee2e6");

                                    if (reservation.ReservationSeats?.Any() == true)
                                    {
                                        int seatNumber = 1;
                                        foreach (var reservationSeat in reservation.ReservationSeats)
                                        {
                                            contentColumn.Item().PaddingTop(8).Row(row =>
                                            {
                                                row.RelativeItem().Column(col =>
                                                {
                                                    col.Item().Text(txt => txt.Span($" Seat: {seatNumber}").FontSize(10).FontColor("#666"));

                                                    col.Item().Row(row1 => {

                                                        row1.RelativeItem().Column(col1 =>
                                                        {
                                                            var seatText = $"Number: {reservationSeat.Seat?.SeatNumber}";
                                                            col1.Item().Text(txt => txt.Span(seatText).FontSize(12).Bold());
                                                        });
                                                        row1.RelativeItem().Column(col1 =>
                                                        {
                                                            var seatClassText = $"Class: {reservationSeat.Seat?.SeatClass?.Name ?? "N/A"}";
                                                            col1.Item().Text(txt => txt.Span(seatClassText).FontSize(12).Bold());
                                                        });

                                                    });
                                                });
                                            });
                                            seatNumber++;
                                        }
                                    }
                                    else
                                    {
                                        contentColumn.Item().PaddingTop(8).Text(txt =>
                                        {
                                            txt.Span("No seat information available").FontSize(10).FontColor("#666");
                                        });
                                    }
                                });

                            // Price Information Section
                            column.Item()
                                .Background("#e7f3ff")
                                .Border(1).BorderColor("#3b82f6")
                                .Padding(15)
                                .Column(contentColumn =>
                                {
                                    contentColumn.Item().Text(txt =>
                                    {
                                        txt.Span("Price Information").FontSize(14).Bold().FontColor("#333");
                                    });
                                    contentColumn.Item().LineHorizontal(1).LineColor("#3b82f6");

                                    contentColumn.Item().PaddingTop(8).Row(row =>
                                    {
                                        row.RelativeItem().Column(col =>
                                        {
                                            col.Item().Text(txt => txt.Span("Total Amount:").FontSize(10).FontColor("#666"));
                                            col.Item().Text(txt => txt.Span($"{reservation.TotalPrice:0.00} EGP").FontSize(16).Bold().FontColor("#3b82f6"));
                                        });
                                    });

                                    contentColumn.Item().PaddingTop(8).Row(row =>
                                    {
                                        row.RelativeItem().Column(col =>
                                        {
                                            col.Item().Text(txt => txt.Span("Payment Status:").FontSize(10).FontColor("#666"));
                                            col.Item().Text(txt =>
                                            {
                                                txt.Span(reservation.IsPaid ? "PAID" : "PENDING")
                                                   .FontSize(12).Bold()
                                                   .FontColor(reservation.IsPaid ? "#10b981" : "#f59e0b");
                                            });
                                        });
                                    });
                                });
                        });

                        // ── Footer ──────────────────────────────────────────────
                        page.Footer().AlignCenter().Text(txt =>
                        {
                            txt.Span("Thank you for choosing SKYSTREAM! ").FontSize(9);
                            txt.Span("For support, contact us at support@skystream.com").FontSize(9).FontColor("#3b82f6");
                        });
                    });
                })
                .GeneratePdf();

                Console.WriteLine($"PDF generated successfully, size: {pdfBytes.Length} bytes");
                return pdfBytes;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"PDF generation failed: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                throw;
            }
        }
    }
}