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
        public async Task<IActionResult> Courses()
        {
            return View(await _courseRepository.GetAll());
        }
        public async Task<IActionResult> StudentCourses()
        {
            return View(await _studentCoursesRepository.GetAll());
        }

        [HttpGet]
        public IActionResult CreateCourse()
        {
            return View();
        }

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

                Console.WriteLine($"Número de estudantes: {students.Count()}");
                Console.WriteLine($"Número de cursos: {courses.Count()}");

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
                Console.WriteLine($"Tentativa de matrícula - StudentID: {studentCourse.StudentID}, CourseID: {studentCourse.CourseID}");

                if (ModelState.IsValid)
                {
                    // Verifica se a relação já existe
                    if (await _studentCoursesRepository.Exists(studentCourse.StudentID, studentCourse.CourseID))
                    {
                        // Obter informações do estudante e curso para a mensagem
                        var student = await _studentRepository.GetById(studentCourse.StudentID);
                        var course = await _courseRepository.GetById(studentCourse.CourseID);

                        ModelState.AddModelError("", $"O aluno {student?.FullName} já está matriculado no curso {course?.Name}.");
                        Console.WriteLine("Matrícula já existe");
                    }
                    else
                    {
                        await _studentCoursesRepository.Create(studentCourse);
                        Console.WriteLine("Matrícula criada com sucesso");
                        return RedirectToAction("StudentCourses");
                    }
                }
                else
                {
                    Console.WriteLine("ModelState inválido");
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

        // LISTA DE MATÉRIAS DE UM CURSO
        // CREATE SUBJECT - GET
        // LISTA DE MATÉRIAS DE UM CURSO
        // NOVA ACTION: Lista todas as matérias

        public async Task<IActionResult> Subjects(int courseId)
        {
            try
            {
                Console.WriteLine($"=== TENTANDO CARREGAR SUBJECTS ===");
                Console.WriteLine($"CourseID recebido: {courseId}");

                var course = await _courseRepository.GetById(courseId);
                if (course == null)
                {
                    Console.WriteLine($"Curso com ID {courseId} não encontrado");
                    return NotFound();
                }

                ViewBag.Course = course;
                var subjects = await _subjectRepository.GetSubjectsByCourseId(courseId);

                Console.WriteLine($"Matérias encontradas: {subjects.Count()}");
                return View(subjects);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro em Subjects: {ex.Message}");
                return View("Error");
            }
        }

        public async Task<IActionResult> AllSubjects()
        {
            var subjects = await _subjectRepository.GetAllWithCourses();
            return View(subjects);
        }

        // NOVA ACTION: Criar matéria (versão independente)
        [HttpGet]
        public async Task<IActionResult> CreateIndependentSubject()
        {
            try
            {
                var courses = await _courseRepository.GetAll();
                ViewBag.Courses = new SelectList(courses, "ID", "Name");
                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao carregar página de criação de matéria independente");
                return View("Error");
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateIndependentSubject(Subject subject)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    // Verificar se o curso existe
                    var course = await _courseRepository.GetById(subject.CourseID);
                    if (course == null)
                    {
                        ModelState.AddModelError("CourseID", "Curso selecionado não existe");
                    }
                    else
                    {
                        // Verificar se já existe matéria com mesmo nome no mesmo curso
                        var existingSubject = (await _subjectRepository.GetAll())
                            .FirstOrDefault(s => s.Name == subject.Name && s.CourseID == subject.CourseID);

                        if (existingSubject != null)
                        {
                            ModelState.AddModelError("Name", $"Já existe uma matéria com o nome '{subject.Name}' neste curso.");
                        }
                        else
                        {
                            await _subjectRepository.Create(subject);
                            TempData["Success"] = $"Matéria '{subject.Name}' criada com sucesso para o curso '{course.Name}'!";
                            return RedirectToAction("AllSubjects");
                        }
                    }
                }

                // Recarregar ViewBag em caso de erro
                var courses = await _courseRepository.GetAll();
                ViewBag.Courses = new SelectList(courses, "ID", "Name", subject.CourseID);
                return View(subject);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao criar matéria independente");
                TempData["Error"] = $"Erro ao criar matéria: {ex.Message}";

                var courses = await _courseRepository.GetAll();
                ViewBag.Courses = new SelectList(courses, "ID", "Name", subject.CourseID);
                return View(subject);
            }
        }

        // NOVA ACTION: Editar matéria (versão independente)
        [HttpGet]
        public async Task<IActionResult> EditIndependentSubject(int id)
        {
            var subject = await _subjectRepository.GetById(id);
            if (subject == null)
            {
                return NotFound();
            }

            var courses = await _courseRepository.GetAll();
            ViewBag.Courses = new SelectList(courses, "ID", "Name", subject.CourseID);

            return View(subject);
        }

        [HttpPost]
        public async Task<IActionResult> EditIndependentSubject(Subject subject)
        {
            if (ModelState.IsValid)
            {
                await _subjectRepository.Update(subject);
                TempData["Success"] = "Matéria atualizada com sucesso!";
                return RedirectToAction("AllSubjects");
            }

            var courses = await _courseRepository.GetAll();
            ViewBag.Courses = new SelectList(courses, "ID", "Name", subject.CourseID);
            return View(subject);
        }

        // NOVA ACTION: Deletar matéria (versão independente)
        [HttpPost]
        public async Task<IActionResult> DeleteIndependentSubject(int id)
        {
            var subject = await _subjectRepository.GetById(id);
            if (subject == null)
            {
                return NotFound();
            }

            await _subjectRepository.Delete(subject);
            TempData["Success"] = "Matéria deletada com sucesso!";
            return RedirectToAction("AllSubjects");
        }

        // Método para debug - verificar todas as matérias no sistema
        [HttpGet]
        public async Task<IActionResult> DebugAllSubjects()
        {
            try
            {
                var allSubjects = await _subjectRepository.GetAll();
                var allCourses = await _courseRepository.GetAll();

                ViewBag.AllSubjects = allSubjects;
                ViewBag.AllCourses = allCourses;

                Console.WriteLine("=== DEBUG: Todas as Matérias ===");
                foreach (var subject in allSubjects)
                {
                    Console.WriteLine($"Matéria: {subject.Name} | CursoID: {subject.CourseID} | Curso: {subject.Course?.Name}");
                }

                Console.WriteLine("=== DEBUG: Todos os Cursos ===");
                foreach (var course in allCourses)
                {
                    var courseSubjects = await _subjectRepository.GetSubjectsByCourseId(course.ID);
                    Console.WriteLine($"Curso: {course.Name} (ID: {course.ID}) | Matérias: {courseSubjects.Count()}");
                }

                return View();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro no debug: {ex.Message}");
                return Content($"Erro no debug: {ex.Message}");
            }
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

        [HttpGet] // Método GET para exibir o formulário de edição
        public async Task<IActionResult> Edit(int id)
        {
            var student = await _studentRepository.GetById(id);
            if (student == null)
            {
                return NotFound();
            }
            return View(student);
        }

        [HttpPost] // Método POST para processar a atualização
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

        [HttpGet]
        public async Task<ContentResult> DebugCourseStructure()
        {
            var courses = await _courseRepository.GetAll();
            var result = "=== ESTRUTURA DOS CURSOS ===\n\n";

            foreach (var course in courses)
            {
                result += $"CURSO: {course.Name} (ID: {course.ID})\n";
                result += $"Matérias no objeto Course: {course.Subjects?.Count ?? 0}\n";

                if (course.Subjects != null && course.Subjects.Any())
                {
                    foreach (var subject in course.Subjects)
                    {
                        result += $"  - {subject.Name} (ID: {subject.ID})\n";
                    }
                }
                else
                {
                    result += "  - Nenhuma matéria carregada\n";
                }
                result += "\n";
            }

            return Content(result, "text/plain");
        }

        [HttpGet]
        public async Task<ContentResult> DebugCoursesAndSubjects()
        {
            try
            {
                var courses = await _courseRepository.GetAll();
                var allSubjects = await _subjectRepository.GetAll();

                var result = "=== DEBUG: CURSOS E MATÉRIAS ===\n\n";

                foreach (var course in courses)
                {
                    result += $"CURSO: {course.Name} (ID: {course.ID})\n";

                    // Matérias do repositório
                    var subjectsFromRepo = await _subjectRepository.GetSubjectsByCourseId(course.ID);
                    result += $"Matérias no repositório: {subjectsFromRepo.Count()}\n";

                    foreach (var subject in subjectsFromRepo)
                    {
                        result += $"  - {subject.Name} (ID: {subject.ID})\n";
                    }

                    result += "\n";
                }

                result += "=== TODAS AS MATÉRIAS NO SISTEMA ===\n";
                foreach (var subject in allSubjects)
                {
                    var subjectCourse = await _courseRepository.GetById(subject.CourseID);
                    result += $"Matéria: {subject.Name} | CursoID: {subject.CourseID} | Curso: {subjectCourse?.Name ?? "NÃO ENCONTRADO"}\n";
                }

                return Content(result, "text/plain");
            }
            catch (Exception ex)
            {
                return Content($"Erro no debug: {ex.Message}", "text/plain");
            }
        }

        [HttpPost]
        public async Task<IActionResult> CleanOrphanedSubjects()
        {
            try
            {
                // Encontrar matérias sem curso válido
                var allSubjects = await _subjectRepository.GetAll();
                var allCourses = await _courseRepository.GetAll();
                var courseIds = allCourses.Select(c => c.ID).ToList();

                var orphanedSubjects = allSubjects.Where(s => !courseIds.Contains(s.CourseID)).ToList();

                // Deletar matérias órfãs
                foreach (var subject in orphanedSubjects)
                {
                    await _subjectRepository.Delete(subject);
                }

                TempData["Message"] = $"Foram removidas {orphanedSubjects.Count} matérias órfãs.";
                Console.WriteLine($"Matérias órfãs removidas: {orphanedSubjects.Count}");
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Erro ao limpar matérias: {ex.Message}";
                Console.WriteLine($"Erro ao limpar matérias órfãs: {ex.Message}");
            }

            return RedirectToAction("Courses");
        }

        [HttpGet]
        public async Task<ContentResult> DebugCourseSubjects()
        {
            var courses = await _courseRepository.GetAll();
            var result = "=== DEBUG: CURSOS E SUAS MATÉRIAS ===\n\n";

            foreach (var course in courses)
            {
                result += $"Curso: {course.Name} (ID: {course.ID})\n";
                result += $"Matérias carregadas: {course.Subjects?.Count ?? 0}\n";

                if (course.Subjects != null && course.Subjects.Any())
                {
                    foreach (var subject in course.Subjects)
                    {
                        result += $"  - {subject.Name}\n";
                    }
                }
                else
                {
                    result += "  - NENHUMA MATÉRIA CARREGADA\n";
                }
                result += "\n";
            }

            return Content(result, "text/plain");
        }
    }
}
