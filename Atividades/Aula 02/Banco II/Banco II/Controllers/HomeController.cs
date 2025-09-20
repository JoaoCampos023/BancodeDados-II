using System.Diagnostics;
using Banco_II.Data;
using Banco_II.Models;
using Banco_II.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Banco_II.Controllers
{
    public class HomeController : Controller
    {
        // permite fazer logs
        private readonly ILogger<HomeController> _logger;
        private readonly IStudentRepository _studentRepository;
        private readonly ICourseRepository _courseRepository;
        private readonly IStudentCoursesRepository _studentCoursesRepository;
        private readonly ISubjectRepository _subjectRepository;

        public HomeController(
            ILogger<HomeController> logger,
            IStudentRepository studentRepository,
            ICourseRepository courseRepository,
            IStudentCoursesRepository studentCoursesRepository,
            ISubjectRepository subjectRepository
        )
        {
            _logger = logger;
            _studentRepository = studentRepository;
            _courseRepository = courseRepository;
            _studentCoursesRepository = studentCoursesRepository;
            _subjectRepository = subjectRepository;
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
                        // Obter informa��es do estudante e curso para a mensagem
                        var student = await _studentRepository.GetById(studentCourse.StudentID);
                        var course = await _courseRepository.GetById(studentCourse.CourseID);

                        ModelState.AddModelError("", $"O aluno {student?.FullName} j� est� matriculado no curso {course?.Name}.");
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

        // LISTA DE MAT�RIAS DE UM CURSO
        public async Task<IActionResult> Subjects(int courseId)
        {
            var course = await _courseRepository.GetById(courseId);
            if (course == null)
            {
                return NotFound();
            }

            ViewBag.Course = course;
            var subjects = await _subjectRepository.GetSubjectsByCourseId(courseId);
            return View(subjects);
        }

        // CREATE SUBJECT - GET
        // LISTA DE MAT�RIAS DE UM CURSO
        public async Task<IActionResult> Subject(int courseId)
        {
            var course = await _courseRepository.GetById(courseId);
            if (course == null)
            {
                return NotFound();
            }

            ViewBag.Course = course;
            var subjects = await _subjectRepository.GetSubjectsByCourseId(courseId);
            return View(subjects);
        }

        // CREATE SUBJECT - POST
        [HttpPost]
        public async Task<IActionResult> CreateSubject(Subject subject)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _subjectRepository.Create(subject);
                    return RedirectToAction("Subjects", new { courseId = subject.CourseID });
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"Erro ao criar mat�ria: {ex.Message}");
                    _logger.LogError(ex, "Erro ao criar mat�ria");
                }
            }

            var course = await _courseRepository.GetById(subject.CourseID);
            ViewBag.Course = course;
            return View(subject);
        }

        // EDIT SUBJECT - GET
        [HttpGet]
        public async Task<IActionResult> EditSubject(int id)
        {
            var subject = await _subjectRepository.GetById(id);
            if (subject == null)
            {
                return NotFound();
            }

            return View(subject);
        }

        // EDIT SUBJECT - POST
        [HttpPost]
        public async Task<IActionResult> EditSubject(Subject subject)
        {
            if (ModelState.IsValid)
            {
                await _subjectRepository.Update(subject);
                return RedirectToAction("Subjects", new { courseId = subject.CourseID });
            }

            return View(subject);
        }

        // DELETE SUBJECT - POST
        [HttpPost]
        public async Task<IActionResult> DeleteSubject(int id)
        {
            var subject = await _subjectRepository.GetById(id);
            if (subject == null)
            {
                return NotFound();
            }

            await _subjectRepository.Delete(subject);
            return RedirectToAction("Subjects", new { courseId = subject.CourseID });
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
