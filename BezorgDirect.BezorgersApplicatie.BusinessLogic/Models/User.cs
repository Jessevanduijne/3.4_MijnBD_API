using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;

namespace BezorgDirect.BezorgersApplicatie.BusinessLogic.Models
{
    /* This class was setup by Lennart de Waart (563079) */
    public class User // User probably should be abstract, but this gives problems with JSON serialization
    {
        [Key, Required]
        public Guid Id { get; set; } // NOT NULL

        [Required]
        public string EmailAddress { get; set; } // NOT NULL

        [Required]
        [JsonIgnore]
        public string Hash { get; set; }

        public string Token { get; set; }

        /// <summary>
        /// Public constructor for the entire User model
        /// </summary>
        /// <param name="id"></param>
        /// <param name="emailAddress"></param>
        /// <param name="hash"></param>
        /// <param name="token"></param>
        public User(Guid id, string emailAddress, string hash, string token)
        {
            this.Id = id;
            this.EmailAddress = emailAddress;
            this.Hash = hash;
            this.Token = token;
        }

        /// <summary>
        /// Register a user,
        /// Tiamo Idzenga, 582063
        /// </summary>
        /// <param name="emailAddress"></param>
        /// <param name="password"></param>
        public User(string emailAddress, string password)
        {
            this.Id = Guid.NewGuid();
            this.EmailAddress = emailAddress;
            this.Hash = EncryptPassword(password);
            this.Token = GenerateToken();
        }

        /// <summary>
        /// Generates a random salt for password storage,
        /// Tiamo Idzenga, 582063
        /// </summary>
        /// <returns>Random Salt</returns>
        private byte[] _generateSalt()
        {
            byte[] salt = new byte[16];

            using (RNGCryptoServiceProvider rnd = new RNGCryptoServiceProvider())
                rnd.GetBytes(salt);

            return salt;
        }

        /// <summary>
        /// Generates a random Bearer Token,
        /// Tiamo Idzenga, 582063
        /// </summary>
        /// <returns>Bearer Token</returns>
        protected string GenerateToken()
        {
            return Guid.NewGuid().ToString("N");
        }

        /// <summary>
        /// Hashes the password,
        /// Tiamo Idzenga, 582063
        /// </summary>
        /// <param name="password"></param>
        /// <returns>The hashed password, salt appended after :</returns>
        protected string EncryptPassword(string password)
        {
            byte[] salt = _generateSalt();
            byte[] encryptedPasswordBytes = KeyDerivation.Pbkdf2(
                password, 
                salt, 
                KeyDerivationPrf.HMACSHA512, 
                10000, 
                16
            );

            return $"{Convert.ToBase64String(salt)}:{Convert.ToBase64String(encryptedPasswordBytes)}";
        }

        /// <summary>
        /// Checkes if input matches the encrypted password stored in the database,
        /// Tiamo Idzenga, 582063
        /// </summary>
        /// <param name="input"></param>
        /// <returns>bool</returns>
        public bool PasswordMatches(string input)
        {
            try {
                string[] hashParts = this.Hash.Split(':');
                byte[] salt = Convert.FromBase64String(hashParts[0]);
                string encryptedPassword = hashParts[1];

                byte[] encryptedInputBytes = KeyDerivation.Pbkdf2(
                    input,
                    salt,
                    KeyDerivationPrf.HMACSHA512,
                    10000,
                    16
                );

                return encryptedPassword ==
                    Convert.ToBase64String(encryptedInputBytes);
            } catch {
                return false;
            }
        }

        /// <summary>
        /// Checkes if User is logged in,
        /// Tiamo Idzenga, 582063
        /// </summary>
        /// <returns>bool</returns>
        public bool IsLoggedIn()
        {
            return this.Token != null;
        }

        /// <summary>
        /// Log the user in, giving him a Token,
        /// Tiamo Idzenga, 582063
        /// </summary>
        public void LogIn()
        {
            this.Token = GenerateToken();
        }

        /// <summary>
        /// Log the user out, removing his token,
        /// Tiamo Idzenga 582063
        /// </summary>
        public void LogOut()
        {
            this.Token = null;
        }
    }
}