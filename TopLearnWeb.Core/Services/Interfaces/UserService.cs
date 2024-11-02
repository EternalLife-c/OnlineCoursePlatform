using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TopLearn.Core.Convertors;
using TopLearn.Core.DTOs;
using TopLearn.Core.Generators;
using TopLearn.Core.Security;
using TopLearn.DataLayer.Entities.Wallet;
using TopLearnWeb.DataLayer.Context;
using TopLearnWeb.DataLayer.Entities.User;

namespace TopLearn.Core.Services.Interfaces
{
    public class UserService : IUserService
    {
        private TopLearnContext _context;
        public UserService(TopLearnContext context)
        {
            _context = context;
        }

        public bool ActivateAccount(string ActiveCode)
        {
            var user = _context.Users.SingleOrDefault(u => u.ActivationCode == ActiveCode);
            if (user == null || user.IsActive)
            {
                return false;
            }

            user.IsActive = true;
            user.ActivationCode = NameGenerator.GenerateUniqeCode();
            _context.SaveChanges();
            return true;

        }

        public int AddUser(User user)
        {
            _context.Users.Add(user);
            _context.SaveChanges();
            return user.UserId;
        }

        public int AddUserFromAdminPanel(CreateUserViewModel user)
        {
            User addUser = new User();
            addUser.UserName = user.UserName;
            addUser.Email = user.Email;
            addUser.RegisterDate = DateTime.Now;
            addUser.IsActive = true;
            addUser.PassWord = PasswordHelper.EncodePasswordMd5(user.PassWord);
            addUser.ActivationCode = NameGenerator.GenerateUniqeCode();
            #region user avatar
            if (user.Avatar != null)
            {
                string imagepath = "";

                string NewAvatarName = SaveNewAvatar(imagepath, user.Avatar);

                addUser.Avatar = NewAvatarName;
            }
            #endregion

            return AddUser(addUser);
        }

        public int AddWalletTransaction(Wallet wallet)
        {
            _context.Wallets.Add(wallet);
            _context.SaveChanges();
            return wallet.WalletId;
        }

        public void ChangeUserPassWord(string username, string newPassWord)
        {
            var user = GetUserByUserName(username);
            user.PassWord = PasswordHelper.EncodePasswordMd5(newPassWord);
            UpdateUser(user);
        }

        public int ChargeWallet(string userName, int amount, string description, bool ispaid = false)
        {
            Wallet wallet = new Wallet()
            {
                Amount = amount,
                TransactionDate = DateTime.Now,
                Description = description,
                IsPaid = ispaid,
                TransactionTypeId = 1,
                UserId = GetUserIdByUserName(userName),
            };

            return AddWalletTransaction(wallet);
        }

        public bool CompareOldPassWord(string oldPassWord, string username)
        {
            string HashOldPassWord = PasswordHelper.EncodePasswordMd5(oldPassWord);
            return _context.Users.Any(u => u.UserName == username && u.PassWord == HashOldPassWord);
        }

        public void DeleteUser(int userId)
        {
            User user = GetUserById(userId);
            user.Deleted = true;
            UpdateUser(user);
        }

        public void EditProfile(string username, EditProfileViewModel profile)
        {
            if (profile.Avatar != null)
            {
                //initialize image path with null
                string imagepath = "";

                //check if the image is the default one or the user has uploaded a custom profile picture
                if (profile.AvatarName != "Default Avatar.jpg")
                {
                    imagepath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/UserAvatar", profile.AvatarName);

                    //delete the existing file (if the user had uploaded a custom profile picture)
                    if (File.Exists(imagepath))
                    {
                        File.Delete(imagepath);
                    }
                }

                //Save the New File to wwwroot
                string NewAvatarName = SaveNewAvatar(imagepath, profile.Avatar);

                var user = GetUserByUserName(username);
                user.UserName = profile.UserName;
                user.Avatar = NewAvatarName;
                UpdateUser(user);
            }
            else if (profile.Avatar == null)
            {
                var user = GetUserByUserName(username);
                user.UserName = profile.UserName;
                UpdateUser(user);
            }

        }

        public void EditUserFromAdminPanel(EditUserViewModel editUser)
        {
            User user = GetUserById(editUser.UserId);
            user.Email = editUser.Email;

            //if the admin leaves the password input alone, the user's password won't change - if he does, the method will change it.
            if (!string.IsNullOrEmpty(editUser.PassWord))
            {
                user.PassWord = PasswordHelper.EncodePasswordMd5(editUser.PassWord);
            }

            //Delete Old Image if existed...
            if (user.Avatar != "Default Avatar.jpg")
            {
                string deletePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/UserAvatar", editUser.AvatarName);
                if (File.Exists(deletePath))
                {
                    {
                        File.Delete(deletePath);
                    } 
                }
            }

            string NewAvatarName = SaveNewAvatar("wwwroot/UserAvatar", editUser.Avatar);
            user.Avatar = NewAvatarName;

            _context.Users.Update(user);
            _context.SaveChanges();
        }

        public bool EmailExists(string email)
        {
            return _context.Users.Any(u => u.Email == email);
        }

        public EditProfileViewModel GetDataForUserProfileEdit(string username)
        {
            return _context.Users.Where(u => u.UserName == username).Select(u => new EditProfileViewModel()
            {
                UserName = u.UserName,
                AvatarName = u.Avatar
            }).Single();
        }

