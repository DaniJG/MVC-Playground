using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using System.Web.Mvc;
using System.Web.Security;
using DotNetOpenAuth.AspNet;
using Microsoft.Web.WebPages.OAuth;
using WebMatrix.WebData;
using DJRM.Common.Security.Model;
using DJRM.Common.Security.Repository;

namespace DJRM.Common.Controllers.Security
{
    [Authorize]
    [InitializeSimpleMembership]
    public class AccountController : Controller
    {

        [AllowAnonymous]
        public JsonResult UserNameIsUnique(string userName)
        {
            using (UsersContext db = new UsersContext())
            {
                bool existsUser = db.UserProfiles.Any(u => u.UserName.ToLower() == userName.ToLower());
                if (existsUser)
                {
                    string message = String.Format("{0} already exists. Please enter a different user name.", userName);
                    return Json(message, JsonRequestBehavior.AllowGet);
                }
            }

            return Json(true, JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public JsonResult EmailIsUnique(string email)
        {
            using (UsersContext db = new UsersContext())
            {
                bool existsEmail = db.UserProfiles.Any(u => u.Email.ToLower() == email.ToLower());
                if (existsEmail)
                {
                    return Json("There is already a user associated with this email.", JsonRequestBehavior.AllowGet);
                }
            }

            return Json(true, JsonRequestBehavior.AllowGet);
        }

        //
        // GET: /Account/Register

        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }

        //
        // POST: /Account/Register

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Register(RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                // Attempt to register the user
                try
                {
                    WebSecurity.CreateUserAndAccount(model.UserName, 
                                                     model.Password, 
                                                     new {Email = model.Email, 
                                                         FirstName = model.FirstName, 
                                                         LastName = model.LastName, 
                                                         Country = model.Country });

                    WebSecurity.Login(model.UserName, model.Password);

                    return RedirectToAction("Index", "Home");
                }
                catch (MembershipCreateUserException e)
                {
                    ModelState.AddModelError("", ErrorCodeToString(e.StatusCode));
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }
        

        //
        // GET: /Account/Manage

        public ActionResult Manage(ManageMessageId? message)
        {
            ViewBag.StatusMessage =
                message == ManageMessageId.ChangePasswordSuccess ? "Your password has been changed."
                : message == ManageMessageId.SetPasswordSuccess ? "Your password has been set."
                : message == ManageMessageId.RemoveLoginSuccess ? "The external login was removed."
                : "";
            ViewBag.HasLocalPassword = OAuthWebSecurity.HasLocalAccount(WebSecurity.GetUserId(User.Identity.Name));
            ViewBag.ReturnUrl = Url.Action("Manage");
            return View();
        }

        //
        // POST: /Account/Manage

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChangePassword(LocalPasswordModel model)
        {
            if (ModelState.IsValid)
            {
                // ChangePassword will throw an exception rather than return false in certain failure scenarios.
                bool changePasswordSucceeded;
                try
                {
                    changePasswordSucceeded = WebSecurity.ChangePassword(User.Identity.Name, model.OldPassword, model.NewPassword);
                }
                catch (Exception)
                {
                    changePasswordSucceeded = false;
                }

                if (!changePasswordSucceeded)
                {                    
                    ModelState.AddModelError("", "The current password is incorrect or the new password is invalid.");
                }
            }
                  

            // If we got this far, something failed, redisplay form
            return PartialView("_ChangePasswordPartial", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SetPassword(LocalPasswordModel model)
        {            
            // User does not have a local password so remove any validation errors caused by a missing
            // OldPassword field
            ModelState state = ModelState["OldPassword"];
            if (state != null)
            {
                state.Errors.Clear();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    WebSecurity.CreateAccount(User.Identity.Name, model.NewPassword);
                    return PartialView("_ChangePasswordPartial", model);
                }
                catch (Exception e)
                {
                    ModelState.AddModelError("", e);
                }
            }
            
            // If we got this far, something failed, redisplay form
            return PartialView("_SetPasswordPartial", model);
        }

        //
        // GET: /Account/Picture

        public ActionResult Picture(String id)
        {
            string userName = String.IsNullOrEmpty(id) ? User.Identity.Name : id;
            using (UsersContext db = new UsersContext())
            {
                var userProfile = db.UserProfiles.Single(u => u.UserName == userName);
                var model = new PictureModel { Email = userProfile.Email, PictureURL = userProfile.PictureURL };
                return PartialView("_Picture", model);
            }                        
        }

        //
        // GET: /Account/EditProfile

        public ActionResult EditProfile()
        {
            using (UsersContext db = new UsersContext())
            {
                var userProfile = db.UserProfiles.Single(u => u.UserName == User.Identity.Name);
                var model = new EditProfileModel { Email = userProfile.Email, 
                                                    FirstName = userProfile.FirstName, 
                                                    LastName = userProfile.LastName, 
                                                    Country = userProfile.Country };
                return PartialView("_EditProfile", model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditProfile(EditProfileModel profile)
        {
            if (ModelState.IsValid)
            {
                using (UsersContext db = new UsersContext())
                {
                    var userProfile = db.UserProfiles.Single(u => u.UserName == User.Identity.Name);
                    userProfile.FirstName = profile.FirstName;
                    userProfile.LastName = profile.LastName;
                    userProfile.Country = profile.Country;
                    db.SaveChanges();
                }
            }
            return PartialView("_EditProfile", profile);
        }

        [ChildActionOnly]
        public ActionResult RemoveExternalLogins()
        {
            ICollection<OAuthAccount> accounts = OAuthWebSecurity.GetAccountsFromUserName(User.Identity.Name);
            List<ExternalLogin> externalLogins = new List<ExternalLogin>();
            foreach (OAuthAccount account in accounts)
            {
                AuthenticationClientData clientData = OAuthWebSecurity.GetOAuthClientData(account.Provider);

                externalLogins.Add(new ExternalLogin
                {
                    Provider = account.Provider,
                    ProviderDisplayName = clientData.DisplayName,
                    ProviderUserId = account.ProviderUserId,
                });
            }

            ViewBag.ShowRemoveButton = externalLogins.Count > 1 || OAuthWebSecurity.HasLocalAccount(WebSecurity.GetUserId(User.Identity.Name));
            return PartialView("_RemoveExternalLoginsPartial", externalLogins);
        }

        //
        // POST: /Account/Disassociate

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Disassociate(string provider, string providerUserId)
        {
            string ownerAccount = OAuthWebSecurity.GetUserName(provider, providerUserId);
            ManageMessageId? message = null;

            // Only disassociate the account if the currently logged in user is the owner
            if (ownerAccount == User.Identity.Name)
            {
                // Use a transaction to prevent the user from deleting their last login credential
                using (var scope = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.Serializable }))
                {
                    bool hasLocalAccount = OAuthWebSecurity.HasLocalAccount(WebSecurity.GetUserId(User.Identity.Name));
                    if (hasLocalAccount || OAuthWebSecurity.GetAccountsFromUserName(User.Identity.Name).Count > 1)
                    {
                        OAuthWebSecurity.DeleteAccount(provider, providerUserId);
                        scope.Complete();
                        message = ManageMessageId.RemoveLoginSuccess;
                    }
                }
            }

            return RedirectToAction("Manage", new { Message = message });
        }

        #region Helpers

        public enum ManageMessageId
        {
            ChangePasswordSuccess,
            SetPasswordSuccess,
            RemoveLoginSuccess,
        }

        private static string ErrorCodeToString(MembershipCreateStatus createStatus)
        {
            // See http://go.microsoft.com/fwlink/?LinkID=177550 for
            // a full list of status codes.
            switch (createStatus)
            {
                case MembershipCreateStatus.DuplicateUserName:
                    return "User name already exists. Please enter a different user name.";

                case MembershipCreateStatus.DuplicateEmail:
                    return "A user name for that e-mail address already exists. Please enter a different e-mail address.";

                case MembershipCreateStatus.InvalidPassword:
                    return "The password provided is invalid. Please enter a valid password value.";

                case MembershipCreateStatus.InvalidEmail:
                    return "The e-mail address provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidAnswer:
                    return "The password retrieval answer provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidQuestion:
                    return "The password retrieval question provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidUserName:
                    return "The user name provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.ProviderError:
                    return "The authentication provider returned an error. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                case MembershipCreateStatus.UserRejected:
                    return "The user creation request has been canceled. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                default:
                    return "An unknown error occurred. Please verify your entry and try again. If the problem persists, please contact your system administrator.";
            }
        }
        #endregion
    }
}
