using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TopLearn.Core.DTOs;
using TopLearn.DataLayer.Entities.Wallet;
using TopLearnWeb.DataLayer.Entities.User;

namespace TopLearn.Core.Services.Interfaces
{
    public interface IUserService
    {
        #region User

        bool UserNameExists(string userName);
        bool EmailExists(string email);
        int AddUser(User user);
        void UpdateUser(User user);
        User LoginUser(LoginViewModel login);
        bool ActivateAccount(string ActiveCode);
        int GetUserIdByUserName(string userName);
        User GetUserById(int userId);
        User GetUserByEmail(string Email);
        User GetUserByUserName(string userName);
        User GetUserByActicationCode(string ActicationCode);
        void DeleteUser(int userId);

        #endregion

        #region User Panel

        UserInformationViewModel GetUserInformation(string username);
        UserInformationViewModel GetUserInformation(int userId);
        SideBarUserInfoViewModel GetSideBarUserInfo(string username);
        EditProfileViewModel GetDataForUserProfileEdit(string username);
        void EditProfile(string username, EditProfileViewModel profile);
        bool CompareOldPassWord(string oldPassWord, string username);
        void ChangeUserPassWord(string username, string newPassWord);

        #endregion

        #region Wallet

        int UserWalletBalance(string userName);
        List<WalletViewModel> GetUserWallet(string userName);
        int ChargeWallet(string userName, int amount, string description, bool ispaid = false);
        int AddWalletTransaction(Wallet wallet);
        Wallet GetWalletByWalletId(int walletId);
        void UpdateWallet(Wallet wallet);
        #endregion

        #region Admin Panel

        //parametere for filtering the user list by the email entered in the search box or the username entered
        //and we can also add any other filter that we want like password or phone number ...
        UsersForAdminViewModel GetUsers(int pageId = 1, string emailFilter = "", string userNameFilter="");
        UsersForAdminViewModel GetDeletedUsers(int pageId = 1, string emailFilter = "", string userNameFilter="");
        int AddUserFromAdminPanel(CreateUserViewModel user);
        EditUserViewModel GetUserForEdit(int userId);
        void EditUserFromAdminPanel(EditUserViewModel editUser);
        #endregion

        #region Profile Picture

        //takes imagepath, the name of the file located in the database and the file itself, saves it and returns the NEW AVATAR NAME+path
        string SaveNewAvatar(string imagepath, IFormFile Avatar);

        #endregion
    }
}
