using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PRN232.LMS.Repositories.Entities;
using PRN232.LMS.Repositories.Repositories;
using PRN232.LMS.Services.Common;
using PRN232.LMS.Services.DTOs;
using PRN232.LMS.Services.Interfaces;

namespace PRN232.LMS.Services.Implementations
{
    public class StudentService : IStudentService
    {
        private readonly IGenericRepository<Student> _studentRepository;
        private readonly IGenericRepository<Enrollment> _enrollmentRepository;
        private readonly IGenericRepository<Course> _courseRepository;

        public StudentService(IGenericRepository<Student> studentRepository, IGenericRepository<Enrollment> enrollmentRepository, IGenericRepository<Course> courseRepository)
        {
            _studentRepository = studentRepository;
            _enrollmentRepository = enrollmentRepository;
            _courseRepository = courseRepository;
        }

        public async Task<PagedResult<StudentResponse>> GetAllStudentsAsync(string? search, string? sort, int page, int pageSize)
        {
            var query = _studentRepository.GetQueryable();

            // 1. Searching
            if (!string.IsNullOrWhiteSpace(search))
            {
                var lowerSearch = search.ToLower();
                query = query.Where(s => s.FullName.ToLower().Contains(lowerSearch) || s.Email.ToLower().Contains(lowerSearch));
            }

            // 2. Sorting
            if (!string.IsNullOrWhiteSpace(sort))
            {
                query = QueryHelper.ApplySorting(query, sort);
            }
            else
            {
                query = query.OrderBy(s => s.StudentId); // Default sort
            }

            // 3. Paging Calculations
            int totalItems = await query.CountAsync();
            int totalPages = (int)Math.Ceiling((double)totalItems / pageSize);
            
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 10;

            var items = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

            var mappedItems = items.Select(s =>
            {
                // map Entity -> BusinessModel
                var model = new PRN232.LMS.Services.BusinessModels.StudentModel
                {
                    StudentId = s.StudentId,
                    FullName = s.FullName,
                    Email = s.Email,
                    DateOfBirth = s.DateOfBirth
                };

                // map BusinessModel -> Response
                return new StudentResponse
                {
                    StudentId = model.StudentId,
                    FullName = model.FullName,
                    Email = model.Email,
                    DateOfBirth = model.DateOfBirth
                };
            }).ToList();

            return new PagedResult<StudentResponse>
            {
                Items = mappedItems,
                Pagination = new PaginationMetadata
                {
                    Page = page,
                    PageSize = pageSize,
                    TotalItems = totalItems,
                    TotalPages = totalPages
                }
            };
        }

        public async Task<StudentResponse?> GetStudentByIdAsync(int studentId)
        {
            var student = await _studentRepository.GetByIdAsync(studentId);
            if (student == null) return null;

            var studentModel = new PRN232.LMS.Services.BusinessModels.StudentModel
            {
                StudentId = student.StudentId,
                FullName = student.FullName,
                Email = student.Email,
                DateOfBirth = student.DateOfBirth
            };

            return new StudentResponse
            {
                StudentId = studentModel.StudentId,
                FullName = studentModel.FullName,
                Email = studentModel.Email,
                DateOfBirth = studentModel.DateOfBirth
            };
        }

        public async Task<StudentResponse> CreateStudentAsync(CreateStudentRequest request)
        {
            var student = new Student
            {
                FullName = request.FullName,
                Email = request.Email,
                DateOfBirth = request.DateOfBirth
            };

            await _studentRepository.AddAsync(student);
            await _studentRepository.SaveChangesAsync();

            var model = new PRN232.LMS.Services.BusinessModels.StudentModel
            {
                StudentId = student.StudentId,
                FullName = student.FullName,
                Email = student.Email,
                DateOfBirth = student.DateOfBirth
            };

            return new StudentResponse
            {
                StudentId = model.StudentId,
                FullName = model.FullName,
                Email = model.Email,
                DateOfBirth = model.DateOfBirth
            };
        }

        public async Task<StudentResponse?> UpdateStudentAsync(int studentId, UpdateStudentRequest request)
        {
            var student = await _studentRepository.GetByIdAsync(studentId);
            if (student == null) return null;

            student.FullName = request.FullName;
            student.Email = request.Email;
            student.DateOfBirth = request.DateOfBirth;

            _studentRepository.Update(student);
            await _studentRepository.SaveChangesAsync();

            var model = new PRN232.LMS.Services.BusinessModels.StudentModel
            {
                StudentId = student.StudentId,
                FullName = student.FullName,
                Email = student.Email,
                DateOfBirth = student.DateOfBirth
            };

            return new StudentResponse
            {
                StudentId = model.StudentId,
                FullName = model.FullName,
                Email = model.Email,
                DateOfBirth = model.DateOfBirth
            };
        }

        public async Task<bool> DeleteStudentAsync(int studentId)
        {
            var student = await _studentRepository.GetByIdAsync(studentId);
            if (student == null) return false;

            _studentRepository.Delete(student);
            await _studentRepository.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<EnrollmentResponse>?> GetEnrollmentsByStudentIdAsync(int studentId, string? expand)
        {
            var student = await _studentRepository.GetByIdAsync(studentId);
            if (student == null) return null;

            var query = _enrollmentRepository.GetQueryable().Where(e => e.StudentId == studentId);

            var expandList = expand?.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(e => e.Trim().ToLower()).ToList() ?? new List<string>();
            if (expandList.Contains("course"))
            {
                query = query.Include(e => e.Course);
            }

            var items = await query.ToListAsync();

            return items.Select(e =>
            {
                // Entity -> BusinessModel
                var enrollmentModel = new PRN232.LMS.Services.BusinessModels.EnrollmentModel
                {
                    EnrollmentId = e.EnrollmentId,
                    StudentId = e.StudentId,
                    CourseId = e.CourseId,
                    EnrollDate = e.EnrollDate,
                    Status = e.Status
                };

                var courseModel = e.Course != null ? new PRN232.LMS.Services.BusinessModels.CourseModel
                {
                    CourseId = e.Course.CourseId,
                    CourseName = e.Course.CourseName,
                    SemesterId = e.Course.SemesterId
                } : null;

                // BusinessModel -> Response
                return new EnrollmentResponse
                {
                    EnrollmentId = enrollmentModel.EnrollmentId,
                    StudentId = enrollmentModel.StudentId,
                    CourseId = enrollmentModel.CourseId,
                    EnrollDate = enrollmentModel.EnrollDate,
                    Status = enrollmentModel.Status,
                    Course = courseModel != null ? new CourseResponse { CourseId = courseModel.CourseId, CourseName = courseModel.CourseName, SemesterId = courseModel.SemesterId } : null
                };
            });
        }

        public async Task<IEnumerable<CourseResponse>?> GetCoursesByStudentIdAsync(int studentId)
        {
            var studentCheck = await _studentRepository.GetByIdAsync(studentId);
            if (studentCheck == null) return null;

            var enrollments = await _enrollmentRepository.GetQueryable().Where(e => e.StudentId == studentId).Include(e => e.Course).ToListAsync();
            var courses = enrollments.Where(e => e.Course != null).Select(e => e.Course!).Distinct().ToList();

            return courses.Select(c =>
            {
                var model = new PRN232.LMS.Services.BusinessModels.CourseModel
                {
                    CourseId = c.CourseId,
                    CourseName = c.CourseName,
                    SemesterId = c.SemesterId
                };

                return new CourseResponse { CourseId = model.CourseId, CourseName = model.CourseName, SemesterId = model.SemesterId };
            });
        }
    }
}

