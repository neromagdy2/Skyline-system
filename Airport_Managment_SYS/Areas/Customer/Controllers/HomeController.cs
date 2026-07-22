using Airport_Managment_SYS.Areas.Customer.ViewModels;
using Airport_Managment_SYS.Repositories;
using Airport_Managment_SYS.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Tesseract;
namespace Airport_Managment_SYS.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize]
    public class HomeController : Controller
    {
        private readonly IRepository<GovernerateState> _governerateStateRepository;
        private readonly IRepository<Trip> _tripRepository;
        private readonly IRepository<SeatClass> _seatClassesRepository;
        private readonly IRepository<Reservation> _reservationRepository;
        private readonly IRepository<ReservationSeat> _reservationSeatRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbcontext _context;
        public HomeController(IRepository<GovernerateState> governerateStateRepository, IRepository<Trip> tripRepository, IRepository<SeatClass> seatClassesRepository, IRepository<Reservation> reservationRepository, IRepository<ReservationSeat> reservationSeatRepository, UserManager<ApplicationUser> userManager, ApplicationDbcontext context)
        {
            _governerateStateRepository = governerateStateRepository;
            _tripRepository = tripRepository;
            _seatClassesRepository = seatClassesRepository;
            _reservationRepository = reservationRepository;
            _reservationSeatRepository = reservationSeatRepository;
            _userManager = userManager;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            var reservations = (await _reservationRepository.GetAsync(
                r => r.ApplicationUserId == user.Id,
                includeFunc: q => q
                    .Include(r => r.Trip)
                        .ThenInclude(t => t.Airport_From).ThenInclude(g => g.GovernerateState)
                    .Include(r => r.Trip)
                        .ThenInclude(t => t.Airport_To).ThenInclude(g=>g.GovernerateState),
                trackd: false
            )).ToList();
            var trips = (await _tripRepository.GetAsync(
                includeFunc: q => q
                    .Include(t => t.Airport_To)
                    .ThenInclude(a => a.GovernerateState)
            ))
            .Take(20)
            .OrderBy(x => Guid.NewGuid())
            .Take(3)
            .ToList();
            var seatClasses = await _seatClassesRepository.GetAsync();
            var governerateStates = await _governerateStateRepository.GetAsync();

            return View(new HomeVM()
            {
                SearchTripsVM = new SearchTripsVM()
                {
                    States = governerateStates,
                    seatClasses = seatClasses
                },
                trips = trips,
                reservations=reservations
            });
        }

        [HttpGet]
        public async Task<IActionResult> SearchTrips(HomeVM homeVM, int page = 1, int pageSize = 10)
        {
            if (!ModelState.IsValid)
            {
              homeVM.SearchTripsVM.seatClasses = await _seatClassesRepository.GetAsync();
                homeVM.SearchTripsVM.States = await _governerateStateRepository.GetAsync();
                return View("Index", homeVM);
            }
            homeVM.SearchTripsVM.States = (await _governerateStateRepository.GetAsync()).ToList();
           homeVM.SearchTripsVM.seatClasses = await _seatClassesRepository.GetAsync();

            var trips = await _tripRepository.GetAsync(
                t => t.Airport_FromId == homeVM.SearchTripsVM.DepartureCity &&
                     t.Airport_ToId == homeVM.SearchTripsVM.ArrivalCity &&
                     t.DateTime.Year == homeVM.SearchTripsVM.DepartureTime.Year&&t.DateTime.Month==homeVM.SearchTripsVM.DepartureTime.Month&& t.DateTime.Day == homeVM.SearchTripsVM.DepartureTime.Day,
                includeFunc: q => q
                    .Include(t => t.Airport_From)
                    .Include(t => t.Airport_To)
                    .Include(t => t.TripSeats)
                        .ThenInclude(ts => ts.Seat)
            );
            if (homeVM.SearchTripsVM.MaxPrice >0)
            {
                trips = trips.Where(t => t.Price < homeVM.SearchTripsVM.MaxPrice);

            }
            if (homeVM.SearchTripsVM.SeatClassIds != null && homeVM.SearchTripsVM.SeatClassIds.Any())
            {
                trips = trips.Where(t =>
                    t.TripSeats != null &&
                    t.TripSeats.Any(ts =>
                      
                        ts.Seat != null &&
                        homeVM.SearchTripsVM.SeatClassIds.Contains(ts.Seat.seatClassId)
                    ));
            }
            
            // Pagination
            int totalItems = trips.Count();
            int totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
            
            trips = trips.Skip((page - 1) * pageSize).Take(pageSize);
            
            homeVM.SearchTripsVM.trips = trips;
            homeVM.SearchTripsVM.CurrentPage = page;
            homeVM.SearchTripsVM.PageSize = pageSize;
            homeVM.SearchTripsVM.TotalItems = totalItems;
            homeVM.SearchTripsVM.TotalPages = totalPages;

            return View(homeVM);
        }


        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var trip = await _tripRepository.GetOneAsync(
                t => t.Id == id,
                includeFunc: q => q
                    .Include(t => t.TripSeats)
                        .ThenInclude(ts => ts.Seat)
                            .ThenInclude(s => s.SeatClass)
                    .Include(t => t.Airport_From)
                    .Include(t => t.Airport_To)
            );
            if (trip == null)
                return NotFound();

            var available = trip.TripSeats.Count(ts => !ts.IsBooked);
            var seatClasses = await _seatClassesRepository.GetAsync();
            var availableByClass = trip.TripSeats
                .Where(ts => !ts.IsBooked && ts.Seat != null)
                .GroupBy(ts => ts.Seat.seatClassId)
                .ToDictionary(g => g.Key, g => g.Count());

            return View(new DetailsTripVM
            {
                Trip = trip,
                AvailableSeats = available,
                SeatClasses = seatClasses,
                AvailableByClass = availableByClass
            });
        }
        //[HttpPost]
        //public IActionResult GetPassengerData(ReserveVM model)
        //{
           
        //    // هنا انت بس بتنقل المستخدم للخطوة التانية
        //    return View(model);
        //}
        //[HttpPost]
        //public async Task<IActionResult> ConfirmReservation(ReserveVM model, IFormFile passportImage)
        //{
        //    if (!ModelState.IsValid)
        //        return View("GetPassengerData", model);

        //    // 1. احفظ الصورة
        //    // 2. OCR
        //    // 3. Compare
        //    // 4. لو تمام → احفظ PassengerTicketData
        //    // Get current user
        //    var user = await _userManager.GetUserAsync(User);
        //    if (user == null)
        //    {
        //        return RedirectToAction("Login", "Authentication", new { area = "Identity" });
        //    }

        //    // ✅ 1. Validate image
        //    if (passportImage == null || passportImage.Length == 0)
        //        {
        //            ModelState.AddModelError("", "Please upload passport image");
        //            return View("GetPassengerData", model);
        //        }

        //        var extension = Path.GetExtension(passportImage.FileName).ToLower();
        //        if (extension != ".jpg" && extension != ".jpeg" && extension != ".png")
        //        {
        //            ModelState.AddModelError("", "Only JPG or PNG allowed");
        //            return View("GetPassengerData", model);
        //        }

        //        // ✅ 2. Save Image
        //        var imagePath = await SaveImage(passportImage);
        //        var fullPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", imagePath.TrimStart('/'));

        //        // ✅ 3. OCR
        //        var text = ExtractTextFromImage(fullPath);

        //        // ✅ 4. Extract MRZ
        //        var mrzLines = ExtractMRZ(text);

        //        if (mrzLines == null)
        //        {
        //            ModelState.AddModelError("", "Passport not clear, try again");
        //            return View("GetPassengerData", model);
        //        }

        //        var extracted = ParseMRZ(mrzLines);

        //        // ✅ 5. Compare
        //        var isValid = CompareData(model, extracted);

        //        if (!isValid)
        //        {
        //            ModelState.AddModelError("", "Data does not match passport");
        //            return View("GetPassengerData", model);
        //        }

        //        // ✅ 6. Save Passenger Data
        //        var passenger = new PassengerTicketData
        //        {
        //            PassengerName = model.PassengerName,
        //            PassengerNationality = model.PassengerNationality,
        //            PassengerPassportNumber = model.PassengerPassportNumber,
        //            PassportExpireDate = model.PassportExpireDate,
        //            PassengerImgUrl =imagePath,
        //            UserId = user.Id // حسب السيستم عندك
        //        };

        //        _context.PassengerTicketData.Add(passenger);
        //        await _context.SaveChangesAsync();

        //    // Get the trip with seats
        //    var trip = await _tripRepository.GetOneAsync(
        //        t => t.Id == model.TripId,
        //        includeFunc: q => q
        //            .Include(t => t.TripSeats)
        //                .ThenInclude(ts => ts.Seat)
        //    );

        //    if (trip == null)
        //    {
        //        TempData["Error"] = "Trip not found.";
        //        return RedirectToAction(nameof(Index));
        //    }

        //    // Validate that all requested seat IDs exist for this trip and are not booked
        //    var tripSeatIds = trip.TripSeats
        //        .Where(ts => ts.Seat != null && ts.Seat.seatClassId == model.SeatClassId)
        //        .Select(ts => ts.SeatId)
        //        .ToHashSet();

        //    // Check if all m seat IDs are valid for this trip and class
        //    var invalidSeats = model.SeatIds.Where(seatId => !tripSeatIds.Contains(seatId)).ToList();
        //    if (invalidSeats.Any())
        //    {
        //        TempData["Error"] = "Some selected seats are not available for this trip or seat class.";
        //        return RedirectToAction(nameof(Details), new { id = model.TripId });
        //    }

        //    // Check if any of the requested seats are already booked
        //    var bookedSeats = trip.TripSeats
        //        .Where(ts => model.SeatIds.Contains(ts.SeatId) && ts.IsBooked)
        //        .Select(ts => ts.SeatId)
        //        .ToList();

        //    if (bookedSeats.Any())
        //    {
        //        TempData["Error"] = "Some selected seats are already booked. Please select different seats.";
        //        return RedirectToAction(nameof(Details), new { id = model.TripId });
        //    }

        //    // Get current user
        //    if (user == null)
        //    {
        //        return RedirectToAction("Login", "Authentication", new { area = "Identity" });
        //    }

        //    // Calculate total price if not provided
        //    if (model.TotalPrice <= 0)
        //    {
        //        model.TotalPrice = (decimal)trip.Price * model.SeatIds.Count;
        //    }

        //    // Create reservation
        //    var reservation = new Reservation
        //    {
        //        TripId = model.TripId,
        //        NumSeats = model.SeatIds.Count,
        //        SeatClassId = model.SeatClassId,
        //        TotalPrice = model.TotalPrice,
        //        ApplicationUserId = user.Id,
        //        IsPaid = false
        //    };

        //    await _reservationRepository.AddAsync(reservation);
        //    await _reservationRepository.CommitAsync();

        //    // Create reservation seat entries
        //    foreach (var seatId in model.SeatIds)
        //    {
        //        var reservationSeat = new ReservationSeat
        //        {
        //            ReservationId = reservation.Id,
        //            SeatId = seatId
        //        };
        //        await _reservationSeatRepository.AddAsync(reservationSeat);
        //    }
        //    await _reservationSeatRepository.CommitAsync();

        //    // Redirect to payment with reservation ID
        //    return RedirectToAction("pay", "Payments", new { area = "Customer", id = reservation.Id });

        //}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reserve(ReserveVM reserveVM)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Invalid reservation data.";
                return RedirectToAction(nameof(Details), new { id = reserveVM.TripId });
            }

            // Get the trip with seats
            var trip = await _tripRepository.GetOneAsync(
                t => t.Id == reserveVM.TripId,
                includeFunc: q => q
                    .Include(t => t.TripSeats)
                        .ThenInclude(ts => ts.Seat)
            );
            
            if (trip == null)
            {
                TempData["Error"] = "Trip not found.";
                return RedirectToAction(nameof(Index));
            }

            // Validate that all requested seat IDs exist for this trip and are not booked
            var tripSeatIds = trip.TripSeats
                .Where(ts => ts.Seat != null && ts.Seat.seatClassId == reserveVM.SeatClassId)
                .Select(ts => ts.SeatId)
                .ToHashSet();

            // Check if all requested seat IDs are valid for this trip and class
            var invalidSeats = reserveVM.SeatIds.Where(seatId => !tripSeatIds.Contains(seatId)).ToList();
            if (invalidSeats.Any())
            {
                TempData["Error"] = "Some selected seats are not available for this trip or seat class.";
                return RedirectToAction(nameof(Details), new { id = reserveVM.TripId });
            }

            // Check if any of the requested seats are already booked
            var bookedSeats = trip.TripSeats
                .Where(ts => reserveVM.SeatIds.Contains(ts.SeatId) && ts.IsBooked)
                .Select(ts => ts.SeatId)
                .ToList();

            if (bookedSeats.Any())
            {
                TempData["Error"] = "Some selected seats are already booked. Please select different seats.";
                return RedirectToAction(nameof(Details), new { id = reserveVM.TripId });
            }

            // Get current user
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login", "Authentication", new { area = "Identity" });
            }

            // Calculate total price if not provided
            if (reserveVM.TotalPrice <= 0)
            {
                reserveVM.TotalPrice = (decimal)trip.Price * reserveVM.SeatIds.Count;
            }

            // Create reservation
            var reservation = new Reservation
            {
                TripId = reserveVM.TripId,
                NumSeats = reserveVM.SeatIds.Count,
                SeatClassId = reserveVM.SeatClassId,
                TotalPrice = reserveVM.TotalPrice,
                ApplicationUserId = user.Id,
                IsPaid = false
            };

            await _reservationRepository.AddAsync(reservation);
            await _reservationRepository.CommitAsync();

            // Create reservation seat entries
            foreach (var seatId in reserveVM.SeatIds)
            {
                var reservationSeat = new ReservationSeat
                {
                    ReservationId = reservation.Id,
                    SeatId = seatId
                };
                await _reservationSeatRepository.AddAsync(reservationSeat);
            }
            await _reservationSeatRepository.CommitAsync();

            // Redirect to payment with reservation ID
            return RedirectToAction("pay", "Payments", new { area = "Customer", id = reservation.Id });
        }



        // GET: /Customer/Home/Bookings
        [HttpGet]
        public async Task<IActionResult> Bookings()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login", "Authentication", new { area = "Identity" });
            }

            var reservations = (await _reservationRepository.GetAsync(
                r => r.ApplicationUserId == user.Id,
                includeFunc: q => q
                    .Include(r => r.Trip)
                        .ThenInclude(t => t.Airport_From)
                    .Include(r => r.Trip)
                        .ThenInclude(t => t.Airport_To),
                trackd: false
            )).ToList();

            return View(reservations);
        }
        [HttpGet]
        public async Task<IActionResult> GetQuestions(int? parentId)
        {
            var questions = await _context.ChatbotQuestions
                .Where(q => q.ChatbotQuestionId == parentId)
                .Select(q => new {
                    q.Id,
                    q.Question,
                    q.Answer
                })
                .ToListAsync();

            return Json(questions);
        }


        [HttpGet]
        public async Task<IActionResult> GetQuestionWithChildren(int id)
        {
            var question = await _context.ChatbotQuestions
                .Where(q => q.Id == id)
                .Select(q => new
                {
                    q.Id,
                    q.Question,
                    q.Answer,
                    Children = _context.ChatbotQuestions
                        .Where(c => c.ChatbotQuestionId == q.Id)
                        .Select(c => new {
                            c.Id,
                            c.Question
                        }).ToList()
                })
                .FirstOrDefaultAsync();

            return Json(question);
        }


        /// <summary>
  
        /// </summary>
        /// <param name="user"></param>
        /// <param name="ocr"></param>
        /// <returns></returns>
        /// الفنكشنز الخاصه بال tcr
    //    public bool CompareData(ReserveVM user,
    //(string Name, string Nationality, string PassportNumber, DateTime ExpiryDate) ocr)
    //    {
    //        // 🔹 Normalize Name
    //        var userName = user.PassengerName.Replace(" ", "").ToLower();
    //        var ocrName = ocr.Name.Replace("<", "").Replace(" ", "").ToLower();

    //        bool nameMatch = ocrName.Contains(userName) || userName.Contains(ocrName);

    //        // 🔹 Passport Number
    //        bool passportMatch = user.PassengerPassportNumber.Trim().ToUpper() ==
    //                             ocr.PassportNumber.Trim().ToUpper();

    //        // 🔹 Nationality (خلي بالك هنا)
    //        bool nationalityMatch = user.PassengerNationality.ToUpper().Contains(ocr.Nationality);

    //        // 🔹 Expiry Date
    //        bool expiryMatch = user.PassportExpireDate.Date == ocr.ExpiryDate.Date;

    //        return nameMatch && passportMatch && nationalityMatch && expiryMatch;
    //    }


    //    public string ExtractTextFromImage(string imagePath)
    //    {
    //        var tessDataPath = Path.Combine(Directory.GetCurrentDirectory(), "tessdata");

    //        using var engine = new TesseractEngine(tessDataPath, "eng", EngineMode.Default);
    //        using var img = Pix.LoadFromFile(imagePath);
    //        using var page = engine.Process(img);

    //        return page.GetText();
    //    }
    //    public string ExtractMRZ(string ocrText)
    //    {
    //        var lines = ocrText
    //            .Split('\n')
    //            .Select(l => l.Trim())
    //            .Where(l => l.Length > 30 && l.Contains("<"))
    //            .ToList();

    //        if (lines.Count >= 2)
    //        {
    //            return lines[^2] + "\n" + lines[^1];
    //        }

    //        return null;
    //    }
    //    public (string name, string nationality, string passportNumber, DateTime expiryDate) ParseMRZ(string mrz)
    //    {
    //        var lines = mrz.Split('\n');

    //        var line1 = lines[0];
    //        var line2 = lines[1];

    //        // Passport Number
    //        var passportNumber = line2.Substring(0, 9).Replace("<", "");

    //        // Nationality
    //        var nationality = line2.Substring(10, 3);

    //        // Expiry Date
    //        var expiryRaw = line2.Substring(21, 6);
    //        var expiryDate = DateTime.ParseExact(expiryRaw, "yyMMdd", null);

    //        // Name
    //        var namePart = line1.Substring(5);
    //        var names = namePart.Split("<<");

    //        var lastName = names[0].Replace("<", " ");
    //        var firstName = names.Length > 1 ? names[1].Replace("<", " ") : "";

    //        var fullName = $"{firstName} {lastName}".Trim();

    //        return (fullName, nationality, passportNumber, expiryDate);
    //    }
    //    public async Task<string> SaveImage(IFormFile file)
    //    {
    //        var fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
    //        var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/passports", fileName);

    //        using var stream = new FileStream(path, FileMode.Create);
    //        await file.CopyToAsync(stream);

    //        return "/passports/" + fileName;
    //    }
    }

}
