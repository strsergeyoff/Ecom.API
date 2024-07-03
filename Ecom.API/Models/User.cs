namespace Ecom.API.Models
{
    /// <summary>
    /// Пользователь
    /// </summary>
    public class User
    {
        /// <summary>
        /// Индификатор
        /// </summary>
        public Guid Id { get; private set; }

        /// <summary>
        /// Логин
        /// </summary>
        public string Login { get; set; }

        /// <summary>
        /// Пароль
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// Роль
        /// </summary>
        public Role Role { get; set; }

        /// <summary>
        /// Магазины
        /// </summary>
        public virtual ICollection<rise_project> Stores { get; set; }

    }
}
