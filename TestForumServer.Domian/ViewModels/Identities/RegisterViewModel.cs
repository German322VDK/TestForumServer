namespace TestForumServer.Domian.ViewModels.Identities
{
    /// <summary>
    /// Represents the view model for user registration.
    /// Inherits from <see cref="LoginViewModel"/>.
    /// Contains additional properties for nickname and password confirmation.
    /// </summary>
    public class RegisterViewModel : LoginViewModel
    {
        /// <summary>
        /// Gets or sets the nickname of the user.
        /// </summary>
        public string NickName { get; set; }

        /// <summary>
        /// Gets or sets the confirmation password for the user registration.
        /// </summary>
        public string PasswordConfirm { get; set; }
    }
}
