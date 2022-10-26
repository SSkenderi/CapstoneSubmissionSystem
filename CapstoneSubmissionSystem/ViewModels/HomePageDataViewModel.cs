using CapstoneSubmissionSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CapstoneSubmissionSystem.ViewModels
{
    public class HomePageDataViewModel
    {
        public User LoggedUser;
        public List<DocType> DocTypes;
        public List<User> Students;
        public List<Document> Documents;
    }
}