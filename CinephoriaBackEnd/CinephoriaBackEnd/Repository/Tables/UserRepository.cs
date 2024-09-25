using CinephoriaBackEnd.Data;
using CinephoriaBackEnd.Models;
using Microsoft.EntityFrameworkCore;


namespace CinephoriaBackEnd.Repository
{
    /// <summary>
    /// Interface pour gérer les opérations liées aux utilisateurs
    /// </summary>
    public interface IUserRepository
    {
        /// <summary>
        /// Enregistrer un nouvel utilisateur
        /// </summary>
        /// <param name="user">Les détails de l'utilisateur à enregistrer</param>
        /// <param name="password">Le mot de passe de l'utilisateur</param>
        /// <returns>L'utilisateur créé</returns>
        Task<AppUser> RegisterUser(AppUser user, string password); // POST /api/auth/register

        /// <summary>
        /// Connexion d'un utilisateur
        /// </summary>
        /// <param name="email">L'adresse e-mail de l'utilisateur</param>
        /// <param name="password">Le mot de passe de l'utilisateur</param>
        /// <returns>L'utilisateur connecté</returns>
        Task<AppUser> LoginUser(string email, string password); // POST /api/auth/login

        /// <summary>
        /// Récupérer les informations d'un utilisateur par son ID
        /// </summary>
        /// <param name="userId">L'ID de l'utilisateur</param>
        /// <returns>Les détails de l'utilisateur</returns>
        Task<AppUser> GetUserById(string userId); // GET /api/users/me

        /// <summary>
        /// Mettre à jour les informations d'un utilisateur
        /// </summary>
        /// <param name="user">Les détails mis à jour de l'utilisateur</param>
        Task UpdateUser(AppUser user); // PUT /api/users/me

        /// <summary>
        /// Créer un compte administrateur
        /// </summary>
        /// <param name="admin">Les détails de l'administrateur à créer</param>
        /// <returns>L'administrateur créé</returns>
        Task<AppUser> CreateAdmin(AppUser admin); // POST /api/admins

        /// <summary>
        /// Récupérer la liste de tous les utilisateurs
        /// </summary>
        /// <returns>Liste de tous les utilisateurs</returns>
        Task<List<AppUser>> GetAllUsers(); // GET /api/admins/users

        /// <summary>
        /// Créer un compte employé
        /// </summary>
        /// <param name="employee">Les détails de l'employé à créer</param>
        /// <returns>L'employé créé</returns>
        Task<AppUser> CreateEmployee(AppUser employee); // POST /api/admins/employees
    }

    public class UserRepository : IUserRepository
    {
        private readonly CinephoriaDbContext _context;

        public UserRepository(CinephoriaDbContext context)
        {
            _context = context;
        }

        public async Task<AppUser> RegisterUser(AppUser user, string password)
        {
            // Logique pour créer un nouvel utilisateur et hacher le mot de passe
            return user;
        }

        public async Task<AppUser> LoginUser(string email, string password)
        {
            // Logique pour valider les informations de connexion
            return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<AppUser> GetUserById(string userId)
        {
            return await _context.Users.FindAsync(userId);
        }

        public async Task UpdateUser(AppUser user)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }

        public async Task<AppUser> CreateAdmin(AppUser admin)
        {
            // Logique pour créer un admin
            return admin;
        }

        public async Task<List<AppUser>> GetAllUsers()
        {
            return await _context.Users.ToListAsync();
        }

        public async Task<AppUser> CreateEmployee(AppUser employee)
        {
            // Logique pour créer un employé
            return employee;
        }
    }

}
