using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DJRM.Common.Infrastructure.Repository;
using DJRM.Common.Infrastructure.Validation;
using DJRM.Configuration.Model;

namespace DJRM.Configuration.Controllers
{
    public class CustomerController : DJRM.Common.Controllers.ControllerBase
    {
        private IUnitOfWork _uow;
        private IRepository<Customer> _repository;
        private IValidator<Customer> _validator;

        public CustomerController(IUnitOfWork uow, IRepository<Customer> repository, IValidator<Customer> validator)
        {
            _uow = uow;
            _repository = repository;
            _validator = validator;
        }

        //
        // GET: /Customer/

        public ViewResult Index()
        {
            return View(_repository.FindAll().ToList());
        }

        //
        // GET: /Customer/Details/5

        public ViewResult Details(int id)
        {
            Customer customer = _repository.FindBy(id);
            return View(customer);
        }

        //
        // GET: /Customer/Create

        public ActionResult Create()
        {
            return View();
        } 

        //
        // POST: /Customer/Create

        [HttpPost]
        public ActionResult Create(Customer customer)
        {
            if (ModelState.IsValid)
            {
                _repository.Add(customer);
                var validationResult = _validator.Validate(customer);
                if (validationResult.Success)
                {
                    _uow.Commit();
                    return RedirectToAction("Index");  
                }
                AddErrorsToModelState(validationResult.Errors);
            }

            return View(customer);
        }
        
        //
        // GET: /Customer/Edit/5
 
        public ActionResult Edit(int id)
        {
            Customer customer = _repository.FindBy(id);
            return View(customer);
        }

        //
        // POST: /Customer/Edit/5

        [HttpPost]
        public ActionResult Edit(int id, FormCollection formData)
        {
            Customer customer = _repository.FindBy(id);
            if (TryUpdateModel(customer))
            {
                _repository.Save(customer);
                var validationResult = _validator.Validate(customer);
                if (validationResult.Success)
                {
                    _uow.Commit();
                    return RedirectToAction("Index");
                }
                AddErrorsToModelState(validationResult.Errors);
            }

            return View(customer);
        }

        //
        // GET: /Customer/Delete/5
 
        public ActionResult Delete(int id)
        {
            Customer customer = _repository.FindBy(id);
            return View(customer);
        }

        //
        // POST: /Customer/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            Customer customer = _repository.FindBy(id);            
            var validationResult = _validator.CanBeDeleted(customer);
            if (validationResult.Success)
            {
                _repository.Remove(customer);
                _uow.Commit();
                return RedirectToAction("Index");
            }
            AddErrorsToModelState(validationResult.Errors);
            return View("Delete", customer);
        }

    }
}