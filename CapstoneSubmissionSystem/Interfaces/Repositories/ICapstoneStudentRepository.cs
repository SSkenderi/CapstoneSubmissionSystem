using CapstoneSubmissionSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CapstoneSubmissionSystem.Interfaces.Repositories
{
    public interface ICapstoneStudentRepository
    {
        List<Student> GetStudentById(int stuID);
        List<Faculty> GetFacultyById(int facID);
        List<Admin> GetAdminById(int admID);
    }
}