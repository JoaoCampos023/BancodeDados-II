using System.Diagnostics;
using Banco_II.Data;
using Banco_II.Models;
using Banco_II.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Banco_II.Controllers
{
    public class HomeController : Controller
    {
        // permite fazer logs
        private readonly ILogger<HomeController> _logger;
        private readonly IStudentRepository _studentRepository;
        private readonly ICourseRepository _courseRepository;
        private readonly IStudentCoursesRepository _studentCoursesRepository;

        public HomeController(
            ILogger<HomeController> logger,
            IStudentRepository studentRepository,
            ICourseRepository courseRepository,
            IStudentCoursesRepository studentCoursesRepository
        )
        {
            _logger = logger;
            _studentRepository = studentRepository;
            _courseRepository = courseRepository;
            _studentCoursesRepository = studentCoursesRepository;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _studentRepository.GetAll());
        }

        // COURSES - Lista de cursos
        public async Task<IActionResult> Courses()
        {
            return View(await _courseRepository.GetAll());
        }

        // STUDENT COURSES - Matr�culas
        public async Task<IActionResult> StudentCourses()
        {
            return View(await _studentCoursesRepository.GetAll());
        }

        // CREATE COURSE - GET
        [HttpGet]
        public IActionResult CreateCourse()
        {
            return View();
        }

        // CREATE COURSE - POST
        [HttpPost]
        public async Task<IActionResult> CreateCourse(Course course)
        {
            if (ModelState.IsValid)
            {
                await _courseRepository.Create(course);
                return RedirectToAction("Courses");
            }
            return View(course);
        }

        // EDIT COURSE - GET
        [HttpGet]
        public async Task<IActionResult> EditCourse(int id)
        {
            var course = await _courseRepository.GetById(id);
            if (course == null)
            {
                return NotFound();
            }
            return View(course);
        }

        // EDIT COURSE - POST
        [HttpPost]
        public async Task<IActionResult> EditCourse(Course course)
        {
            if (ModelState.IsValid)
            {
                await _courseRepository.Update(course);
                return RedirectToAction("Courses");
            }
            return View(course);
        }

        // DELETE COURSE - POST
        [HttpPost]
        public async Task<IActionResult> DeleteCourse(int id)
        {
            var course = await _courseRepository.GetById(id);
            if (course == null)
            {
                return NotFound();
            }
            await _courseRepository.Delete(course);
            return RedirectToAction("Courses");
        }

        // CREATE STUDENT COURSE - GET
        [HttpGet]
        public async Task<IActionResult> CreateStudentCourse()
        {
            try
            {
                var students = await _studentRepository.GetAll();
                var courses = await _courseRepository.GetAll();

                Console.WriteLine($"N�mero de estudantes: {students.Count()}");
                Console.WriteLine($"N�mero de cursos: {courses.Count()}");

                ViewBag.Students = new SelectList(students, "ID", "FullName");
                ViewBag.Courses = new SelectList(courses, "ID", "Name");

                return View();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro no CreateStudentCourse (GET): {ex.Message}");
                return View("Error");
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateStudentCourse(StudentCourses studentCourse)
        {
            try
            {
                Console.WriteLine($"Tentativa de matr�cula - StudentID: {studentCourse.StudentID}, CourseID: {studentCourse.CourseID}");

                if (ModelState.IsValid)
                {
                    // Verifica se a rela��o j� existe
                    if (await _studentCoursesRepository.Exists(studentCourse.StudentID, studentCourse.CourseID))
                    {
                        ModelState.AddModelError("", "Este estudante j� est� matriculado neste curso.");
                        Console.WriteLine("Matr�cula j� existe");
                    }
                    else
                    {
                        await _studentCoursesRepository.Create(studentCourse);
                        Console.WriteLine("Matr�cula criada com sucesso");
                        return RedirectToAction("StudentCourses");
                    }
                }
                else
                {
                    Console.WriteLine("ModelState inv�lido");
                    foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                    {
                        Console.WriteLine($"Erro: {error.ErrorMessage}");
                    }
                }

                // Recarregar as listas para o ViewBag
                var students = await _studentRepository.GetAll();
                var courses = await _courseRepository.GetAll();

                ViewBag.Students = new SelectList(students, "ID", "FullName", studentCourse.StudentID);
                ViewBag.Courses = new SelectList(courses, "ID", "Name", studentCourse.CourseID);

                return View(studentCourse);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro no CreateStudentCourse (POST): {ex.Message}");
                return View("Error");
            }
        }

        // DELETE STUDENT COURSE - POST
        [HttpPost]
        public async Task<IActionResult> DeleteStudentCourse(int studentId, int courseId)
        {
            var studentCourse = await _studentCoursesRepository.GetById(studentId, courseId);
            if (studentCourse == null)
            {
                return NotFound();
            }
            await _studentCoursesRepository.Delete(studentCourse);
            return RedirectToAction("StudentCourses");
        }


        [HttpPost]
        public async Task<IActionResult> Create(Student student)
        {
            if (ModelState.IsValid)
            {
                await _studentRepository.Create(student);
                return RedirectToAction("Index");
            }
            return View(student);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var student = await _studentRepository.GetById(id)!;
            if (student == null)
            {
                return NotFound();
            }
            await _studentRepository.Delete(student);
            return RedirectToAction("Index");
        }

        [HttpGet] // M�todo GET para exibir o formul�rio de edi��o
        public async Task<IActionResult> Edit(int id)
        {
            var student = await _studentRepository.GetById(id);
            if (student == null)
            {
                return NotFound();
            }
            return View(student);
        }

        [HttpPost] // M�todo POST para processar a atualiza��o
        public async Task<IActionResult> Edit(Student student)
        {
            if (ModelState.IsValid)
            {
                await _studentRepository.Update(student);
                return RedirectToAction("Index");
            }
            return View(student);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpGet]
        public async Task<IActionResult> CheckData()
        {
            var students = await _studentRepository.GetAll();
            var courses = await _courseRepository.GetAll();

            ViewBag.Students = students;
            ViewBag.Courses = courses;
            ViewBag.StudentCount = students.Count();
            ViewBag.CourseCount = courses.Count();

            return View();
        }
    }
}
