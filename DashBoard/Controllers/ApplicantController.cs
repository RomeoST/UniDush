﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using AutoMapper;
using DashBoard.Attributes;
using DashBoard.BLL.Infrastructure;
using DashBoard.BLL.Interfaces;
using DashBoard.BLL.Services;
using DashBoard.Model.Models;
using DashBoard.Models;

namespace DashBoard.Controllers
{
    public class ApplicantController : Controller
    {
        private IApplicantService ApplicantService;
        private IUStructService UStructService;

        public ApplicantController(IApplicantService applicantService, IUStructService uStructService)
        {
            ApplicantService = applicantService;
            UStructService = uStructService;
        }

        // GET: Appicant
        public ActionResult Index()
        {
            return View();
        }
        
        [HttpGet]
        public ActionResult LoadApplicants(int count)
        {
            var applicants = Task.Run(() => ApplicantService.GetAll()).Result;

            ViewBag.IdName = "list-applicant";
            ViewBag.SizeList = "15";

            if (count < 1)
                return PartialView("_PartialDefaultList", new BaseFormModel[]{});

            if (count > applicants.Count())
                count = applicants.Count();

            applicants = applicants.Skip(applicants.Count() - count);
            var result = Mapper.Map<IEnumerable<Applicant>, IEnumerable<BaseFormModel>>(applicants);

            return PartialView("_PartialDefaultList", result);
        }

        public async Task<ActionResult> GetApplicantDetail(int id)
        {
            var applicant = await Task.Run(() => ApplicantService.Find(id));
            if (applicant == null)
                return Json(new {model = "failed", modelList = new[] {"Детальної інформації не знайдено!"}});

            var result = Mapper.Map<Applicant, ApplicantFormModel>(applicant);

            return Json(new {model = "confirmed", applicant = result}, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ValidateAjax]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SaveApplicant(ApplicantFormModel applicant)
        {
            var data = Mapper.Map<ApplicantFormModel, Applicant>(applicant);
            OperationDetails result;

            // Create or Edit applicant
            if (data.ApplicantId == 0)
                result = await ApplicantService.Create(data);
            else
                result = await ApplicantService.Edit(data);

            return Json(new {model = result.Successed?"confirmed":"failed", modelList = result.Message}, JsonRequestBehavior.AllowGet);
        }

        [ChildActionOnly]
        [OutputCache(Duration = 120, Location = OutputCacheLocation.Downstream)]
        public ActionResult GetDepartments()
        {
            var list = UStructService.FindDepartments(p => p.isAdmission == true).ToList();
            var tmp = Mapper.Map<IEnumerable<Department>, IEnumerable<BaseFormModel>>(list);
            ViewBag.IdName = "listDepart";
            return PartialView("_PartialDefaultComboBox", tmp);
        } 
    }
}