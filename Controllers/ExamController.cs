using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Rotativa.AspNetCore;
using trySupa.Models;
using Newtonsoft.Json;

namespace trySupa.Controllers
{
    public class ExamController : Controller
    {
        private readonly SupabaseService _supabaseService;

        public ExamController(SupabaseService supabaseService)
        {
            _supabaseService = supabaseService;
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            // fetch Departments and Classes from Supabase
            var departments = await _supabaseService.GetDepartments();
            var classes = await _supabaseService.GetClasses();

            ViewBag.Departments = departments.Select(d => d.DeptName).ToList();
            ViewBag.ClassNames = classes.Select(c => c.ClassName).ToList();

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> FetchSubjects([FromBody] SubjectRequest request)
        {
            // Log the received data to check
            Console.WriteLine($"Class: {request.ClassName}, Department: {request.Department}");

            if (string.IsNullOrEmpty(request.ClassName) || string.IsNullOrEmpty(request.Department))
            {
                return Json(new { success = false, message = "Class and Department must be selected." });
            }

            var allSubjects = await _supabaseService.GetSubYearDept(); // Get mappings
            var allSubjectsData = await _supabaseService.GetSubjects(); // Get subject names

            var classes = await _supabaseService.GetClasses();
            var depts = await _supabaseService.GetDepartments();

            var selectedClass = classes.FirstOrDefault(c => c.ClassName == request.ClassName);
            var selectedDept = depts.FirstOrDefault(d => d.DeptName == request.Department);

            if (selectedClass == null || selectedDept == null)
            {
                return Json(new { success = false, message = "Class or Department not found." });
            }

            var subjectIds = allSubjects
                .Where(x => x.ClassId == selectedClass.ClassId && x.DeptId == selectedDept.DeptId)
                .Select(x => x.SubjectId)
                .ToList();

            var subjectNames = allSubjectsData
                .Where(s => subjectIds.Contains(s.SubjectId))
                .Select(s => s.SubjectName)
                .ToList();

            return Json(new { success = true, subjects = subjectNames });
        }

        [HttpPost]
        public IActionResult Preview(ExamPreviewViewModel model)
        {
            model.IsPdf = false; // normal web view
            return View(model);
        }

        [HttpPost]
        public IActionResult DownloadPdf(ExamPreviewViewModel model)
        {
            model.IsPdf = true; // PDF view
            return new ViewAsPdf("Preview", model)
            {
                FileName = $"{model.ClassName}_{model.Department}_ExamTimetable.pdf",
                PageOrientation = Rotativa.AspNetCore.Options.Orientation.Portrait,
                PageSize = Rotativa.AspNetCore.Options.Size.A4
            };
        }

    }
}