        public UsersForAdminViewModel GetDeletedUsers(int pageId = 1, string emailFilter = "", string userNameFilter = "")
        {
            IQueryable<User> result = _context.Users.IgnoreQueryFilters().Where(u=> u.Deleted);
            if (!string.IsNullOrEmpty(emailFilter))
            {
                result = result.Where(u => u.Email.Contains(emailFilter));
            }
            if (!string.IsNullOrEmpty(userNameFilter))
            {
                result = result.Where(u => u.UserName.Contains(userNameFilter));
            }

            //the part that we determine how many users are listed in one page and how many should a page contain
            int take = 25;
            int skip = (pageId - 1) * take;

            UsersForAdminViewModel list = new UsersForAdminViewModel();
            list.CurrentPage = pageId;
            list.PageCount = result.Count() / take;
            list.Users = result.OrderBy(u => u.RegisterDate).Skip(skip).Take(take).ToList();

            return list;
        }

        public SideBarUserInfoViewModel GetSideBarUserInfo(string username)
        {
            return _context.Users.Where(u => u.UserName == username).Select(u => new SideBarUserInfoViewModel()
            {
                UserName = u.UserName,
                ImageName = u.Avatar
            }).Single();
        }

        public User GetUserByActicationCode(string ActicationCode)
        {
            return _context.Users.SingleOrDefault(u => u.ActivationCode == ActicationCode);
        }

        public User GetUserByEmail(string Email)
        {
            return _context.Users.SingleOrDefault(u => u.Email == Email);
        }

        public User GetUserById(int userId)
        {
            return _context.Users.Find(userId);
        }

        public User GetUserByUserName(string userName)
        {
            return _context.Users.SingleOrDefault(u => u.UserName == userName);
        }

        public EditUserViewModel GetUserForEdit(int userId)
        {
            return _context.Users.Where(u => u.UserId == userId).Select(u => new EditUserViewModel()
            {
                UserId = u.UserId,
                AvatarName = u.Avatar,
                UserName = u.UserName,
                Email = u.Email,
                UserRoles = u.UserRoles.Select(r => r.RoleId).ToList()

            }).Single();
        }

        public int GetUserIdByUserName(string userName)
        {
            return _context.Users.Single(u => u.UserName == userName).UserId;
        }

        public UserInformationViewModel GetUserInformation(string username)
        {
            var user = GetUserByUserName(username);
            UserInformationViewModel information = new UserInformationViewModel();
            information.UserName = user.UserName;
            information.Email = user.Email;
            information.RegisterDate = user.RegisterDate;
            information.Wallet = UserWalletBalance(username);

            return information;
        }

        public UserInformationViewModel GetUserInformation(int userId)
        {
            var user = GetUserById(userId);
            UserInformationViewModel information = new UserInformationViewModel();
            information.UserName = user.UserName;
            information.Email = user.Email;
            information.RegisterDate = user.RegisterDate;
            information.Wallet = UserWalletBalance(user.UserName);

            return information;
        }

        public UsersForAdminViewModel GetUsers(int pageId = 1, string emailFilter = "", string userNameFilter = "")
        {
            IQueryable<User> result = _context.Users;
            if (!string.IsNullOrEmpty(emailFilter))
            {
                result = result.Where(u => u.Email.Contains(emailFilter));
            }
            if (!string.IsNullOrEmpty(userNameFilter))
            {
                result = result.Where(u => u.UserName.Contains(userNameFilter));
            }

            //the part that we determine how many users are listed in one page and how many should a page contain
            int take = 25;
            int skip = (pageId - 1) * take;

            UsersForAdminViewModel list = new UsersForAdminViewModel();
            list.CurrentPage = pageId;
            list.PageCount = result.Count() / take;
            list.Users = result.OrderBy(u => u.RegisterDate).Skip(skip).Take(take).ToList();

            return list;
        }

        public List<WalletViewModel> GetUserWallet(string userName)
        {
            int userId = GetUserIdByUserName(userName);
            return _context.Wallets.Where(w => w.UserId == userId && w.IsPaid)
                .Select(w => new WalletViewModel()
                {
                    Amount = w.Amount,
                    Description = w.Description,
                    Type = w.TransactionTypeId, //check later in case of problems
                    DateTime = w.TransactionDate
                })
                .ToList();
        }

        public Wallet GetWalletByWalletId(int walletId)
        {
            return _context.Wallets.Find(walletId);
        }

        public User LoginUser(LoginViewModel login)
        {
            string hashpassword = PasswordHelper.EncodePasswordMd5(login.PassWord);
            string email = FixedText.FixEmail(login.Email);

            return _context.Users.SingleOrDefault(u => u.PassWord == hashpassword && u.Email == login.Email);
        }

        public string SaveNewAvatar(string imagepath, IFormFile Avatar)
        {

            //generate a unique name for the new profile picture + the extension of the newly uploaded file
            string AvatarName = NameGenerator.GenerateUniqeCode() + Path.GetExtension(Avatar.FileName);

            //image file's full path - combine the created name+extension with directory of the file
            imagepath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/UserAvatar", AvatarName);

            //write the uploaded file to the disk at the specified imagepath.
            using (var stream = new FileStream(imagepath, FileMode.Create))
            {
                Avatar.CopyTo(stream);
            }

            return AvatarName;
        }

        public void UpdateUser(User user)
        {
            _context.Update(user);
            _context.SaveChanges();
        }

        public void UpdateWallet(Wallet wallet)
        {
            _context.Wallets.Update(wallet);
            _context.SaveChanges();
        }

        public bool UserNameExists(string userName)
        {
            return _context.Users.Any(u => u.UserName == userName);
        }

        public int UserWalletBalance(string userName)
        {
            int userId = GetUserIdByUserName(userName);
            var deposite = _context.Wallets
                .Where(w => w.UserId == userId && w.TransactionTypeId == 1 && w.IsPaid)
                .Select(w => w.Amount).ToList();
            var withdrawal = _context.Wallets
                .Where(w => w.UserId == userId && w.TransactionTypeId == 2)
                .Select(w => w.Amount).ToList();

            return (deposite.Sum() - withdrawal.Sum());
        }
    }
}
