using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DJRM.Common.Infrastructure.Repository;
using DJRM.Common.Infrastructure.Validation;
using DJRM.Configuration.Model;

namespace Configuration.Controllers
{
    public class TaskController : DJRM.Common.Controllers.ControllerBase
    {
        private IUnitOfWork _uow;
        private IRepository<Task> _repository;
        private IValidator<Task> _validator;

        public TaskController(IUnitOfWork uow, IRepository<Task> repository, IValidator<Task> validator)
        {
            _uow = uow;
            _repository = repository;
            _validator = validator;
        }

        //
        // GET: /Task/

        public ViewResult Index()
        {
            return View(_repository.FindAll().ToList());
        }

        //
        // GET: /Task/Details/5

        public ViewResult Details(int id)
        {
            Task task = _repository.FindBy(id);
            return View(task);
        }

        //
        // GET: /Task/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /Task/Create

        [HttpPost]
        public ActionResult Create(Task task)
        {
            if (ModelState.IsValid)
            {
                _repository.Add(task);
                var validationResult = _validator.Validate(task);
                if (validationResult.Success)
                {
                    _uow.Commit();
                    return RedirectToAction("Index");
                }
                AddErrorsToModelState(validationResult.Errors);
            }

            return View(task);
        }

        //
        // GET: /Task/Edit/5

        public ActionResult Edit(int id)
        {
            Task task = _repository.FindBy(id);
            return View(task);
        }

        //
        // POST: /Task/Edit/5

        [HttpPost]
        public ActionResult Edit(int id, FormCollection formData)
        {
            Task task = _repository.FindBy(id);
            if (TryUpdateModel(task))
            {
                _repository.Save(task);
                var validationResult = _validator.Validate(task);
                if (validationResult.Success)
                {
                    _uow.Commit();
                    return RedirectToAction("Index");
                }
                AddErrorsToModelState(validationResult.Errors);
            }

            return View(task);
        }

        //
        // GET: /Task/Delete/5

        public ActionResult Delete(int id)
        {
            Task task = _repository.FindBy(id);
            return View(task);
        }

        //
        // POST: /Task/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            Task task = _repository.FindBy(id);
            var validationResult = _validator.CanBeDeleted(task);
            if (validationResult.Success)
            {
                _repository.Remove(task);
                _uow.Commit();
                return RedirectToAction("Index");
            }
            AddErrorsToModelState(validationResult.Errors);
            return View("Delete", task);
        }
    }
}